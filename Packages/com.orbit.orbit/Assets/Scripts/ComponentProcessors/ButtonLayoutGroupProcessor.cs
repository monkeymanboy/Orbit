using System.Collections.Generic;

namespace Orbit.ComponentProcessors {
    using Components.ButtonLayoutGroup;
    using TypeSetters;

    public class ButtonLayoutGroupProcessor : ComponentProcessor<ButtonLayoutGroup> {
        public override Dictionary<string, TypeSetter<ButtonLayoutGroup>> Setters => new() {
            {"IsVertical", new BoolSetter<ButtonLayoutGroup>((component, value) => component.IsVertical = value) }
        };
    }
}