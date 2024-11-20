using Orbit.TypeSetters;
using System.Collections.Generic;
using Orbit.Components;

namespace Orbit.ComponentProcessors {
    using Components;
    using TypeSetters;

    public class MarkupPrefabProcessor : ComponentProcessor<MarkupPrefab> {
        public override Dictionary<string, TypeSetter<MarkupPrefab>> Setters => new() {
            {"ID", new StringSetter<MarkupPrefab>((component, value) => CurrentData.SetValue(value, component)) },
        };
    }
}
