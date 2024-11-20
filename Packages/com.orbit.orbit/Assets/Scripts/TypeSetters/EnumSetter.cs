using System;
using System.Linq;
using System.Collections.Generic;

namespace Orbit.TypeSetters {
    public class EnumSetter<T, U> : TypeSetter<T, U> where U : Enum {
        protected override string[] Regexes {
            get {
                if(!typeof(U).IsDefined(typeof(FlagsAttribute), inherit: false))
                    return new string[] { "[-+]?[0-9]*" };
                string enumerationsString = string.Join('|', GenerateEnumerations()); //I feel like there's gotta be a good way to write the regex to not need to include this twice but it's just for the schema so who cares
                return new string[] { "[-+]?[0-9]*", $"(({enumerationsString}),\\s*)*({enumerationsString})" };
            }
        }

        public EnumSetter(Action<T, U> setter) : base(setter) { }
        public EnumSetter(ActionRef<T, U> setter) : base(setter) { }

        public override U Parse(string value) {
            return (U)Enum.Parse(typeof(U), value);
        }

        protected override IEnumerable<string> GenerateEnumerations() => Enum.GetValues(typeof(U)).Cast<U>().Select(x => x.ToString());
    }
}
