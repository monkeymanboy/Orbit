using Orbit.TypeSetters;
using Orbit.Parser;
using System.Collections.Generic;
using Orbit.Components.Settings;
using UnityEngine;

namespace Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using Parser;
    using TypeSetters;

    public class SettingComponentProcessor : ComponentProcessor<SettingComponent> {
        public override Dictionary<string, TypeSetter<SettingComponent>> Setters => new() {
            {"BoundValue", new StringSetter<SettingComponent>(SetBoundValue) },
            {"ValueChangedEvent", new StringSetter<SettingComponent>(SetChangedEvent) },
            {"NotifyValueChanged", new BoolSetter<SettingComponent>((component, value) => component.NotifyValueChanged = value) },
        };

        public override void Process(Component genericComponent, TagParameters processorParams) {
            base.Process(genericComponent, processorParams);
            CurrentData.AddEvent("PostParse", (genericComponent as SettingComponent).PostParse);
        }

        private void SetBoundValue(SettingComponent setting, string valueID) {
            setting.UIValue = CurrentData.GetValueFromID(valueID);
        }

        private void SetChangedEvent(SettingComponent setting, string events) {
            UIRenderData data = CurrentData;
            setting.OnChangeEvent = () => {
                data.EmitEvent(events);
            };
        }
    }
}
