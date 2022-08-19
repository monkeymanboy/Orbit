using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atlas.Orbit.TypeSetters {
    public class ColorSetter<T> : TypeSetter<T, Color> {
        protected override string[] Regexes => new string[] { "([#][a-fA-F0-9]*)" };
        public ColorSetter(Action<T, Color> setter) : base(setter) { }

        public override Color Parse(string value) {
            if(ColorUtility.TryParseHtmlString(value, out Color color))
                return color;
            throw new Exception();
        }
        protected override IEnumerable<string> GenerateEnumerations() => new string[] {
            "red",
            "cyan",
            "blue",
            "darkblue",
            "lightblue",
            "purple",
            "yellow",
            "lime",
            "fuchsia",
            "white",
            "silver",
            "grey",
            "black",
            "orange",
            "brown",
            "maroon",
            "green",
            "olive",
            "navy",
            "teal",
            "aqua",
            "magenta",
        };
    }
}
