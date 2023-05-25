namespace Atlas.Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using System.Collections;
    using System.Collections.Generic;
    using TypeSetters;

    public class DropdownSettingProcessor : ComponentProcessor<DropdownSetting> {
        public override Dictionary<string, TypeSetter<DropdownSetting>> Setters => new() {
            {"Items", new ObjectSetter<DropdownSetting, IList>((component, value) => component.OptionsList = value) },
        };
    }
}