using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Atlas.Orbit.ComponentProcessors {
    public class LayoutElementProcessor : ComponentProcessor<LayoutElement> {
        public override Dictionary<string, TypeSetter<LayoutElement>> Setters => new() {
            {"IgnoreLayout", new BoolSetter<LayoutElement>((component, value) => component.ignoreLayout = value) },
            {"LayoutPriority", new IntSetter<LayoutElement>((component, value) => component.layoutPriority = value) },
            {"PreferredWidth", new FloatSetter<LayoutElement>((component, value) => component.preferredWidth = value) },
            {"PreferredHeight", new FloatSetter<LayoutElement>((component, value) => component.preferredHeight = value) },
            {"MinWidth", new FloatSetter<LayoutElement>((component, value) => component.minWidth = value) },
            {"MinHeight", new FloatSetter<LayoutElement>((component, value) => component.minHeight = value) },
            {"FlexibleWidth", new FloatSetter<LayoutElement>((component, value) => component.flexibleWidth = value) },
            {"FlexibleHeight", new FloatSetter<LayoutElement>((component, value) => component.flexibleHeight = value) },
        };
    }
}
