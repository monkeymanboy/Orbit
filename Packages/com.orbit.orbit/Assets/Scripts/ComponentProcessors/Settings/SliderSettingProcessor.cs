﻿using Orbit.TypeSetters;
using System.Collections.Generic;
using Orbit.Components.Settings;
using Orbit.Parser;

namespace Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using Parser;
    using TypeSetters;

    public class SliderSettingProcessor : ComponentProcessor<SliderSetting> {
        public override Dictionary<string, TypeSetter<SliderSetting>> Setters => new() {
            {"MaxValue", new FloatSetter<SliderSetting>((component, value) => component.MaxValue = value ) },
            {"MinValue", new FloatSetter<SliderSetting>((component, value) => component.MinValue = value ) },
            {"Increments", new IntSetter<SliderSetting>((component, value) => component.Increments = value ) },
            {"Formatter", new ObjectSetter<SliderSetting, UIFunction>((component, value) => component.Formatter = value ) },
            {"ImmediateUpdate", new BoolSetter<SliderSetting>((component, value) => component.ImmediateUpdate = value ) }
        };
    }
}
