using System;
using System.Collections.Generic;

namespace Atlas.Orbit.TypeSetters {
    public class BoolSetter<T> : TypeSetter<T, bool> {
        public BoolSetter(Action<T, bool> setter) : base(setter) { }
        public BoolSetter(ActionRef<T, bool> setter) : base(setter) { }

        public override bool Parse(string value) {
            return bool.Parse(value);
        }

        protected override IEnumerable<string> GenerateEnumerations() => new string[] { "true", "false" };
    }
}
