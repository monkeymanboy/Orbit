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
            foreach(KeyValuePair<string, TagParameters.BoundData> pair in parameters.Data) {
                UIValue uiValue = pair.Value.boundValue;
                if(CachedValueSetters != null && CachedValueSetters.TryGetValue(pair.Key, out TypeSetter<T, UIValue> valueTypeSetter) && uiValue != null) {
                    valueTypeSetter.Set<UIValue>(ref data, uiValue);
                }
                if(CachedSetters.TryGetValue(pair.Key, out TypeSetter<T> typeSetter)) {
                    if(uiValue != null) {
                        typeSetter.Set(ref data, uiValue);
                        continue;
                    }
                    if(pair.Value.isDataResourcePath)
                        typeSetter.SetFromResource(ref data, pair.Value.data);
                    else 
                        typeSetter.SetFromString(ref data, pair.Value.data);
                }
            }
            Execute(parameters.Node, parent, parameters.RenderData, data);
        }
        public abstract void Execute(XmlNode node, GameObject parent, UIRenderData renderData, T data);

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
