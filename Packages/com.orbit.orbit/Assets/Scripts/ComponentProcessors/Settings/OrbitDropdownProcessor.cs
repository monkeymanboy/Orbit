namespace Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using System.Collections;
    using System.Collections.Generic;
    using TypeSetters;

    public class OrbitDropdownProcessor : ComponentProcessor<OrbitDropdown> {
        public override Dictionary<string, TypeSetter<OrbitDropdown>> Setters => new() {
            {"Items", new ObjectSetter<OrbitDropdown, IList>((component, value) => component.OptionsList = value) },
        };
    }
}