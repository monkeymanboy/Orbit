using Orbit.Components;
using Orbit.Parser;
using Orbit.TypeSetters;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Collections;

namespace Orbit.ComponentProcessors {
    using Components;
    using Parser;
    using TypeSetters;

    public class ScrollingListProcessor : ComponentProcessor<ScrollingList> {
        public override Dictionary<string, TypeSetter<ScrollingList>> Setters => new() {
            {"CellSize", new FloatSetter<ScrollingList>((component, value) => component.CellSize = value) },
            {"CellSpacing", new FloatSetter<ScrollingList>((component, value) => component.CellSpacing = value) },
            {"ListDirection", new EnumSetter<ScrollingList, ScrollingList.ScrollDirection>((component, value) => component.Direction = value) },
            {"Items", new ObjectSetter<ScrollingList, IList>((component, value) => component.Hosts = value) },
            {"RefreshEvent", new StringSetter<ScrollingList>((component, value) => CurrentData.AddEvent(value, component.Refresh)) }
        };

        public override void SetNode(ScrollingList component, XmlNode node) {
            component.ItemXml = node;
        }

        public override void Process(Component genericComponent, TagParameters processorParams) {
            base.Process(genericComponent, processorParams);
            (genericComponent as ScrollingList).ParentData = CurrentData;
            CurrentData.AddEvent("PostParse", (genericComponent as ScrollingList).Refresh);
        }
    }
}
