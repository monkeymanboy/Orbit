using System;
using System.Globalization;
using UnityEngine;

namespace Orbit.TypeSetters {
    public class Vector2Setter<T> : TypeSetter<T, Vector2> {
        protected override string[] Regexes => new string[] { "[-+]?[0-9]*\\.?[0-9]+", //matches 'float'
            "[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+"
        }; //matches 'float,float'
        public Vector2Setter(Action<T, Vector2> setter) : base(setter) { }
        public Vector2Setter(ActionRef<T, Vector2> setter) : base(setter) { }

        public override Vector2 Parse(string value) {
            int commaIndex = value.IndexOf(',');
            if(commaIndex == -1) { //If single number use that number as x and y
                float singleFloat = float.Parse(value, CultureInfo.InvariantCulture);
                return new Vector2(singleFloat, singleFloat);
            }
            return new Vector2(float.Parse(value.AsSpan(0, commaIndex), NumberStyles.Any, CultureInfo.InvariantCulture),
                float.Parse(value.AsSpan(commaIndex+1, value.Length-commaIndex-1), NumberStyles.Any, CultureInfo.InvariantCulture));
        }
    }
}
