using Atlas.Orbit.Components;
using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class ContentScalerProcessor : ComponentProcessor<ContentScaler> {
        public override Dictionary<string, TypeSetter<ContentScaler>> Setters => new() {
            {"ContentScale", new FloatSetter<ContentScaler>((component, value) => component.ContentScale = value) },
        };
    }
}
