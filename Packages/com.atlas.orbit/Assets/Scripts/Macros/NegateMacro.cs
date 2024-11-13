using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Atlas.Orbit.Parser;
using Atlas.Orbit.Schema.Attributes;
using Atlas.Orbit.TypeSetters;

namespace Atlas.Orbit.Macros {

    [RequiresProperty("ID")]
    [RequiresProperty("BoolValue")]
    public class NegateMacro : Macro<NegateMacro.NegateMacroData> {
        public struct NegateMacroData {
            public string ID;
            public string BoolID;
        }
        public override string Tag => "NEGATE";

        public override Dictionary<string, TypeSetter<NegateMacroData>> Setters => new() {
            {"ID", new StringSetter<NegateMacroData>((ref NegateMacroData data, string value) => data.ID = value) },
            {"BoolValue", new StringSetter<NegateMacroData>((ref NegateMacroData data, string value) => data.BoolID = value) }
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, NegateMacroData data) {
            UIValue boolValue = renderData.GetValueFromID(data.BoolID);
            boolValue.OnChange += () => {
                renderData.SetValue(data.ID, !boolValue.GetValue<bool>());
            };
            renderData.SetValue(data.ID, !boolValue.GetValue<bool>());
        }
    }
}