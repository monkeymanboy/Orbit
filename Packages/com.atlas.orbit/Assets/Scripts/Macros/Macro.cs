using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using System.Linq;
    using System.Reflection;
    using TypeSetters;

    public abstract class Macro {
        public abstract string Tag { get; }
        public OrbitParser Parser { get; set; }

        public abstract void Execute(XmlNode node, GameObject parent, TagParameters parameters);

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

        public override void Execute(XmlNode node, GameObject parent, TagParameters parameters) {
            T data = default;
            foreach(KeyValuePair<string, string> pair in parameters.Data) {
                if(CachedValueSetters != null && CachedValueSetters.TryGetValue(pair.Key, out TypeSetter<T, UIValue> valueTypeSetter)
                   && parameters.Values.TryGetValue(pair.Key, out UIValue uiValue)) {
                    valueTypeSetter.Set(ref data, uiValue);
                }
                if(CachedSetters.TryGetValue(pair.Key, out TypeSetter<T> typeSetter)) {
                    
                    if(pair.Value == null) {
                        typeSetter.Set(ref data, parameters.Values[pair.Key].GetValue());
                        continue;
                    }
                    typeSetter.SetFromString(ref data, pair.Value);
                }
            }
            Execute(node, parent, parameters.RenderData, data);
        }
        public abstract void Execute(XmlNode node, GameObject parent, UIRenderData renderData, T data);

        public override List<XmlSchemaAttribute> GenerateSchemaAttributes() {
            List<XmlSchemaAttribute> attributes = new List<XmlSchemaAttribute>();
            RequiresPropertyAttribute[] requiresAttributes = GetType().GetCustomAttributes(typeof(RequiresPropertyAttribute)).Cast<RequiresPropertyAttribute>().ToArray();
            foreach(KeyValuePair<string, TypeSetter<T>> pair in CachedSetters) {
                XmlSchemaAttribute attribute = new XmlSchemaAttribute();
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
