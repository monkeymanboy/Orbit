using Orbit.Components;
using Orbit.TypeSetters;
using System.Collections.Generic;

namespace Orbit.ComponentProcessors {
    using Components;
    using TypeSetters;

    public class ContentScalerProcessor : ComponentProcessor<ContentScaler> {
        public override Dictionary<string, TypeSetter<ContentScaler>> Setters => new() {
            {"ContentScale", new FloatSetter<ContentScaler>((component, value) => component.ContentScale = value) },
        };
    }
}
