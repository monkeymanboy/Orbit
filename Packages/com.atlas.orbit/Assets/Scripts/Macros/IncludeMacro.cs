using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ViewType")]
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
            {"ViewType", new ObjectSetter<IncludeMacroData, Type>((ref IncludeMacroData data, Type value) => data.ViewType = value) },
            {"ViewValue", new StringSetter<IncludeMacroData>((ref IncludeMacroData data, string value) => data.ViewValue = value) },
            {"Active", new BoolSetter<IncludeMacroData>((ref IncludeMacroData data, bool value) => data.Active = value) },
        };


        public override Dictionary<string, TypeSetter<IncludeMacroData, UIValue>> ValueSetters => new() {
            {"Active", new ObjectSetter<IncludeMacroData, UIValue>((ref IncludeMacroData data, UIValue value) => data.ActiveValue = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, IncludeMacroData data) {
            GameObject viewGO = new GameObject(data.ViewType.Name);
            viewGO.SetActive(false);
            RectTransform viewRect = viewGO.AddComponent<RectTransform>();
            viewRect.SetParent(parent.transform, false);
            viewRect.anchorMin = new Vector2(0, 0);
            viewRect.anchorMax = new Vector2(1, 1);
            viewRect.sizeDelta = Vector2.zero;
            renderData.SetValue(data.ViewValue, viewGO.AddComponent(data.ViewType));
            if(data.Active.HasValue) {
                viewGO.SetActive(data.Active.Value);
                data.ActiveValue.OnChange += () => {
                    viewGO.SetActive(data.ActiveValue.GetValue<bool>());
                };
            }
        }
    }
}
