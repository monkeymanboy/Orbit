namespace Orbit.Schema.Attributes {
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequiresPropertyAttribute : Attribute {
        public string Property { get; }
        
        public RequiresPropertyAttribute(string property) {
            Property = property;
        }
    }
}