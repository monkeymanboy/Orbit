﻿using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using TMPro;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class TextMeshProUGUIProcessor : ComponentProcessor<TextMeshProUGUI> {
        public override Dictionary<string, TypeSetter<TextMeshProUGUI>> Setters => new() {
            {"Text", new StringSetter<TextMeshProUGUI>((component, value) => component.text = value) },
            {"FontSize", new FloatSetter<TextMeshProUGUI>((component, value) => component.fontSize = value) },
            {"FontColor", new ColorSetter<TextMeshProUGUI>((component, value) => component.color = value) },
            {"RichText", new BoolSetter<TextMeshProUGUI>((component, value) => component.richText = value) },
            {"FontStyle", new EnumSetter<TextMeshProUGUI, FontStyles>((component, value) => component.fontStyle = value) },
            {"TextAlignment", new EnumSetter<TextMeshProUGUI, TextAlignmentOptions>((component, value) => component.alignment = value) },
            {"TextOverflowMode", new EnumSetter<TextMeshProUGUI, TextOverflowModes>((component, value) => component.overflowMode = value) },
            {"WordWrapping", new BoolSetter<TextMeshProUGUI>((component, value) => component.enableWordWrapping = value) },
        };
    }
}
