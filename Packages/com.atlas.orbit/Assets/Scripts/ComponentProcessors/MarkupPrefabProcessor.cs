using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using Atlas.Orbit.Components;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class MarkupPrefabProcessor : ComponentProcessor<MarkupPrefab> {
        public override Dictionary<string, TypeSetter<MarkupPrefab>> Setters => new() {
            {"ID", new StringSetter<MarkupPrefab>((component, value) => CurrentData.SetValue(value, component)) },
        };
    }
}
