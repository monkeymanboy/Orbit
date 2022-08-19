using System.Collections.Generic;
using System.Xml;
using Atlas.Orbit.TypeSetters;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("Items")]
    public class ForEachMacro : Macro<ForEachMacro> {
        public override string Tag => "FOR_EACH";

        public IEnumerable<object> Items { get; set; }

        public override Dictionary<string, TypeSetter<ForEachMacro>> Setters => new Dictionary<string, TypeSetter<ForEachMacro>>() {
            {"Items", new ObjectSetter<ForEachMacro, IEnumerable<object>>((data, value) => data.Items = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, ForEachMacro data) {
            foreach(object host in data.Items) {
                Parser.Parse(node, parent, host, CurrentData);
            }
        }

        public override void SetToDefault() {
            Items = null;
        }
    }
}
