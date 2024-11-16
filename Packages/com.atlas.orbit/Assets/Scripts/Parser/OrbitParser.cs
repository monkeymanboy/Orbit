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
    using Attributes.TagGenerators;
    using ComponentProcessors;
    using Macros;
    using System.Diagnostics;
    using Util;
    using Debug = UnityEngine.Debug;

    public class OrbitParser {
        internal const char RETRIEVE_VALUE_PREFIX = '~';
        internal const char PARENT_HOST_VALUE_PREFIX = '^';
        internal const char NEGATE_VALUE_PREFIX = '!';

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
        
        public Dictionary<string, Color> ColorDefinitions { get; set; } = new();

        private XmlDocument doc = new();
        private XmlReaderSettings readerSettings = new();
        private Dictionary<string, List<ComponentProcessor>> processorPrefabCache = new();

        private bool initialized = false;

        public XmlDocument XmlDocument => doc;

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

        public void DefineColor(string name, Color color) {
            ColorDefinitions.Remove(name);
            ColorDefinitions.Add(name, color);
        }

        public void AddAssemblyTypes(Assembly assembly) {
            ComponentProcessors.AddRange(UtilReflection.GetAllSubclasses<ComponentProcessor>(assembly));
            foreach(Macro macro in UtilReflection.GetAllSubclasses<Macro>(assembly)) {
                macro.Parser = this;
                Macros.Add(macro.Tag, macro);
            }
        }

        public XmlNode ParseXML(string content) {
            doc.Load(XmlReader.Create(new StringReader(content), readerSettings));
            return doc;
        }

        public UIRenderData Parse(string content, GameObject parent, object host = null,
            UIRenderData parentData = null) {
            return Parse(ParseXML(content), parent, host, parentData);
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
                    EventEmitterAttribute eventEmitter = fieldInfo.GetCustomAttribute<EventEmitterAttribute>(true);
                    if(eventEmitter != null) {
                        fieldInfo.SetValue(host, new Action(() => renderData.EmitEvent(eventEmitter.ID)));
                        if(eventEmitterFields == null)
                            eventEmitterFields = new();
                        eventEmitterFields.Add((eventEmitter.ID, fieldInfo));
                    }
                    ValueIDAttribute valueID = fieldInfo.GetCustomAttribute<ValueIDAttribute>(true);
                    string id = null;
                    if(valueID != null)
                        id = valueID.ID ?? fieldInfo.Name;
                    TagGenerator tagGenerator = fieldInfo.GetCustomAttribute<TagGenerator>(true);
                    if(tagGenerator != null) {
                        //Allows UIValues to be made implicitly by presence of a tag generator attribute without ValueID
                        id ??= fieldInfo.Name;
                        renderData.TagGenerators.Add((id, tagGenerator));
                    }
                    if(id == null)
                        continue;
                    renderData.Values.Add(id, new UIFieldValue(renderData, fieldInfo));
                }
                renderData.EventEmitterFields = eventEmitterFields;

                foreach(PropertyInfo propInfo in host.GetType().GetProperties(HOST_FLAGS)) {
                    ValueIDAttribute valueID = propInfo.GetCustomAttribute<ValueIDAttribute>(true);
                    string id = null;
                    if(valueID != null)
                        id = valueID.ID ?? propInfo.Name;
                    TagGenerator tagGenerator = propInfo.GetCustomAttribute<TagGenerator>(true);
                    if(tagGenerator != null) {
                        //Allows UIValues to be made implicitly by presence of a tag generator attribute without ValueID
                        id ??= propInfo.Name;
                        renderData.TagGenerators.Add((id, tagGenerator));
                    } 
                    if(id == null)
                        continue;
                    UIPropertyValue uiPropertyValue = new(renderData, propInfo);
                    renderData.Values.Add(id, uiPropertyValue);
                    renderData.Properties.Add(propInfo.Name, uiPropertyValue);
                }

                foreach(MethodInfo methodInfo in host.GetType().GetMethods(HOST_FLAGS)) {
                    ValueIDAttribute valueID = methodInfo.GetCustomAttribute<ValueIDAttribute>(true);
                    if(valueID != null) {
                        renderData.SetValue(string.IsNullOrEmpty(valueID.ID) ? methodInfo.Name : valueID.ID,
                            new UIFunction(renderData, methodInfo));
                    }
                    ListenForAttribute listenFor = methodInfo.GetCustomAttribute<ListenForAttribute>(true);
                    if(listenFor == null)
                        continue;
                    if(methodInfo.GetParameters().Length == 1)
                        renderData.AddChildEvent(listenFor.Events,
                            (host) => methodInfo.Invoke(renderData.Host, ArrayParameters<object>.Single(host)));
                    else
                        renderData.AddEvent(listenFor.Events,
                            () => methodInfo.Invoke(renderData.Host, Array.Empty<object>()));
                }
            }

            renderData.Values.Add("this", new UIHostValue(renderData));
            renderData.AddEvent("RefreshAll", renderData.RefreshAllValues);
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
                    ViewComponentAttribute objectID = fieldInfo.GetCustomAttribute<ViewComponentAttribute>(true);
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
                    string valueID = value.Substring(1);

                    UIValue uiValue = renderData.GetValueFromID(valueID);

                    parameters.Values.Add(propertyName, uiValue);
                    parameters.Data.Add(propertyName,
                        null); //If data points to a null string it is bound to a property/field
                    continue;
                }

                parameters.Data.Add(propertyName, value);
            }

            parameters.Values.Add("_Node", new DefinedUIValue<XmlNode>(null, node));
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

            if(processorPrefabCache.TryGetValue(node.Name, out List<ComponentProcessor> processors)) {
                foreach(ComponentProcessor processor in processors) {
                    markupPrefab.ProcessComponentType(processor, parameters);
                }
            } else {
                //First time encountering this object it must search all the components and build a list for the cache
                List<ComponentProcessor> processorCache = new();
                foreach(ComponentProcessor processor in ComponentProcessors) {
                    if(markupPrefab.ProcessComponentType(processor, parameters))
                        processorCache.Add(processor);
                }
                processorPrefabCache.Add(node.Name, processorCache);
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