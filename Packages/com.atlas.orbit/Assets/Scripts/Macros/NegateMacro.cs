using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Atlas.Orbit.Parser;
using Atlas.Orbit.Schema.Attributes;
using Atlas.Orbit.TypeSetters;

namespace Atlas.Orbit.Macros {

    [RequiresProperty("ID")]
    [RequiresProperty("BoolValue")]
    public class NegateMacro : Macro<NegateMacro> {
        public override string Tag => "NEGATE";

        public string ID { get; set; }
        public string BoolID { get; set; }

        public override Dictionary<string, TypeSetter<NegateMacro>> Setters => new() {
            {"ID", new StringSetter<NegateMacro>((data, value) => data.ID = value) },
            {"BoolValue", new StringSetter<NegateMacro>((data, value) => data.BoolID = value) }
        };

        public override void Execute(XmlNode node, GameObject parent, NegateMacro data) {
            UIRenderData renderData = CurrentData;
            UIValue boolValue = renderData.GetValueFromID(BoolID);
            string id = ID;
            boolValue.OnChange += () => {
                renderData.SetValue(id, !boolValue.GetValue<bool>());
            };
            renderData.SetValue(id, !boolValue.GetValue<bool>());
        }

        public override void SetToDefault() {
            ID = null;
            BoolID = null;
        }
    }
}