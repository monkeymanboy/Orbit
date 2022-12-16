using Atlas.Orbit.Components;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;
using System;

namespace Atlas.Orbit.Parser {
    using Attributes;
    using ComponentProcessors;
    using Macros;
    using System.Diagnostics;
    using Util;
    using Debug = UnityEngine.Debug;

    public class OrbitParser {
        internal const string RETRIEVE_VALUE_PREFIX = "~";
        internal const string PARENT_HOST_VALUE_PREFIX = "^";

        internal const BindingFlags HOST_FLAGS =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static OrbitParser parser;

        public static OrbitParser DefaultParser {
            get {
                if(parser == null) {
                    parser = new OrbitParser();
                }

                return parser;
            }
        }

        public List<ComponentProcessor> ComponentProcessors { get; set; }
        public Dictionary<string, Macro> Macros { get; set; }
        public Dictionary<string, GameObject> Prefabs { get; set; }

        private XmlDocument doc = new XmlDocument();
        private XmlReaderSettings readerSettings = new XmlReaderSettings();

        private bool initialized = false;

        public virtual void Init() {
            readerSettings.IgnoreComments = true;
            ComponentProcessors = UtilReflection.GetAllSubclasses<ComponentProcessor>();
            Macros = new Dictionary<string, Macro>();
            Prefabs = new Dictionary<string, GameObject>();
            foreach(Macro macro in UtilReflection.GetAllSubclasses<Macro>()) {
                macro.Parser = this;
                Macros.Add(macro.Tag, macro);
            }

            initialized = true;
        }

        public void AddAssemblyTypes(Assembly assembly) {
            ComponentProcessors.AddRange(UtilReflection.GetAllSubclasses<ComponentProcessor>(assembly));
            foreach(Macro macro in UtilReflection.GetAllSubclasses<Macro>(assembly)) {
                macro.Parser = this;
                Macros.Add(macro.Tag, macro);
            }
        }

        public UIRenderData Parse(string content, GameObject parent, object host = null,
            UIRenderData parentData = null) {
            doc.Load(XmlReader.Create(new StringReader(content), readerSettings));
            return Parse(doc, parent, host, parentData);
        }

        public UIRenderData Parse(XmlNode parentNode, GameObject parent, object host = null,
            UIRenderData parentData = null, Action<UIRenderData> preParse = null) {
            if(!initialized) {
#if ORBIT_BENCHMARK
                Stopwatch stopwatch = new();
                stopwatch.Start();
#endif
                Init();
#if ORBIT_BENCHMARK
                stopwatch.Stop();
                Debug.Log($"Orbit Init: {stopwatch.ElapsedMilliseconds}ms");
#endif
            }

#if ORBIT_BENCHMARK
            Stopwatch parseStopWatch = new();
            parseStopWatch.Start();
#endif
            
            UIRenderData renderData = new UIRenderData { Host = host, ParentRenderData = parentData, Parser = this };

            if(host != null) {
                List<(string, FieldInfo)> eventEmitterFields = null;
                foreach(FieldInfo fieldInfo in host.GetType().GetFields(HOST_FLAGS)) {
                    EventEmitterAttribute eventEmitter =
                        fieldInfo.GetCustomAttributes(typeof(EventEmitterAttribute), true).FirstOrDefault() as
                            EventEmitterAttribute;
                    if(eventEmitter != null) {
                        fieldInfo.SetValue(host, new Action(() => renderData.EmitEvent(eventEmitter.ID)));
                        if(eventEmitterFields == null)
                            eventEmitterFields = new();
                        eventEmitterFields.Add((eventEmitter.ID, fieldInfo));
                    }

                    ValueIDAttribute valueID =
                        fieldInfo.GetCustomAttributes(typeof(ValueIDAttribute), true).FirstOrDefault() as
                            ValueIDAttribute;
                    if(valueID == null)
                        continue;
                    renderData.Values.Add(string.IsNullOrEmpty(valueID.ID) ? fieldInfo.Name : valueID.ID,
                        new UIFieldValue(renderData, fieldInfo));
                }
                renderData.EventEmitterFields = eventEmitterFields;

                foreach(PropertyInfo propInfo in host.GetType().GetProperties(HOST_FLAGS)) {
                    ValueIDAttribute valueID =
                        propInfo.GetCustomAttributes(typeof(ValueIDAttribute), true).FirstOrDefault() as
                            ValueIDAttribute;
                    if(valueID == null)
                        continue;
                    UIPropertyValue uiPropertyValue = new UIPropertyValue(renderData, propInfo);
                    renderData.Values.Add(string.IsNullOrEmpty(valueID.ID) ? propInfo.Name : valueID.ID,
                        uiPropertyValue);
                    renderData.Properties.Add(propInfo.Name, uiPropertyValue);
                    renderData.PropertyInfoCache.Add(propInfo.Name, propInfo);
                }

                foreach(MethodInfo methodInfo in host.GetType().GetMethods(HOST_FLAGS)) {
                    ValueIDAttribute valueID =
                        methodInfo.GetCustomAttributes(typeof(ValueIDAttribute), true).FirstOrDefault() as
                            ValueIDAttribute;
                    if(valueID != null) {
                        renderData.SetValue(string.IsNullOrEmpty(valueID.ID) ? methodInfo.Name : valueID.ID,
                            new UIFunction(renderData, methodInfo));
                    }

                    ListenForAttribute listenFor =
                        methodInfo.GetCustomAttributes(typeof(ListenForAttribute), true).FirstOrDefault() as
                            ListenForAttribute;
                    if(listenFor == null)
                        continue;
                    if(methodInfo.GetParameters().Length == 1)
                        renderData.AddChildEvent(listenFor.Events,
                            (host) => methodInfo.Invoke(renderData.Host, new object[] { host }));
                    else
                        renderData.AddEvent(listenFor.Events,
                            () => methodInfo.Invoke(renderData.Host, new object[] { }));
                }
            }

            renderData.Values.Add("this", new UIHostValue(renderData));

            renderData.AddEvent("RefreshAll",
                renderData
                    .RefreshAllValues); //TODO(David): Not sure if I want this to actually exist or not, should reconsider after implementing property binding

            renderData.EmitEvent("PreParse");

            preParse?.Invoke(renderData);

            foreach (XmlNode node in parentNode.ChildNodes) {
                GameObject nodeGO = RenderNode(node, parent, renderData);
                if(nodeGO != null) renderData.RootObjects.Add(nodeGO);
                //TODO(David): if nodeGO is null we parsed a macro, this is fine to ignore in most cases however if the macro has child nodes those should be included in root objects
            }

            if(host != null) {
                List<(string, FieldInfo)> viewComponentFields = null;
                foreach(FieldInfo fieldInfo in host.GetType().GetFields(HOST_FLAGS)) {
                    //TODO(David): ViewComponentAttributes could be cached so we don't need to iterate over the fields twice
                    ViewComponentAttribute objectID =
                        fieldInfo.GetCustomAttributes(typeof(ViewComponentAttribute), true).FirstOrDefault() as
                            ViewComponentAttribute;
                    if(objectID == null) continue;
                    if(viewComponentFields == null)
                        viewComponentFields = new();
                    viewComponentFields.Add(new(objectID.ID, fieldInfo));
                    if(renderData.GetValueFromID(objectID.ID).GetValue() is MarkupPrefab markupPrefab) {
                        fieldInfo.SetValue(host, markupPrefab.FindComponent(fieldInfo.FieldType));
                    } else {
                        throw new Exception(
                            "Tried using [ViewComponent] on an ID that is not bound to an object in the view");
                    }
                }

                renderData.ViewComponentFields = viewComponentFields;
            }

            renderData.EmitEvent("PostParse");
#if ORBIT_BENCHMARK
            parseStopWatch.Stop();
            Debug.Log($"Parsed {host.GetType().Name} in {parseStopWatch.ElapsedMilliseconds}ms");
#endif
            return renderData;
        }

        public GameObject RenderNode(XmlNode node, GameObject parent, UIRenderData renderData) {
            TagParameters parameters = new TagParameters();
            parameters.RenderData = renderData;
            if(parameters.Data == null) 
                parameters.Data = new Dictionary<string, string>();
            if(parameters.Values == null) 
                parameters.Values = new Dictionary<string, UIValue>(); //TODO(David) reuse Dictionaries?
            foreach(XmlAttribute attribute in node.Attributes) {
                string propertyName = attribute.Name;
                string value = attribute.Value;
                if(value.StartsWith(RETRIEVE_VALUE_PREFIX)) {
                    string valueID = value.Substring(RETRIEVE_VALUE_PREFIX.Length);

                    UIValue uiValue = renderData.GetValueFromID(valueID);

                    parameters.Values.Add(propertyName, uiValue);
                    parameters.Data.Add(propertyName,
                        null); //If data points to a null string it is bound to a property/field
                    continue;
                }

                parameters.Data.Add(propertyName, value);
            }

            parameters.Values.Add("_Node", new DefinedUIValue(null, node));
            parameters.Data.Add("_Node", null);

            if(Macros.TryGetValue(node.Name, out Macro macro)) {
                RenderMacroNode(node, macro, parent, parameters);
                return null;
            }

            return RenderTagNode(node, parent, parameters);
        }

        private GameObject RenderTagNode(XmlNode node, GameObject parent, TagParameters parameters) {
            GameObject nodeGO = CreatePrefab(node.Name, parent);
            MarkupPrefab markupPrefab = nodeGO.GetComponent<MarkupPrefab>();
            if(markupPrefab == null)
                throw new Exception($"'Orbit/Prefabs/{node.Name}' is missing it's MarkupPrefab component");
            foreach(ComponentProcessor processor in ComponentProcessors) {
                //TODO(PERFORMANCE): It might be better to instead loop through the components on the prefab and then look up the processors in a dictionary, though with a small number of processors this should be fine for a while.
                Component component = markupPrefab.FindComponent(processor.ComponentType);
                if(component != null) {
                    processor.Process(component, parameters);
                }
            }

            if(markupPrefab.ParseChildren) {
                foreach(XmlNode childNode in node.ChildNodes)
                    RenderNode(childNode, markupPrefab.ChildrenContainer, parameters.RenderData);
            }

            return nodeGO;
        }

        private void RenderMacroNode(XmlNode node, Macro macro, GameObject parent, TagParameters parameters) {
            macro.Execute(node, parent, parameters);
        }

        public virtual GameObject CreatePrefab(string name, GameObject parent) {
            if(!Prefabs.TryGetValue(name, out GameObject prefab)) {
                prefab = Resources.Load<GameObject>($"Orbit/Prefabs/{name}");
                Prefabs.Add(name, prefab);
            }
            GameObject newObject = GameObject.Instantiate(prefab, parent.transform);
            return newObject;
        }

        public static void SetDefaultParser(OrbitParser parser) {
            OrbitParser.parser = parser;
        }
    }
}