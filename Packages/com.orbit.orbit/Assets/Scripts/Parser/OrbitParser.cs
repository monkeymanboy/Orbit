﻿using Orbit.Components;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;
using System;

namespace Orbit.Parser {
    using Attributes;
    using Attributes.TagGenerators;
    using ComponentProcessors;
    using Components;
    using Macros;
    using System.Diagnostics;
    using Util;
    using Debug = UnityEngine.Debug;

    public class OrbitParser {
        internal const char RETRIEVE_VALUE_PREFIX = '~';
        internal const char PARENT_HOST_VALUE_PREFIX = '^';
        internal const char NEGATE_VALUE_PREFIX = '!';
        internal const char RESOURCE_VALUE_PREFIX = '@';
        internal const char GLOBAL_VALUE_PREFIX = '$';

        internal const BindingFlags HOST_FLAGS =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static OrbitParser parser;

        public static OrbitParser DefaultParser {
            get {
                if(parser == null) {
                    parser = new OrbitParser();
#if ORBIT_BENCHMARK
                    Stopwatch stopwatch = new();
                    stopwatch.Start();
#endif
                    parser.Init(OrbitConfig.Config);
#if ORBIT_BENCHMARK
                    stopwatch.Stop();
                    Debug.Log($"Orbit Init: {stopwatch.ElapsedMilliseconds}ms");
#endif
                }

                return parser;
            }
        }

        public List<ComponentProcessor> ComponentProcessors { get; set; }
        public Dictionary<string, Macro> Macros { get; set; }
        public Dictionary<string, GameObject> Prefabs { get; set; }
        
        public Dictionary<string, Color> ColorDefinitions { get; set; } = new();
        public Dictionary<string, UIValue> Globals { get; set; } = new();

        private XmlDocument doc = new();
        private XmlReaderSettings readerSettings = new();
        private Dictionary<string, List<(ComponentProcessor,int)>> processorPrefabCache = new();
        private Dictionary<string, TagParameters.BoundData> reusableParameterData = new();
        private List<Component> reusableComponentList = new();

        public XmlDocument XmlDocument => doc;

        public virtual void Init(OrbitConfig config) {
            readerSettings.IgnoreComments = true;
            ComponentProcessors = UtilReflection.GetAllSubclasses<ComponentProcessor>();
            Macros = new Dictionary<string, Macro>();
            Prefabs = new Dictionary<string, GameObject>();
            foreach(Macro macro in UtilReflection.GetAllSubclasses<Macro>()) {
                macro.Parser = this;
                Macros.TryAdd(macro.Tag, macro);
            }
            ColorDefinitions.Clear();
            foreach(OrbitConfig.ColorDefintion colorDef in config.Colors) {
                DefineColor(colorDef.name, colorDef.color);
            }
        }

        public void SetGlobal<T>(string name, T value) {
            if(Globals.TryGetValue(name, out UIValue uiValue)) {
                uiValue.SetValue(value);
                return;
            }
            Globals[name] = new DefinedUIValue<T>(null, value);
        }

        public void DefineColor(string name, Color color) {
            ColorDefinitions[name] = color;
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
#if ORBIT_BENCHMARK
            Stopwatch parseStopWatch = new();
            parseStopWatch.Start();
#endif
            
            UIRenderData renderData = new() {
                Host = host,
                ParentRenderData = parentData,
                Parser = this,
                RootParent = parent
            };

            if(parentData != null && parentData.RootParent == parent) {
                //This can happen if macros that parse with a new host are at the root, can share RootObjects in these cases to make sure these objects get destroyed when hot reloading
                renderData.RootObjects = parentData.RootObjects;
            }

            List<(string, FieldInfo)> viewComponentFields = null;
            List<(string, PropertyInfo)> viewComponentProperties = null;
            if(host != null) {
                List<(string, FieldInfo)> eventEmitterFields = null;
                foreach(FieldInfo fieldInfo in host.GetType().GetFields(HOST_FLAGS)) {
                    ViewComponentAttribute viewComponent = fieldInfo.GetCustomAttribute<ViewComponentAttribute>(true);
                    if(viewComponent != null) {
                        if(viewComponentFields == null)
                            viewComponentFields = new();
                        viewComponentFields.Add((viewComponent.ID,fieldInfo));
                    }
                    
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
                    ViewComponentAttribute viewComponent = propInfo.GetCustomAttribute<ViewComponentAttribute>(true);
                    if(viewComponent != null) {
                        if(viewComponentProperties == null)
                            viewComponentProperties = new();
                        viewComponentProperties.Add((viewComponent.ID,propInfo));
                    }
                    
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

            Stopwatch renderNodesStopwatch = new();
            renderNodesStopwatch.Start();
            foreach (XmlNode node in parentNode.ChildNodes) {
                RenderNode(node, parent, renderData);
            }
            renderNodesStopwatch.Stop();

            if(viewComponentFields != null) {
                foreach((string valueID, FieldInfo fieldInfo) in viewComponentFields) {
                    UIValue value = renderData.GetValueFromID(valueID);
                    void UpdateViewComponent() {
                        fieldInfo.SetValue(host, value.GetValue<MarkupPrefab>().FindComponent(fieldInfo.FieldType));
                    }
                    value.OnChange += UpdateViewComponent;
                    UpdateViewComponent();
                }
            }
            
            if(viewComponentProperties != null) {
                foreach((string valueID, PropertyInfo propInfo) in viewComponentProperties) {
                    UIValue value = renderData.GetValueFromID(valueID);
                    void UpdateViewComponent() {
                        propInfo.SetValue(host, value.GetValue<MarkupPrefab>().FindComponent(propInfo.PropertyType));
                    }
                    value.OnChange += UpdateViewComponent;
                    UpdateViewComponent();
                }
            }
            renderData.EmitEvent("PostParse");
#if ORBIT_BENCHMARK
            parseStopWatch.Stop();
            Debug.Log($"Parsed {host.GetType().Name} in {parseStopWatch.ElapsedMilliseconds}ms {renderNodesStopwatch.ElapsedMilliseconds}ms");
#endif
            return renderData;
        }
        
        public void RenderNode(XmlNode node, GameObject parent, UIRenderData renderData) {
            TagParameters parameters = new() {
                RenderData = renderData,
                Node = node,
                Data = reusableParameterData
            };
            parameters.Data.Clear(); //Clear out dictionary since the same one gets reused here
            foreach(XmlAttribute attribute in node.Attributes) {
                string propertyName = attribute.Name;
                string value = attribute.Value;
                switch(value[0]) {
                    case RETRIEVE_VALUE_PREFIX:
                        parameters.Data.Add(propertyName, new TagParameters.BoundData(renderData.GetValueFromID(value.Substring(1))));
                        break;
                    case RESOURCE_VALUE_PREFIX:
                        parameters.Data.Add(propertyName, new TagParameters.BoundData(value.Substring(1), true));
                        break;
                    case GLOBAL_VALUE_PREFIX:
                        if(!Globals.TryGetValue(value.Substring(1), out UIValue uiValue)) 
                            throw new Exception($"Attempted to access '{value}' but no such global exists");
                        parameters.Data.Add(propertyName, new TagParameters.BoundData(uiValue));
                        break;
                    default:
                        parameters.Data.Add(propertyName, new TagParameters.BoundData(value));
                        break;
                    
                }
            }
            parameters.Node = node;
            
            if(Macros.TryGetValue(node.Name, out Macro macro)) {
                RenderMacroNode(macro, parent, parameters);
                return;
            }
            RenderTagNode(parent, parameters);
        }

        private void RenderTagNode(GameObject parent, TagParameters parameters) {
            XmlNode node = parameters.Node;
            GameObject nodeGO = CreatePrefab(node.Name, parent);
            MarkupPrefab markupPrefab = nodeGO.GetComponent<MarkupPrefab>();
            if(markupPrefab == null)
                throw new Exception($"'OrbitPrefabs/{node.Name}' is missing it's MarkupPrefab component");
            
            reusableComponentList.Clear();
            markupPrefab.GetAllComponents(reusableComponentList);
            if(!processorPrefabCache.TryGetValue(node.Name, out List<(ComponentProcessor,int)> processors)) {
                processors = new();
                foreach(ComponentProcessor processor in ComponentProcessors) {
                    markupPrefab.AddComponentIndexes(processor, processors, reusableComponentList);
                }
                processorPrefabCache.Add(node.Name, processors);
            }
            foreach((ComponentProcessor processor, int index) in processors) {
                processor.Process(reusableComponentList[index], parameters);
            }

            if(markupPrefab.ParseChildren) {
                foreach(XmlNode childNode in node.ChildNodes)
                    RenderNode(childNode, markupPrefab.ChildrenContainer, parameters.RenderData);
            }
            
            if(parent == parameters.RenderData.RootParent) {
                parameters.RenderData.RootObjects.Add(nodeGO);
            }
        }

        private void RenderMacroNode(Macro macro, GameObject parent, TagParameters parameters) {
            macro.Execute(parent, parameters);
        }

        public virtual GameObject CreatePrefab(string name, GameObject parent) {
            if(!Prefabs.TryGetValue(name, out GameObject prefab)) {
                prefab = Resources.Load<GameObject>($"OrbitPrefabs/{name}");
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