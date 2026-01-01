namespace Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using System.Collections.Generic;
    using TypeSetters;

    public class OrbitLabelWidthAdjusterProcessor : ComponentProcessor<OrbitLabelWidthAdjuster> {
        public override Dictionary<string, TypeSetter<OrbitLabelWidthAdjuster>> Setters => new() {
            {"LabelWidth", new FloatSetter<OrbitLabelWidthAdjuster>((component, value) => component.LabelWidth = value) },
        };
    }
}