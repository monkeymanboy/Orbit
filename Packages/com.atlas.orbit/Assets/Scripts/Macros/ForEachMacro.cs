using System.Collections.Generic;
using System.Xml;
using Atlas.Orbit.TypeSetters;
using Atlas.Orbit.Schema.Attributes;
using UnityEngine;
using System.Collections;

namespace Atlas.Orbit.Macros {
    using Parser;
    using System;

    [RequiresProperty("Items")]
    public class ForEachMacro : Macro<ForEachMacro> {
        public override string Tag => "FOR_EACH";

        public IList Items { get; set; }
        public string RefreshEvent { get; set; }

        public override Dictionary<string, TypeSetter<ForEachMacro>> Setters => new() {
            {"Items", new ObjectSetter<ForEachMacro, IList>((data, value) => data.Items = value) },
            {"RefreshEvent", new StringSetter<ForEachMacro>((data,value) => data.RefreshEvent = value)}
        };

        public override void Execute(XmlNode node, GameObject parent, ForEachMacro data) {
            IList items = data.Items;
            UIRenderData renderData = CurrentData;
            List<UIRenderData> rendered = new(items.Count);
            for(int i=0;i<items.Count;i++) {
                rendered.Add(Parser.Parse(node, parent, items[i], CurrentData));
            }

            if(RefreshEvent != null) {
                Action refreshAction = () => {
                    for(int i=0;i<items.Count;i++) {
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
                };
                renderData.AddEvent(RefreshEvent, refreshAction);
            }
        }

        public override void SetToDefault() {
            Items = null;
            RefreshEvent = null;
        }
    }
}
