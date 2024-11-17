using Atlas.Orbit.TypeSetters;
using Atlas.Orbit.Parser;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Schema;
using Atlas.Orbit.Components;

namespace Atlas.Orbit.ComponentProcessors {
    using Parser;
    using System.Xml;
    using TypeSetters;

    public abstract class ComponentProcessor {
        public abstract Type ComponentType { get; }
        protected UIRenderData CurrentData { get; set; }

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
            foreach(KeyValuePair<string, TagParameters.BoundData> pair in processorParams.Data) {
                if(CachedSetters.TryGetValue(pair.Key, out TypeSetter<T> typeSetter)) {
                    UIValue uiValue = pair.Value.boundValue;
                    if(uiValue != null) {
                        ValueChangeSetter valueChangeSetter = genericComponent.gameObject.GetComponent<ValueChangeSetter>();
                        if(valueChangeSetter == null)
                            valueChangeSetter = genericComponent.gameObject.AddComponent<ValueChangeSetter>();
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
                        continue;
                    }
                    typeSetter.SetFromString(component, pair.Value.data);
                }
            }
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
