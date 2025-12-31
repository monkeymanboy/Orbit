using System;
using System.Collections.Generic;
using System.Xml;
using Orbit.Parser;
using Orbit.TypeSetters;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ID")]
    [RequiresProperty("FirstID")]
    [RequiresProperty("SecondID")]
    public class CompareMacro : Macro<CompareMacro.CompareMacroData> {
        public struct CompareMacroData {
            public string ID;
            public string FirstID;
            public string SecondID;
        }
        public override string Tag => "COMPARE";

        public override Dictionary<string, TypeSetter<CompareMacroData>> Setters => new() {
            {"ID", new StringSetter<CompareMacroData>((ref CompareMacroData data, string value) => data.ID = value) },
            {"FirstValue", new StringSetter<CompareMacroData>((ref CompareMacroData data, string value) => data.FirstID = value) },
            {"SecondValue", new StringSetter<CompareMacroData>((ref CompareMacroData data, string value) => data.SecondID = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, OrbitRenderData renderData, CompareMacroData data) {
            UIValue firstValue = renderData.GetValueFromID(data.FirstID);
            UIValue secondValue = renderData.GetValueFromID(data.SecondID);
            UIValue resultValue = renderData.SetValue(data.ID, firstValue.GetValue().Equals(secondValue.GetValue()));
            void OnValueChange() {
                resultValue.SetValue(firstValue.GetValue().Equals(secondValue.GetValue()));
            }
            firstValue.OnChange += OnValueChange;
            secondValue.OnChange += OnValueChange;
        }
    }
}
