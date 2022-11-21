namespace Atlas.Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using Parser;
    using System.Collections.Generic;
    using TypeSetters;

    public class InputSettingProcessor : ComponentProcessor<InputSetting> {
        public override Dictionary<string, TypeSetter<InputSetting>> Setters => new() {
            {"EndEditEvent", new StringSetter<InputSetting>(SetEndEditEvent) }
        };

        private void SetEndEditEvent(InputSetting setting, string events) {
            UIRenderData data = CurrentData;
            setting.OnEndEditEvent = () => {
                data.EmitEvent(events);
            };
        }
    }
}