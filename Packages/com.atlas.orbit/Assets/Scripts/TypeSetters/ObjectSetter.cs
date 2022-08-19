using Atlas.Orbit.Exceptions;
using System;

namespace Atlas.Orbit.TypeSetters {
    using Exceptions;

    public class ObjectSetter<T, U> : TypeSetter<T, U> {
        public ObjectSetter(Action<T, U> setter) : base(setter) { }

        public override U Parse(string value) {
            throw new ParseValueException(typeof(U), value, "Did you mean to use a UIValue?");
        }
    }
}
