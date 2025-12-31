using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using System.Linq;
    using System.Reflection;
    using TypeSetters;

    public abstract class Macro {
        public abstract string Tag { get; }
        public OrbitParser Parser { get; set; }
        public virtual bool CanHaveChildren => false;

        public abstract void Execute(GameObject parent, TagParameters parameters);

        public abstract List<XmlSchemaAttribute> GenerateSchemaAttributes();
    }
    public abstract class Macro<T> : Macro {
        private Dictionary<string, TypeSetter<T>> cachedSetters;
        public Dictionary<string, TypeSetter<T>> CachedSetters {
            get {
                if(cachedSetters == null)
                    cachedSetters = Setters;
                return cachedSetters;
            }
        }
        
        private Dictionary<string, TypeSetter<T, UIValue>> cachedValueSetters;
        public Dictionary<string, TypeSetter<T, UIValue>> CachedValueSetters {
            get {
                if(cachedValueSetters == null)
                    cachedValueSetters = ValueSetters;
                return cachedValueSetters;
            }
        }
        
        public abstract Dictionary<string, TypeSetter<T>> Setters { get; }
        public virtual Dictionary<string, TypeSetter<T, UIValue>> ValueSetters => null;

        public override void Execute(GameObject parent, TagParameters parameters) {
            T data = default;
            if(parameters.DefaultData != null) {
                foreach(KeyValuePair<string, TagParameters.BoundData> pair in parameters.DefaultData) {
                    if(parameters.Data.TryGetValue(pair.Key, out _)) continue;
                    BindValue(ref data, pair.Key, pair.Value);
                }
            }
            foreach(KeyValuePair<string, TagParameters.BoundData> pair in parameters.Data) {
                BindValue(ref data, pair.Key, pair.Value);
            }
            Execute(parameters.Node, parent, parameters.RenderData, data);
        }
        private void BindValue(ref T data, string key, TagParameters.BoundData boundData) {
            UIValue uiValue = boundData.boundValue;
            if(CachedValueSetters != null && CachedValueSetters.TryGetValue(key, out TypeSetter<T, UIValue> valueTypeSetter) && uiValue != null) {
                valueTypeSetter.Set<UIValue>(ref data, uiValue);
            }
            if(!CachedSetters.TryGetValue(key, out TypeSetter<T> typeSetter))
                return;
            if(uiValue != null) {
                typeSetter.Set(ref data, uiValue);
                return;
            }
            if(boundData.isDataResourcePath)
                typeSetter.SetFromResource(ref data, boundData.data);
            else 
                typeSetter.SetFromString(ref data, boundData.data);
        }
        public abstract void Execute(XmlNode node, GameObject parent, OrbitRenderData renderData, T data);

        public override List<XmlSchemaAttribute> GenerateSchemaAttributes() {
            List<XmlSchemaAttribute> attributes = new();
            RequiresPropertyAttribute[] requiresAttributes = GetType().GetCustomAttributes(typeof(RequiresPropertyAttribute)).Cast<RequiresPropertyAttribute>().ToArray();
            foreach(KeyValuePair<string, TypeSetter<T>> pair in CachedSetters) {
                XmlSchemaAttribute attribute = new();
                attribute.Name = pair.Key;
                if(requiresAttributes.Any(x => x.Property == pair.Key)) 
                    attribute.Use = XmlSchemaUse.Required;
                attribute.SchemaType = pair.Value.GenerateSchemaType();
                attributes.Add(attribute);
            }
            return attributes;
        }
    }
}
