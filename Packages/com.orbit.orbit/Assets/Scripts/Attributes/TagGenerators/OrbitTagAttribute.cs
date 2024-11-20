using System.Xml;
namespace Orbit.Attributes.TagGenerators {
    using Parser;

    public class OrbitTagAttribute : TagGenerator {
        private string tagString;

        public OrbitTagAttribute(string tagString) {
            this.tagString = tagString;
        }

        public override XmlNode GenerateTag(XmlDocument doc, string propertyId) => OrbitParser.DefaultParser.ParseXML(tagString.Replace("$VALUE_ID", propertyId)).ChildNodes[0];
    }
}