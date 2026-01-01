using Orbit.TypeSetters;
using System.Collections.Generic;
using Orbit.Components.Settings;
using Orbit.Parser;

namespace Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using Parser;
    using TypeSetters;

    public class OrbitSliderProcessor : ComponentProcessor<OrbitSlider> {
        public override Dictionary<string, TypeSetter<OrbitSlider>> Setters => new() {
            {"MaxValue", new FloatSetter<OrbitSlider>((component, value) => component.MaxValue = value ) },
            {"MinValue", new FloatSetter<OrbitSlider>((component, value) => component.MinValue = value ) },
            {"Increments", new IntSetter<OrbitSlider>((component, value) => component.Increments = value ) },
            {"Formatter", new ObjectSetter<OrbitSlider, UIFunction>((component, value) => component.Formatter = value ) },
            {"ImmediateUpdate", new BoolSetter<OrbitSlider>((component, value) => component.ImmediateUpdate = value ) }
        };
    }
}
