using Atlas.Orbit.Components;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Atlas.Orbit.Parser;
using System.Collections;
using Atlas.Orbit.TypeSetters;

namespace Atlas.Orbit.ComponentProcessors {

    public class OrbitListProcessor : ComponentProcessor<OrbitList> {
        public override Dictionary<string, TypeSetter<OrbitList>> Setters => new() {
            {"VisibleItems", new IntSetter<OrbitList>((component, value) => component.visibleItems = value) },
            {"Spacing", new FloatSetter<OrbitList>((component, value) => component.spacing = value) },
            {"ExtraMiddleSpace", new FloatSetter<OrbitList>((component, value) => component.extraMiddleSpace = value) },
            {"Items", new ObjectSetter<OrbitList, IList>((component, value) => component.Hosts = value) },
            {"ScrollLocked", new BoolSetter<OrbitList>((component, value) => component.ScrollLocked = value) },
            {"_Node", new ObjectSetter<OrbitList, XmlNode>((component, value) => component.ItemXml = value) },
            {"OnCellCentered", new ObjectSetter<OrbitList, UIFunction>((component, value) => component.OnCellCentered = value ) }
        };

        public override void Process(Component genericComponent, TagParameters processorParams) {
            base.Process(genericComponent, processorParams);
            (genericComponent as OrbitList).ParentData = CurrentData;
            CurrentData.AddEvent("PostParse", (genericComponent as OrbitList).Refresh);
        }
    }
}
