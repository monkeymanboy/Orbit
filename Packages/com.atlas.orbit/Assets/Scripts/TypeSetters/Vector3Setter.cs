using System;
using System.Globalization;
using UnityEngine;

namespace Atlas.Orbit.TypeSetters {
    public class Vector3Setter<T> : TypeSetter<T, Vector3> {
        protected override string[] Regexes => new string[] { "[-+]?[0-9]*\\.?[0-9]+", //matches 'float'
            "[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+" }; //matches 'float,float,float'
        public Vector3Setter(Action<T, Vector3> setter) : base(setter) { }
        public Vector3Setter(ActionRef<T, Vector3> setter) : base(setter) { }

        public override Vector3 Parse(string value) {
            int commaIndex = value.IndexOf(',');
            if(commaIndex == -1) { //If single number use that number as x, y, and z
                float singleFloat = float.Parse(value, CultureInfo.InvariantCulture);
                return new Vector3(singleFloat, singleFloat, singleFloat);
            }
            int secondCommaIndex = value.IndexOf(',', commaIndex+1);
            return new Vector3(float.Parse(value.AsSpan(0, commaIndex), NumberStyles.Any, CultureInfo.InvariantCulture),
                float.Parse(value.AsSpan(commaIndex+1, secondCommaIndex-commaIndex-1), NumberStyles.Any, CultureInfo.InvariantCulture),
                float.Parse(value.AsSpan(secondCommaIndex+1, value.Length-secondCommaIndex-1), NumberStyles.Any, CultureInfo.InvariantCulture));
        }
    }
}