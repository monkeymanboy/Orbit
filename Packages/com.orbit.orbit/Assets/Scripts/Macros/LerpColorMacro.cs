using Orbit.Parser;
using Orbit.TypeSetters;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ID")]
    [RequiresProperty("ValueID")]
    [RequiresProperty("StartColor")]
    [RequiresProperty("EndColor")]
    public class LerpColorMacro : Macro<LerpColorMacro.LerpColorMacroData> {
        public struct LerpColorMacroData {
            public string ID;
            public string ValueID;
            public Color StartColor;
            public Color EndColor;
        }
        public override string Tag => "LERP_COLOR";

        public override Dictionary<string, TypeSetter<LerpColorMacroData>> Setters => new() {
            {"ID", new StringSetter<LerpColorMacroData>((ref LerpColorMacroData data, string value) => data.ID = value) },
            {"ValueID", new StringSetter<LerpColorMacroData>((ref LerpColorMacroData data, string value) => data.ValueID = value) },
            {"StartColor", new ColorSetter<LerpColorMacroData>((ref LerpColorMacroData data, Color value) => data.StartColor = value) },
            {"EndColor", new ColorSetter<LerpColorMacroData>((ref LerpColorMacroData data, Color value) => data.EndColor = value) }
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, LerpColorMacroData data) {
            UIValue value = renderData.GetValueFromID(data.ValueID);

            UIValue colorValue = renderData.SetValue(data.ID, Color.Lerp(data.StartColor, data.EndColor, value.GetValue<float>()));
            void OnValueChange() {
                colorValue.SetValue(Color.Lerp(data.StartColor, data.EndColor, value.GetValue<float>()));
            }
            value.OnChange += OnValueChange;
        }

        private float Remap(float value, Vector2 fromRange, Vector2 toRange) {
            return (value - fromRange.x) / (fromRange.y - fromRange.x) * (toRange.y - toRange.x) + toRange.x;
        }
    }
}
