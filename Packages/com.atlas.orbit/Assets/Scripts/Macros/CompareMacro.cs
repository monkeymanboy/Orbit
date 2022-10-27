using System;
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
    [RequiresProperty("FirstID")]
    [RequiresProperty("SecondID")]
    public class CompareMacro : Macro<CompareMacro> {
        public override string Tag => "COMPARE";

        public string ID { get; set; }
        public string FirstID { get; set; }
        public string SecondID { get; set; }

        public override Dictionary<string, TypeSetter<CompareMacro>> Setters => new Dictionary<string, TypeSetter<CompareMacro>>() {
            {"ID", new StringSetter<CompareMacro>((data, value) => data.ID = value) },
            {"FirstValue", new StringSetter<CompareMacro>((data, value) => data.FirstID = value) },
            {"SecondValue", new StringSetter<CompareMacro>((data, value) => data.SecondID = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, CompareMacro data) {
            UIRenderData renderData = CurrentData;
            UIValue firstValue = renderData.GetValueFromID(FirstID);
            UIValue secondValue = renderData.GetValueFromID(SecondID);
            string id = ID;
            Action UpdateValue = () => {
                renderData.SetValue(id, firstValue.GetValue().Equals(secondValue.GetValue()));
            };
            firstValue.OnChange += UpdateValue;
            secondValue.OnChange += UpdateValue;
            UpdateValue();
        }

        public override void SetToDefault() {
            ID = null;
            FirstID = null;
            SecondID = null;
        }
    }
}
