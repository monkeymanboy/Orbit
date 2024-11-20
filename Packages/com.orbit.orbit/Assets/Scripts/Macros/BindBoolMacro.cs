using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ID")]
    [RequiresProperty("BoolValue")]
    [RequiresProperty("TrueValue")]
    [RequiresProperty("FalseValue")]
    public class BindBoolMacro : Macro<BindBoolMacro.BindBoolMacroData> {
        public struct BindBoolMacroData {
            public string ID;
            public string BoolID;
            public string TrueID;
            public string FalseID;
        }
        public override string Tag => "BIND_BOOL";

        public override Dictionary<string, TypeSetter<BindBoolMacroData>> Setters => new() {
            {"ID", new StringSetter<BindBoolMacroData>((ref BindBoolMacroData data, string value) => data.ID = value) },
            {"BoolValue", new StringSetter<BindBoolMacroData>((ref BindBoolMacroData data, string value) => data.BoolID = value) },
            {"TrueValue", new StringSetter<BindBoolMacroData>((ref BindBoolMacroData data, string value) => data.TrueID = value) },
            {"FalseValue", new StringSetter<BindBoolMacroData>((ref BindBoolMacroData data, string value) => data.FalseID = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, BindBoolMacroData data) {
            UIValue boolValue = renderData.GetValueFromID(data.BoolID);
            boolValue.OnChange += () => {
                renderData.SetValue(data.ID, renderData.GetValueFromID(boolValue.GetValue<bool>() ? data.TrueID : data.FalseID).GetValue());
            };
            renderData.SetValue(data.ID, renderData.GetValueFromID(boolValue.GetValue<bool>() ? data.TrueID : data.FalseID).GetValue());
        }
    }
}
