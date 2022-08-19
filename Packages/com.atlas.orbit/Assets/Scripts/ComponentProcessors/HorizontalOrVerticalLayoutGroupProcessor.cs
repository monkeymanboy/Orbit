using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class HorizontalOrVerticalLayoutGroupProcessor : ComponentProcessor<HorizontalOrVerticalLayoutGroup> {
        public override Dictionary<string, TypeSetter<HorizontalOrVerticalLayoutGroup>> Setters => new() {
            {"Spacing", new FloatSetter<HorizontalOrVerticalLayoutGroup>((component, value) => component.spacing = value) },
            {"ChildForceExpandWidth", new BoolSetter<HorizontalOrVerticalLayoutGroup>((component, value) => component.childForceExpandWidth = value) },
            {"ChildForceExpandHeight", new BoolSetter<HorizontalOrVerticalLayoutGroup>((component, value) => component.childForceExpandHeight = value) },
            {"ChildControlWidth", new BoolSetter<HorizontalOrVerticalLayoutGroup>((component, value) => component.childControlWidth = value) },
            {"ChildControlHeight", new BoolSetter<HorizontalOrVerticalLayoutGroup>((component, value) => component.childControlHeight = value) },
            {"ChildScaleWidth", new BoolSetter<HorizontalOrVerticalLayoutGroup>((component, value) => component.childScaleWidth = value) },
            {"ChildScaleHeight", new BoolSetter<HorizontalOrVerticalLayoutGroup>((component, value) => component.childScaleHeight = value) },
        };
    }
}
