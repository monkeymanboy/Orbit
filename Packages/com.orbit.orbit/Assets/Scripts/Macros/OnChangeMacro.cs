using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ValueID")]
    [RequiresProperty("Event")]
    public class OnChangeMacro : Macro<OnChangeMacro.OnChangeMacroData> {
        public struct OnChangeMacroData {
            public string ValueID;
            public string Event;
            public UIValue ConditionValue;
        }
        public override string Tag => "ON_CHANGE";

        public override Dictionary<string, TypeSetter<OnChangeMacroData>> Setters => new() {
            {"ValueID", new StringSetter<OnChangeMacroData>((ref OnChangeMacroData data, string value) => data.ValueID = value) },
            {"Event", new StringSetter<OnChangeMacroData>((ref OnChangeMacroData data, string value) => data.Event = value) }
        };

        public override Dictionary<string, TypeSetter<OnChangeMacroData, UIValue>> ValueSetters => new() {
            {"Condition", new ObjectSetter<OnChangeMacroData, UIValue>((ref OnChangeMacroData data, UIValue value) => data.ConditionValue = value) }
        };

        public override void Execute(XmlNode node, GameObject parent, OrbitRenderData renderData, OnChangeMacroData data) {
            UIValue value = renderData.GetValueFromID(data.ValueID);

            if(data.ConditionValue == null) {
                value.OnChange += () => renderData.EmitEvent(data.Event);
            } else {
                value.OnChange += () => {
                    if(data.ConditionValue.GetValue<bool>()) 
                        renderData.EmitEvent(data.Event);
                };
            }
        }
    }
}
