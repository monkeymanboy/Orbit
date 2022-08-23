using Atlas.Orbit.Components;
using Atlas.Orbit.Parser;
using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Atlas.Orbit.ComponentProcessors {
    using Parser;
    using TypeSetters;

    public class ScrollingListProcessor : ComponentProcessor<ScrollingList> {
        public override Dictionary<string, TypeSetter<ScrollingList>> Setters => new() {
            {"CellHeight", new FloatSetter<ScrollingList>((component, value) => component.CellHeight = value) },
            {"CellSpacing", new FloatSetter<ScrollingList>((component, value) => component.CellSpacing = value) },
            {"Items", new ObjectSetter<ScrollingList, List<object>>((component, value) => component.Hosts = value) },
            {"RefreshEvent", new StringSetter<ScrollingList>((component, value) => CurrentData.AddEvent(value, component.Refresh)) },
            {"_Node", new ObjectSetter<ScrollingList, XmlNode>((component, value) => component.ItemXml = value) },
        };

        public override void Process(Component genericComponent, TagParameters processorParams) {
            base.Process(genericComponent, processorParams);
            (genericComponent as ScrollingList).ParentData = CurrentData;
            CurrentData.AddEvent("PostParse", (genericComponent as ScrollingList).Refresh);
        }
    }
}
