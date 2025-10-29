using System;
using System.Globalization;
using UnityEngine;

namespace Orbit.TypeSetters {
    public class Vector4Setter<T> : TypeSetter<T, Vector4> {
        protected override string[] Regexes => new string[] { "[-+]?[0-9]*\\.?[0-9]+", //matches 'float'
            "[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+,[-+]?[0-9]*\\.?[0-9]+" }; //matches 'float,float,float,float'
        public Vector4Setter(Action<T, Vector4> setter) : base(setter) { }
        public Vector4Setter(ActionRef<T, Vector4> setter) : base(setter) { }

        public override Vector4 Parse(string value) {
            int commaIndex = value.IndexOf(',');
            if(commaIndex == -1) { //If single number use that number as x, y, and z
                float singleFloat = float.Parse(value, CultureInfo.InvariantCulture);
                return new Vector4(singleFloat, singleFloat, singleFloat, singleFloat);
            }
            int secondCommaIndex = value.IndexOf(',', commaIndex+1);
            int thirdCommaIndex = value.IndexOf(',', secondCommaIndex+1);
            return new Vector4(float.Parse(value.AsSpan(0, commaIndex), NumberStyles.Any, CultureInfo.InvariantCulture),
                float.Parse(value.AsSpan(commaIndex+1, secondCommaIndex-commaIndex-1), NumberStyles.Any, CultureInfo.InvariantCulture),
                float.Parse(value.AsSpan(secondCommaIndex+1, thirdCommaIndex-secondCommaIndex-1), NumberStyles.Any, CultureInfo.InvariantCulture),
                float.Parse(value.AsSpan(thirdCommaIndex+1, value.Length-thirdCommaIndex-1), NumberStyles.Any, CultureInfo.InvariantCulture));
        }
    }
}