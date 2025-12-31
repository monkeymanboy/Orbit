namespace Orbit.ComponentProcessors {
    using System.Collections.Generic;
    using Components;
    using TypeSetters;

    public class OrbitTagProcessor : ComponentProcessor<OrbitTag> {
        public override Dictionary<string, TypeSetter<OrbitTag>> Setters => new() {
            {"ID", new StringSetter<OrbitTag>((component, value) => CurrentData.SetValue(value, component)) },
        };
    }
}
