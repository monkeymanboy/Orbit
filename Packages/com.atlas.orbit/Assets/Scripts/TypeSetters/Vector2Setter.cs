using System;
using System.Globalization;
using UnityEngine;

namespace Atlas.Orbit.TypeSetters {
    public class Vector2Setter<T> : TypeSetter<T, Vector2> {
        protected override string[] Regexes => new string[] { "[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+" }; //matches 'float,float'
        public Vector2Setter(Action<T, Vector2> setter) : base(setter) { }
        public Vector2Setter(ActionRef<T, Vector2> setter) : base(setter) { }

        public override Vector2 Parse(string value) {
            string[] vals = value.Split(',');
            return new Vector2(float.Parse(vals[0], CultureInfo.InvariantCulture),
                               float.Parse(vals[1], CultureInfo.InvariantCulture));
        }
    }
}
