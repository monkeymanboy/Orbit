using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ViewValue")]
    public class IncludeMacro : Macro<IncludeMacro.IncludeMacroData> {
        public struct IncludeMacroData {
            public Type ViewType;
            public string ViewValue;
            public bool? Active;
            public UIValue ActiveValue;
        }
        public override string Tag => "INCLUDE";

        public override Dictionary<string, TypeSetter<IncludeMacroData>> Setters => new() {
            {"ViewValue", new StringSetter<IncludeMacroData>((ref IncludeMacroData data, string value) => data.ViewValue = value) },
            {"ViewType", new ObjectSetter<IncludeMacroData, Type>((ref IncludeMacroData data, Type value) => data.ViewType = value) },
            {"Active", new BoolSetter<IncludeMacroData>((ref IncludeMacroData data, bool value) => data.Active = value) },
        };


        public override Dictionary<string, TypeSetter<IncludeMacroData, UIValue>> ValueSetters => new() {
            {"Active", new ObjectSetter<IncludeMacroData, UIValue>((ref IncludeMacroData data, UIValue value) => data.ActiveValue = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, OrbitRenderData renderData, IncludeMacroData data) {
            UIValue resolvedValue = null;
            if(data.ViewType == null) {
                resolvedValue = renderData.GetValueFromID(data.ViewValue);
                data.ViewType = resolvedValue.GetValueType();
            }
            GameObject viewGO = new(data.ViewType.Name);
            viewGO.SetActive(false);
            RectTransform viewRect = viewGO.AddComponent<RectTransform>();
            viewRect.SetParent(parent.transform, false);
            viewRect.anchorMin = new Vector2(0, 0);
            viewRect.anchorMax = new Vector2(1, 1);
            viewRect.sizeDelta = Vector2.zero;
            if(resolvedValue == null) 
                renderData.SetValue(data.ViewValue, viewGO.AddComponent(data.ViewType));
            else
                resolvedValue.SetValue(viewGO.AddComponent(data.ViewType));
            if(data.Active.HasValue) {
                viewGO.SetActive(data.Active.Value);
                if(data.ActiveValue != null) {
                    data.ActiveValue.OnChange += () => {
                        viewGO.SetActive(data.ActiveValue.GetValue<bool>());
                    };
                }
            } else {
                viewGO.SetActive(true);
            }
        }
    }
}
