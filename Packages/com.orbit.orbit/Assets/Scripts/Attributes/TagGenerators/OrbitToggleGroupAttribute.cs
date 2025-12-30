namespace Orbit.Attributes.TagGenerators {
    using System.Xml;
    using Util;

    public class OrbitToggleGroupAttribute : TagGenerator {
        public string ID { get; set; }
        public bool AllowSwitchOff { get; set; }

        public override XmlNode GenerateTag(XmlDocument doc, string propertyId) {
            XmlNode node = doc.CreateNode("element", Tag ?? "TOGGLE_GROUP", null);
            node.AddAttribute("ID", ID ?? propertyId);
            node.AddAttribute("AllowSwitchOff", AllowSwitchOff);
            return node;
        }
    }
}