namespace Orbit.Attributes {
    using System;
    
    [Flags]
    public enum OrbitMemberAccess {
        None,
        PublicField = 1 << 0,
        PrivateField = 1 << 1,
        PublicProperty = 1 << 2,
        PrivateProperty = 1 << 3,
        PublicMethod = 1 << 5,
        PrivateMethod = 1 << 6,
        Public = PublicField | PublicProperty | PublicMethod,
        Private = PrivateField | PrivateProperty | PrivateMethod
    }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class OrbitClassAttribute : Attribute {
        /// <summary>
        /// This will include default empty UIValues on these members
        /// </summary>
        public OrbitMemberAccess Access;
    }
}