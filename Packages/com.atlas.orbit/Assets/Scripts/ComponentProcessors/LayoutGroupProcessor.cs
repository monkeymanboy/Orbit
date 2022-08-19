using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class LayoutGroupProcessor : ComponentProcessor<LayoutGroup> {
        public override Dictionary<string, TypeSetter<LayoutGroup>> Setters => new() {
            {"ChildAlign", new EnumSetter<LayoutGroup, TextAnchor>((component, value) => component.childAlignment = value) },
            {"PadTop", new IntSetter<LayoutGroup>((component, value) => component.padding = new RectOffset(component.padding.left, component.padding.right, value, component.padding.bottom)) },
            {"PadBottom", new IntSetter<LayoutGroup>((component, value) => component.padding = new RectOffset(component.padding.left, component.padding.right, component.padding.top, value)) },
            {"PadLeft", new IntSetter<LayoutGroup>((component, value) => component.padding = new RectOffset(value, component.padding.right, component.padding.top, component.padding.bottom)) },
            {"PadRight", new IntSetter<LayoutGroup>((component, value) => component.padding = new RectOffset(component.padding.left, value, component.padding.top, component.padding.bottom)) },
            {"PadAll", new IntSetter<LayoutGroup>((component, value) => component.padding = new RectOffset(value, value, value, value)) },
        };
    }
}
