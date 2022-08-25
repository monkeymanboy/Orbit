using System.Collections.Generic;
using System.Xml;
using Atlas.Orbit.TypeSetters;
using Atlas.Orbit.Schema.Attributes;
using UnityEngine;
using System.Collections;

namespace Atlas.Orbit.Macros {

    [RequiresProperty("Items")]
    public class ForEachMacro : Macro<ForEachMacro> {
        public override string Tag => "FOR_EACH";

        public IList Items { get; set; }

        public override Dictionary<string, TypeSetter<ForEachMacro>> Setters => new() {
            {"Items", new ObjectSetter<ForEachMacro, IList>((data, value) => data.Items = value) },
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
