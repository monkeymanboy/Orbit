using System.Collections.Generic;
using System.Xml;
using Orbit.Parser;
using Orbit.TypeSetters;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using System;
    using TypeSetters;

    [RequiresProperty("ID")]
    [RequiresProperty("Range")]
    [RequiresProperty("ValueID")]
    [RequiresProperty("ValueRange")]
    public class RemapMacro : Macro<RemapMacro.RemapMacroData> {
        public struct RemapMacroData {
            public string ID;
            public Vector2 Range;
            public string ValueID;
            public Vector2 ValueRange;
        }
        public override string Tag => "REMAP";

        public override Dictionary<string, TypeSetter<RemapMacroData>> Setters => new() {
            {"ID", new StringSetter<RemapMacroData>((ref RemapMacroData data, string value) => data.ID = value) },
            {"ValueID", new StringSetter<RemapMacroData>((ref RemapMacroData data, string value) => data.ValueID = value) },
            {"Range", new Vector2Setter<RemapMacroData>((ref RemapMacroData data, Vector2 value) => data.Range = value) },
            {"ValueRange", new Vector2Setter<RemapMacroData>((ref RemapMacroData data, Vector2 value) => data.ValueRange = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, RemapMacroData data) {
            UIValue value = renderData.GetValueFromID(data.ValueID);
            
            UIValue mappedValue = renderData.SetValue(data.ID, Remap(value.GetValue<float>(), data.ValueRange, data.Range));
            void OnValueChange() {
                mappedValue.SetValue(Remap(value.GetValue<float>(), data.ValueRange, data.Range));
            }

            value.OnChange += OnValueChange;
        }

        private float Remap(float value, Vector2 fromRange, Vector2 toRange) {
            return (value - fromRange.x) / (fromRange.y - fromRange.x) * (toRange.y - toRange.x) + toRange.x;
        }
    }
}
