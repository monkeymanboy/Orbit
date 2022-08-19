using System;

namespace Atlas.Orbit.Attributes {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EventEmitterAttribute : Attribute {
        public string ID { get; }

        public EventEmitterAttribute(string id) {
            ID = id;
        }
    }
}
