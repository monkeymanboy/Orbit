using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ID")]
    [RequiresProperty("BoolValue")]
    [RequiresProperty("TrueValue")]
    [RequiresProperty("FalseValue")]
    public class BindBoolMacro : Macro<BindBoolMacro> {
        public override string Tag => "BIND_BOOL";

        public string ID { get; set; }
        public string BoolID { get; set; }
        public string TrueID { get; set; }
        public string FalseID { get; set; }

        public override Dictionary<string, TypeSetter<BindBoolMacro>> Setters => new Dictionary<string, TypeSetter<BindBoolMacro>>() {
            {"ID", new StringSetter<BindBoolMacro>((data, value) => data.ID = value) },
            {"BoolValue", new StringSetter<BindBoolMacro>((data, value) => data.BoolID = value) },
            {"TrueValue", new StringSetter<BindBoolMacro>((data, value) => data.TrueID = value) },
            {"FalseValue", new StringSetter<BindBoolMacro>((data, value) => data.FalseID = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, BindBoolMacro data) {
            UIRenderData renderData = CurrentData;
            UIValue boolValue = renderData.GetValueFromID(BoolID);
            boolValue.OnChange += () => {
                renderData.SetValue(ID, renderData.GetValueFromID(boolValue.GetValue<bool>() ? TrueID : FalseID).GetValue());
            };
            renderData.SetValue(ID, renderData.GetValueFromID(boolValue.GetValue<bool>() ? TrueID : FalseID).GetValue());
        }

        public override void SetToDefault() {
            ID = null;
            BoolID = null;
            TrueID = null;
            FalseID = null;
        }
    }
}
