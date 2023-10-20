using System.Collections.Generic;
using System.Xml;
using Atlas.Orbit.TypeSetters;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("Host")]
    public class AsHostMacro : Macro<AsHostMacro> {
        public override string Tag => "AS_HOST";

        public object Host { get; set; }
        public UIValue HostValue { get; set; }

        public override Dictionary<string, TypeSetter<AsHostMacro>> Setters => new() {
            {"Host", new ObjectSetter<AsHostMacro, object>((data, value) => data.Host = value) },
        };

        public override Dictionary<string, TypeSetter<AsHostMacro, UIValue>> ValueSetters => new() { 
            {"Host", new ObjectSetter<AsHostMacro, UIValue>((data, value) => data.HostValue = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, AsHostMacro data) {
            UIRenderData renderData = Parser.Parse(node, parent, Host, CurrentData);
            UIValue hostValue = HostValue;
            if(hostValue != null) {
                hostValue.OnChange += () => renderData.Host = hostValue.GetValue();
            }
        }

        public override void SetToDefault() {
            Host = null;
            HostValue = null;
        }
    }
}
