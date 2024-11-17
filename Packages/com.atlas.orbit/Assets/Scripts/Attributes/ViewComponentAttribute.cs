using System;

namespace Atlas.Orbit.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ViewComponentAttribute : Attribute {
        public string ID { get; }

        public ViewComponentAttribute(string id) {
            ID = id;
        }
    }
}
