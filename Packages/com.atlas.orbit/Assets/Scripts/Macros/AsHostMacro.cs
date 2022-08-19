using System.Collections.Generic;
using System.Xml;
using Atlas.Orbit.TypeSetters;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("Host")]
    public class AsHostMacro : Macro<AsHostMacro> {
        public override string Tag => "AS_HOST";

        public object Host { get; set; }

        public override Dictionary<string, TypeSetter<AsHostMacro>> Setters => new Dictionary<string, TypeSetter<AsHostMacro>>() {
            {"Host", new ObjectSetter<AsHostMacro, object>((data, value) => data.Host = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, AsHostMacro data) {
            Parser.Parse(node, parent, Host, CurrentData);
        }

        public override void SetToDefault() {
            Host = null;
        }
    }
}
