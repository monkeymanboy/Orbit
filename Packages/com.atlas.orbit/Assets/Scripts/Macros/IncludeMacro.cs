using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("ViewType")]
    [RequiresProperty("ViewValue")]
    public class IncludeMacro : Macro<IncludeMacro> {
        public override string Tag => "INCLUDE";

        public Type ViewType { get; set; }
        public string ViewValue { get; set; }
        public bool StartsActive { get; set; }

        public override Dictionary<string, TypeSetter<IncludeMacro>> Setters => new Dictionary<string, TypeSetter<IncludeMacro>>() {
            {"ViewType", new ObjectSetter<IncludeMacro, Type>((data, value) => data.ViewType = value) },
            {"ViewValue", new StringSetter<IncludeMacro>((data, value) => data.ViewValue = value) },
            {"StartsActive", new BoolSetter<IncludeMacro>((data, value) => data.StartsActive = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, IncludeMacro data) {
            GameObject viewGO = new GameObject(ViewType.Name);
            viewGO.SetActive(false);
            RectTransform viewRect = viewGO.AddComponent<RectTransform>();
            viewRect.SetParent(parent.transform, false);
            viewRect.anchorMin = new Vector2(0, 0);
            viewRect.anchorMax = new Vector2(1, 1);
            viewRect.sizeDelta = Vector2.zero;
            CurrentData.SetValue(ViewValue, viewGO.AddComponent(ViewType));
            if(StartsActive)
                viewGO.SetActive(true);
        }

        public override void SetToDefault() {
            ViewType = null;
            ViewValue = null;
            StartsActive = true;
        }
    }
}
