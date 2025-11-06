using System.Collections.Generic;
using System.Xml;
using Orbit.TypeSetters;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("Host")]
    public class AsHostMacro : Macro<AsHostMacro.AsHostMacroData> {
        public struct AsHostMacroData {
            public object Host;
            public UIValue HostValue;
        }
        public override string Tag => "AS_HOST";
        public override bool CanHaveChildren => true;

        public override Dictionary<string, TypeSetter<AsHostMacroData>> Setters => new() {
            {"Host", new ObjectSetter<AsHostMacroData, object>((ref AsHostMacroData data, object value) => data.Host = value) },
        };

        public override Dictionary<string, TypeSetter<AsHostMacroData, UIValue>> ValueSetters => new() { 
            {"Host", new ObjectSetter<AsHostMacroData, UIValue>((ref AsHostMacroData data, UIValue value) => data.HostValue = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, AsHostMacroData data) {
            UIRenderData newRenderData = Parser.Parse(node, parent, data.Host, renderData);
            UIValue hostValue = data.HostValue;
            if(hostValue != null) {
                hostValue.OnChange += () => newRenderData.Host = hostValue.GetValue();
            }
        }
    }
}
