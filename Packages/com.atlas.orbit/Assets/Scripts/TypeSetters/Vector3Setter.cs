using System;
using System.Globalization;
using UnityEngine;

namespace Atlas.Orbit.TypeSetters {
    public class Vector3Setter<T> : TypeSetter<T, Vector3> {
        protected override string[] Regexes => new string[] { "[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+" }; //matches 'float,float,float'
        public Vector3Setter(Action<T, Vector3> setter) : base(setter) { }

        public override Vector3 Parse(string value) {
            string[] vals = value.Split(',');
            return new Vector3(float.Parse(vals[0], CultureInfo.InvariantCulture),
                float.Parse(vals[1], CultureInfo.InvariantCulture),
                float.Parse(vals[2], CultureInfo.InvariantCulture));
        }
    }
}