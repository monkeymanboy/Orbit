using System;

namespace Atlas.Orbit.Attributes {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ListenForAttribute : Attribute {
        public string Events { get; }

        public ListenForAttribute(string events) {
            Events = events;
        }
    }
}
