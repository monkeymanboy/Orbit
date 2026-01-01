namespace Orbit.ComponentProcessors.Settings {
    using Components.Settings;
    using Parser;
    using System.Collections.Generic;
    using TypeSetters;

    public class OrbitInputFieldProcessor : ComponentProcessor<OrbitInputField> {
        public override Dictionary<string, TypeSetter<OrbitInputField>> Setters => new() {
            {"EndEditEvent", new StringSetter<OrbitInputField>(SetEndEditEvent) }
        };

        private void SetEndEditEvent(OrbitInputField inputField, string events) {
            OrbitRenderData data = CurrentData;
            inputField.OnEndEditEvent = () => {
                data.EmitEvent(events);
            };
        }
    }
}