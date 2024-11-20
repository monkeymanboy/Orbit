using System.Collections.Generic;
using System.Xml;
using Orbit.TypeSetters;
using Orbit.Schema.Attributes;
using UnityEngine;
using System.Collections;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using System;
    using TypeSetters;

    [RequiresProperty("Items")]
    public class ForEachMacro : Macro<ForEachMacro.ForEachMacroData> {
        public struct ForEachMacroData {
            public UIValue ItemsValue;
            public IList Items;
            public string RefreshEvent;
        }
        public override string Tag => "FOR_EACH";

        public override Dictionary<string, TypeSetter<ForEachMacroData>> Setters => new() {
            {"Items", new ObjectSetter<ForEachMacroData, IList>((ref ForEachMacroData data, IList value) => data.Items = value) },
            {"RefreshEvent", new StringSetter<ForEachMacroData>((ref ForEachMacroData data, string value) => data.RefreshEvent = value)}
        };

        public override Dictionary<string, TypeSetter<ForEachMacroData, UIValue>> ValueSetters => new() {
            {"Items", new ObjectSetter<ForEachMacroData, UIValue>((ref ForEachMacroData data, UIValue value) => data.ItemsValue = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, ForEachMacroData data) {
            UIValue uiValue = data.ItemsValue;
            IList items = data.Items;
            List<UIRenderData> rendered = new(items.Count);
            for(int i=0;i<items.Count;i++) {
                rendered.Add(Parser.Parse(node, parent, items[i], renderData));
            }

            void RefreshAction() {
                for(int i = 0;i < items.Count;i++) {
                    if(i < rendered.Count) {
                        rendered[i].EnableRootObjects();
                        rendered[i].Host = items[i];
                    } else {
                        rendered.Add(Parser.Parse(node, parent, items[i], renderData));
                    }
                }

                for(int i = items.Count;i < rendered.Count;i++) {
                    rendered[i].DisableRootObjects();
                }
            }

            uiValue.OnChange += () => {
                items = uiValue.GetValue<IList>();
                RefreshAction();
            };

            if(data.RefreshEvent != null) {
                renderData.AddEvent(data.RefreshEvent, RefreshAction);
            }
        }
    }
}
