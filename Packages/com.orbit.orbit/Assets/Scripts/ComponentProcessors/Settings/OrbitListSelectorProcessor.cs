namespace Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using System.Collections;
    using System.Collections.Generic;
    using TypeSetters;

    public class OrbitListSelectorProcessor : ComponentProcessor<OrbitListSelector> {
        public override Dictionary<string, TypeSetter<OrbitListSelector>> Setters => new() {
            {"Items", new ObjectSetter<OrbitListSelector, IList>((component, value) => component.OptionsList = value) },
        };
    }
}