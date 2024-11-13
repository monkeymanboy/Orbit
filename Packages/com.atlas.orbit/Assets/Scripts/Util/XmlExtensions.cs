namespace Atlas.Orbit.Util {
    using System.Xml;

    public static class XmlDocumentExtensions {
        public static void AddAttribute<T>(this XmlNode node, string attribute, T value) {
            XmlAttribute xmlAttribute = node.OwnerDocument.CreateAttribute(attribute);
            xmlAttribute.Value = value.ToString();
            node.Attributes.Append(xmlAttribute);
        }
    }
}