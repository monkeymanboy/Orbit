using System.Collections.Generic;

namespace Orbit.ComponentProcessors {
    using TypeSetters;
    using UnityEngine.UI;

    public class ToggleProcessor : ComponentProcessor<Toggle> {
        public override Dictionary<string, TypeSetter<Toggle>> Setters => new() {
            {"ToggleGroup", new ObjectSetter<Toggle,ToggleGroup>((component, value) => component.group = value) }
        };
    }
}
