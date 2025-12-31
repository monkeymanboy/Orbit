using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

namespace Orbit.ComponentProcessors {
    using Components;
    using Parser;
    using TypeSetters;

    public abstract class ComponentProcessor {
        public abstract Type ComponentType { get; }
        protected OrbitRenderData CurrentData { get; set; }

        public abstract void Process(Component genericComponent, TagParameters processorParams);

        public abstract List<XmlSchemaAttribute> GenerateSchemaAttributes();
    }
    public abstract class ComponentProcessor<T> : ComponentProcessor where T : Component {
        public override Type ComponentType => typeof(T);

        private Dictionary<string, TypeSetter<T>> cachedSetters;
        public Dictionary<string ,TypeSetter<T>> CachedSetters {
            get {
                if(cachedSetters == null)
                    cachedSetters = Setters;
                return cachedSetters;
            }
        }
        public abstract Dictionary<string, TypeSetter<T>> Setters { get; }

        public override void Process(Component genericComponent, TagParameters processorParams) {
            if(!(genericComponent is T component))
                return;
            CurrentData = processorParams.RenderData;
            SetNode(component, processorParams.Node);
            if(processorParams.DefaultData != null) {
                foreach(KeyValuePair<string, TagParameters.BoundData> pair in processorParams.DefaultData) {
                    if(processorParams.Data.TryGetValue(pair.Key, out _)) continue;
                    BindValue(component, pair.Key, pair.Value);
                }
            }
            foreach(KeyValuePair<string, TagParameters.BoundData> pair in processorParams.Data) {
                BindValue(component, pair.Key, pair.Value);
            }
        }

        private void BindValue(T component, string key, TagParameters.BoundData boundData) {
            if(!CachedSetters.TryGetValue(key, out TypeSetter<T> typeSetter))
                return;
            UIValue uiValue = boundData.boundValue;
            if(uiValue != null) {
                ValueChangeSetter valueChangeSetter = component.gameObject.GetComponent<ValueChangeSetter>();
                if(valueChangeSetter == null)
                    valueChangeSetter = component.gameObject.AddComponent<ValueChangeSetter>();
                Action setValue = () => {
                    if(valueChangeSetter == null) { //If the object gets destroyed while disabled OnDestroy is not called so we check for that here
                        valueChangeSetter.NotifyDestroyed();
                        return;
                    }
                    if(uiValue.HasValue) typeSetter.Set(component, uiValue);
                };
                uiValue.OnChange += setValue;
                valueChangeSetter.OnObjectDestroyed += () => {
                    uiValue.OnChange -= setValue;
                };
                setValue();
                return;
            }
            if(boundData.isDataResourcePath)
                typeSetter.SetFromResource(component, boundData.data);
            else 
                typeSetter.SetFromString(component, boundData.data);
        }
        
        public virtual void SetNode(T data, XmlNode node) { }
        
        public override List<XmlSchemaAttribute> GenerateSchemaAttributes() {
            List<XmlSchemaAttribute> attributes = new();
            foreach(KeyValuePair<string, TypeSetter<T>> pair in CachedSetters) {
                XmlSchemaAttribute attribute = new();
                attribute.Name = pair.Key;
                attribute.SchemaType = pair.Value.GenerateSchemaType();
                attributes.Add(attribute);
            }
            return attributes;
        }
    }
}
