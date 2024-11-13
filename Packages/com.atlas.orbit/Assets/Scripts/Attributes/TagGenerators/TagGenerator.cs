using System;
using System.Xml;

namespace Atlas.Orbit.Attributes.TagGenerators {
    /// <summary>
    /// Presence of this attribute will also implicitly create a UIValue, though you can still define one explicitly if you like
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class TagGenerator : Attribute {
        public string Group { get; set; }
        public abstract XmlNode GenerateTag(XmlDocument doc, string propertyId);
    }
}