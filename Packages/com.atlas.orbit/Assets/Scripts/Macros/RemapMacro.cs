using System.Collections.Generic;
using System.Xml;
using Atlas.Orbit.Parser;
using Atlas.Orbit.TypeSetters;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ID")]
    [RequiresProperty("Range")]
    [RequiresProperty("ValueID")]
    [RequiresProperty("ValueRange")]
    public class RemapMacro : Macro<RemapMacro> {
        public override string Tag => "REMAP";

        public string ID { get; set; }
        public Vector2 Range { get; set; }
        public string ValueID { get; set; }
        public Vector2 ValueRange { get; set; }

        public override Dictionary<string, TypeSetter<RemapMacro>> Setters => new Dictionary<string, TypeSetter<RemapMacro>>() {
            {"ID", new StringSetter<RemapMacro>((data, value) => data.ID = value) },
            {"ValueID", new StringSetter<RemapMacro>((data, value) => data.ValueID = value) },
            {"Range", new Vector2Setter<RemapMacro>((data, value) => data.Range = value) },
            {"ValueRange", new Vector2Setter<RemapMacro>((data, value) => data.ValueRange = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, RemapMacro data) {
            UIRenderData renderData = CurrentData;
            UIValue value = renderData.GetValueFromID(ValueID);
            Vector2 valueRange = ValueRange;
            Vector2 range = Range;
            string id = ID;
            value.OnChange += () => {
                renderData.SetValue(id, Remap(value.GetValue<float>(), valueRange, range));
            };
            renderData.SetValue(id, Remap(value.GetValue<float>(), valueRange, range));
        }

        private float Remap(float value, Vector2 fromRange, Vector2 toRange) {
            return (value - fromRange.x) / (fromRange.y - fromRange.x) * (toRange.y - toRange.x) + toRange.x;
        }

        public override void SetToDefault() {
            ID = null;
            ValueID = null;
            Range = Vector2.zero;
            ValueRange = Vector2.zero;
        }
    }
}
