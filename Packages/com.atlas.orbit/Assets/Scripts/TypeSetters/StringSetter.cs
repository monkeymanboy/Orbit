using System;

namespace Atlas.Orbit.TypeSetters {
    public class StringSetter<T> : TypeSetter<T, string> {
        protected override string[] Regexes => new string[] { ".*" };
        public StringSetter(Action<T, string> setter) : base(setter) { }
        public StringSetter(ActionRef<T, string> setter) : base(setter) { }

        public override string Parse(string value) {
            return value;
        }

        public override void Set(T obj, object value) {
            base.Set(obj, value is string ? value : (value?.ToString() ?? throw new Exception($"Orbit encountered null when string was expected")));
        }
        public override void Set(ref T obj, object value) {
            base.Set(ref obj, value is string ? value : (value?.ToString() ?? throw new Exception($"Orbit encountered null when string was expected")));
        }
    }
}
