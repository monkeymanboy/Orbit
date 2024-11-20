using Orbit.TypeSetters;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Orbit.ComponentProcessors {
    using TypeSetters;

    public class ContentSizeFitterProcessor : ComponentProcessor<ContentSizeFitter> {
        public override Dictionary<string, TypeSetter<ContentSizeFitter>> Setters => new() {
            {"VerticalFit", new EnumSetter<ContentSizeFitter, ContentSizeFitter.FitMode>((component, value) => component.verticalFit = value) },
            {"HorizontalFit", new EnumSetter<ContentSizeFitter, ContentSizeFitter.FitMode>((component, value) => component.horizontalFit = value) }
        };
    }
}
