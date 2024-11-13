using System;
using System.Linq;
using System.Collections.Generic;

namespace Atlas.Orbit.TypeSetters {
    public class EnumSetter<T, U> : TypeSetter<T, U> where U : Enum {
        //TODO(David): Generate a regex that will match any number of enumerations separated by a '|' for flags
        public EnumSetter(Action<T, U> setter) : base(setter) { }
        public EnumSetter(ActionRef<T, U> setter) : base(setter) { }

        public override U Parse(string value) {
            string[] toCombine = value.Split('|');

            if(toCombine.Length == 1)
                return (U)Enum.Parse(typeof(U), value);

            //TODO(David): This solution will break for enums not backed by an integer, think of better solution later
            int? combinedValue = null;
            foreach(string enumVal in toCombine) {
                U parsedValue = (U)Enum.Parse(typeof(U), value);
                int intVal = Convert.ToInt32(enumVal);

                if(combinedValue.HasValue == false)
                    combinedValue = intVal;
                else
                    combinedValue |= intVal;
            }
            return (U)Enum.ToObject(typeof(U), combinedValue);
        }

        protected override IEnumerable<string> GenerateEnumerations() => Enum.GetValues(typeof(U)).Cast<U>().Select(x => x.ToString());
    }
}
