using System;

namespace Atlas.Orbit.Attributes {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValueIDAttribute : Attribute {
        public string ID { get; }

        public ValueIDAttribute(string id = null) {
            ID = id;
        }
    }
}
