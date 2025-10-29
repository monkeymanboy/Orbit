using System.Collections.Generic;
using TMPro;

namespace Orbit.ComponentProcessors {
    using TypeSetters;
    using UnityEngine;

    public class TMP_TextProcessor : ComponentProcessor<TMP_Text> {
        public override Dictionary<string, TypeSetter<TMP_Text>> Setters => new() {
            {"Text", new StringSetter<TMP_Text>((component, value) => component.text = value) },
            {"FontSize", new FloatSetter<TMP_Text>((component, value) => component.fontSize = value) },
            {"AutoFontSize", new Vector2Setter<TMP_Text>(SetAutoFontSize) },
            {"FontColor", new ColorSetter<TMP_Text>((component, value) => component.color = value) },
            {"RichText", new BoolSetter<TMP_Text>((component, value) => component.richText = value) },
            {"FontStyle", new EnumSetter<TMP_Text, FontStyles>((component, value) => component.fontStyle = value) },
            {"TextAlignment", new EnumSetter<TMP_Text, TextAlignmentOptions>((component, value) => component.alignment = value) },
            {"TextOverflowMode", new EnumSetter<TMP_Text, TextOverflowModes>((component, value) => component.overflowMode = value) },
            {"WordWrapping", new BoolSetter<TMP_Text>((component, value) => component.enableWordWrapping = value) },
            {"TextMargin", new Vector4Setter<TMP_Text>((component, value) => component.margin = value) }
        };

        private void SetAutoFontSize(TMP_Text component, Vector2 size) {
            component.fontSizeMin = size.x;
            component.fontSizeMax = size.y;
            component.enableAutoSizing = true;
        }
    }
}
