namespace Atlas.Orbit.Attributes.TagGenerators {
    using System.Xml;
    using TMPro;
    using UnityEngine;
    using Util;

    public class OrbitTextAttribute : TagGenerator {
        public string FontColor { get; set; }

        private float? fontSize;
        public float FontSize {
            get => fontSize.GetValueOrDefault();
            set => fontSize = value;
        }

        private float? autoMinFontSize;
        /// <summary>
        /// Sets font size automatically between AutoMinFontSize and FontSize
        /// </summary>
        public float AutoMinFontSize {
            get => autoMinFontSize.GetValueOrDefault();
            set => autoMinFontSize = value;
        }

        private bool? richText;
        public bool RichText {
            get => richText.GetValueOrDefault();
            set => richText = value;
        }

        private bool? wordWrapping;
        public bool WordWrapping {
            get => wordWrapping.GetValueOrDefault();
            set => wordWrapping = value;
        }

        private FontStyles? fontStyle;
        public FontStyles FontStyle {
            get => fontStyle.GetValueOrDefault();
            set => fontStyle = value;
        }

        private TextAlignmentOptions? textAlignment;
        public TextAlignmentOptions TextAlignment {
            get => textAlignment.GetValueOrDefault();
            set => textAlignment = value;
        }

        private TextOverflowModes? textOverflowMode;
        public TextOverflowModes TextOverflowMode {
            get => textOverflowMode.GetValueOrDefault();
            set => textOverflowMode = value;
        }

        public override XmlNode GenerateTag(XmlDocument doc, string propertyId) {
            XmlNode node = doc.CreateNode("element", "Text", null);
            node.AddAttribute("Text", $"~{propertyId}");
            if(FontColor != null)
                node.AddAttribute("FontColor", FontColor);
            if(fontSize.HasValue) {
                if(autoMinFontSize.HasValue)
                    node.AddAttribute("AutoFontSize", $"{autoMinFontSize.Value},{fontSize.Value}");
                else
                    node.AddAttribute("FontSize", fontSize.Value);
            }
            if(richText.HasValue)
                node.AddAttribute("RichText", richText.Value);
            if(wordWrapping.HasValue)
                node.AddAttribute("WordWrapping", wordWrapping.Value);
            if(fontStyle.HasValue)
                node.AddAttribute("FontStyle", fontStyle.Value);
            if(textAlignment.HasValue)
                node.AddAttribute("FontStyle", textAlignment.Value);
            if(textOverflowMode.HasValue)
                node.AddAttribute("FontStyle", textOverflowMode.Value);
            return node;
        }
    }
}