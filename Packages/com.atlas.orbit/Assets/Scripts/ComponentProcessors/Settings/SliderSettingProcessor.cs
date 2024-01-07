using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using Atlas.Orbit.Components.Settings;
using Atlas.Orbit.Parser;

namespace Atlas.Orbit.ComponentProcessors.Settings {
    using TypeSetters;

    public class SliderSettingProcessor : ComponentProcessor<SliderSetting> {
        public override Dictionary<string, TypeSetter<SliderSetting>> Setters => new() {
            {"MaxValue", new FloatSetter<SliderSetting>((component, value) => component.MaxValue = value ) },
            {"MinValue", new FloatSetter<SliderSetting>((component, value) => component.MinValue = value ) },
            {"Increments", new IntSetter<SliderSetting>((component, value) => component.Increments = value ) },
            {"UseInt", new BoolSetter<SliderSetting>((component, value) => component.UseInt = value ) },
            {"Formatter", new ObjectSetter<SliderSetting, UIFunction>((component, value) => component.Formatter = value ) },
            {"ImmediateUpdate", new BoolSetter<SliderSetting>((component, value) => component.ImmediateUpdate = value ) },
            {"WholeNumbers", new BoolSetter<SliderSetting>((component, value) => component.WholeNumbers = value ) }
        };
    }
}
