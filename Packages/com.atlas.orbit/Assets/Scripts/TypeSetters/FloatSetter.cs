using System;
using System.Globalization;

namespace Atlas.Orbit.TypeSetters {
    public class FloatSetter<T> : TypeSetter<T, float> {
        protected override string[] Regexes => new string[] { "[-+]?[0-9]*\\.?[0-9]+" };
        public FloatSetter(Action<T, float> setter) : base(setter) { }
        public FloatSetter(ActionRef<T, float> setter) : base(setter) { }

        public override float Parse(string value) {
            return float.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
