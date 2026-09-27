namespace Orbit.ComponentProcessors.Settings {
    using Parser;
    using System.Collections.Generic;
    using TMPro;
    using TypeSetters;

    public class TMP_InputFieldProcessor : ComponentProcessor<TMP_InputField> {
        public override Dictionary<string, TypeSetter<TMP_InputField>> Setters => new() {
            {"InputContentType", new EnumSetter<TMP_InputField, TMP_InputField.ContentType>(((component, value) => component.contentType = value)) },
            {"SubmitEvent", new StringSetter<TMP_InputField>(ApplySubmitEvent) },
            {"EndEditEvent", new StringSetter<TMP_InputField>(ApplyEndEditEvent) }
        };

        private void ApplySubmitEvent(TMP_InputField inputField, string events) {
            OrbitRenderData data = CurrentData;
            inputField.onSubmit.AddListener((_) => {
                data.EmitEvent(events);
            });
        }

        private void ApplyEndEditEvent(TMP_InputField inputField, string events) {
            OrbitRenderData data = CurrentData;
            inputField.onEndEdit.AddListener((_) => {
                data.EmitEvent(events);
            });
        }
    }
}