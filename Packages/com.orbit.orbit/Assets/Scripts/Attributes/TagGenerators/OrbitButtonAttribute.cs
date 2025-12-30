namespace Orbit.Attributes.TagGenerators {
    using System;
    using System.Xml;
    using Util;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OrbitButtonAttribute : OrbitTextAttribute {
        public string ClickEvent { get; set; }
        public override XmlNode GenerateTag(XmlDocument doc, string propertyId) {
            Tag ??= "Button"; //Need to do this first so we can just inherit all of text stuff while defaulting to button
            Text ??= propertyId; //We also want to change how null text is handled here and make it the propertyId in that case (and not the value of the property)
            XmlNode node = base.GenerateTag(doc, propertyId);
            node.AddAttribute("ClickEvent", ClickEvent ?? propertyId);
            return node;
        }
    }
}