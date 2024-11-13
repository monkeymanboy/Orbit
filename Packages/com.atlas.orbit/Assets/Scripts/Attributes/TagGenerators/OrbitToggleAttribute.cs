namespace Atlas.Orbit.Attributes.TagGenerators {
    using System.Xml;
    using Util;

    public class OrbitToggleAttribute : TagGenerator {
        public string Text { get; set; }
        public string ValueChangedEvent { get; set; }

        public override XmlNode GenerateTag(XmlDocument doc, string propertyId) {
            XmlNode node = doc.CreateNode("element", "SettingToggle", null);
            node.AddAttribute("Text", Text ?? propertyId);
            node.AddAttribute("BoundValue", propertyId);
            if(ValueChangedEvent != null)
                node.AddAttribute("ValueChangedEvent", ValueChangedEvent);
            return node;
        }
    }
}