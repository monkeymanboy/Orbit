namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using System.Collections.Generic;
    using System.Xml;
    using TypeSetters;
    using UnityEngine;
    using UnityEngine.UI;

    [RequiresProperty("ID")]
    public class ToggleGroupMacro : Macro<ToggleGroupMacro.ToggleGroupMacroData> {
        public struct ToggleGroupMacroData {
            public string ID;
            public bool AllowSwitchOff;
        }

        public override string Tag => "TOGGLE_GROUP";
        
        public override Dictionary<string, TypeSetter<ToggleGroupMacroData>> Setters => new() {
            { "ID", new StringSetter<ToggleGroupMacroData>((ref ToggleGroupMacroData data, string value) => data.ID = value) },
            { "AllowSwitchOff", new BoolSetter<ToggleGroupMacroData>((ref ToggleGroupMacroData data, bool value) => data.AllowSwitchOff = value) }
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, ToggleGroupMacroData data) {
            ToggleGroup toggleGroup = parent.AddComponent<ToggleGroup>();
            toggleGroup.allowSwitchOff = data.AllowSwitchOff;
            renderData.SetValue(data.ID, toggleGroup);
        }
    }
}