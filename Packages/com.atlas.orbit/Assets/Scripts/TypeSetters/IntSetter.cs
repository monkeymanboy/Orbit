using System;
using System.Globalization;

namespace Atlas.Orbit.TypeSetters {
    public class IntSetter<T> : TypeSetter<T, int> {
        protected override string[] Regexes => new string[] { "[-+]?[0-9]*" };
        public IntSetter(Action<T, int> setter) : base(setter) { }

        public override int Parse(string value) {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
