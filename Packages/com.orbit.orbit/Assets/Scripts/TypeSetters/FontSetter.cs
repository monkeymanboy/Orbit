using System;
using System.Collections.Generic;

namespace Orbit.TypeSetters {
    using Exceptions;
    using Parser;

    public class FontSetter<T> : TypeSetter<T, OrbitConfig.OrbitFont> {
        public FontSetter(Action<T, OrbitConfig.OrbitFont> setter) : base(setter) { }
        public FontSetter(ActionRef<T, OrbitConfig.OrbitFont> setter) : base(setter) { }

        public override OrbitConfig.OrbitFont Parse(string value) {
            if(OrbitConfig.Config.Fonts.TryGetValue(value, out OrbitConfig.OrbitFont font))
                return font;
            throw new ParseValueException(typeof(OrbitConfig.OrbitFont), value,"Could not locate font");
        }

        protected override IEnumerable<string> GenerateEnumerations() => OrbitConfig.Config.Fonts.Keys;
    }
}