using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atlas.Orbit.TypeSetters {
    using Parser;

    public class ColorSetter<T> : TypeSetter<T, Color> {
        protected override string[] Regexes => new string[] { "([#][a-fA-F0-9]*)" };
        public ColorSetter(Action<T, Color> setter) : base(setter) { }
        public ColorSetter(ActionRef<T, Color> setter) : base(setter) { }

        public override Color Parse(string value) {
            if(ColorUtility.TryParseHtmlString(value, out Color color))
                return color;
            if(OrbitParser.DefaultParser.ColorDefinitions.TryGetValue(value, out Color definedColor))
                return definedColor;
            throw new Exception($"Error attempting to parse color '{value}'");
        }
        protected override IEnumerable<string> GenerateEnumerations() {
            List<string> validColors = new List<string> {
                "red", "cyan", "blue", "darkblue", "lightblue", "purple", "yellow", "lime", "fuchsia", "white",
                "silver", "grey", "black", "orange", "brown", "maroon", "green", "olive", "navy", "teal", "aqua",
                "magenta",
            };
            validColors.AddRange(OrbitParser.DefaultParser.ColorDefinitions.Keys);
            return validColors;
        }
    }
}
