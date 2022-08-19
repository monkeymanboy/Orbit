using Atlas.Orbit.Parser;
using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ID")]
    [RequiresProperty("ValueID")]
    [RequiresProperty("StartColor")]
    [RequiresProperty("EndColor")]
    public class LerpColorMacro : Macro<LerpColorMacro> {
        public override string Tag => "LERP_COLOR";

        public string ID { get; set; }
        public string ValueID { get; set; }
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public override Dictionary<string, TypeSetter<LerpColorMacro>> Setters => new Dictionary<string, TypeSetter<LerpColorMacro>>() {
            {"ID", new StringSetter<LerpColorMacro>((data, value) => data.ID = value) },
            {"ValueID", new StringSetter<LerpColorMacro>((data, value) => data.ValueID = value) },
            {"StartColor", new ColorSetter<LerpColorMacro>((data, value) => data.StartColor = value) },
            {"EndColor", new ColorSetter<LerpColorMacro>((data, value) => data.EndColor = value) }
        };

        public override void Execute(XmlNode node, GameObject parent, LerpColorMacro data) {
            UIRenderData renderData = CurrentData;
            UIValue value = renderData.GetValueFromID(ValueID);
            Color startColor = StartColor;
            Color endColor = EndColor;
            string id = ID;
            value.OnChange += () => {
                renderData.SetValue(id, Color.Lerp(startColor, endColor, value.GetValue<float>()));
            };
            renderData.SetValue(id, Color.Lerp(startColor, endColor, value.GetValue<float>()));
        }

        private float Remap(float value, Vector2 fromRange, Vector2 toRange) {
            return (value - fromRange.x) / (fromRange.y - fromRange.x) * (toRange.y - toRange.x) + toRange.x;
        }

        public override void SetToDefault() {
            ID = null;
            ValueID = null;
            StartColor = Color.white;
            EndColor = Color.white;
        }
    }
}
