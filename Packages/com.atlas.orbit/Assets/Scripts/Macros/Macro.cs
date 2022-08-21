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
        protected UIRenderData CurrentData { get; set; }
        public OrbitParser Parser { get; set; }

        public abstract void Execute(XmlNode node, GameObject parent, TagParameters parameters);

        public abstract List<XmlSchemaAttribute> GenerateSchemaAttributes();
    }
    public abstract class Macro<T> : Macro where T : Macro<T> {
        private Dictionary<string, TypeSetter<T>> cachedSetters;
        public Dictionary<string, TypeSetter<T>> CachedSetters {
            get {
                if(cachedSetters == null)
                    cachedSetters = Setters;
                return cachedSetters;
            }
        }
        public abstract Dictionary<string, TypeSetter<T>> Setters { get; }
        public abstract void SetToDefault();

        public override void Execute(XmlNode node, GameObject parent, TagParameters parameters) {
            T data = (T) this;
            CurrentData = parameters.RenderData;
            SetToDefault();
            foreach(KeyValuePair<string, string> pair in parameters.Data) {
                if(CachedSetters.TryGetValue(pair.Key, out TypeSetter<T> typeSetter)) {
                    if(pair.Value == null) {
                        typeSetter.Set(data, parameters.Values[pair.Key].GetValue());
                        continue;
                    }
                    typeSetter.SetFromString(data, pair.Value);
                }
            }
            Execute(node, parent, data);
        }
        public abstract void Execute(XmlNode node, GameObject parent, T data);

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
