namespace Orbit.ComponentProcessors.Settings {
    using System.Collections.Generic;
    using TMPro;
    using TypeSetters;

    public class TMP_InputFieldProcessor : ComponentProcessor<TMP_InputField> {
        public override Dictionary<string, TypeSetter<TMP_InputField>> Setters => new() {
            {"InputContentType", new EnumSetter<TMP_InputField, TMP_InputField.ContentType>(((component, value) => component.contentType = value)) }
        };
    }
}