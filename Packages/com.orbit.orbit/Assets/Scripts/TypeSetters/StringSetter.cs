using System;

namespace Orbit.TypeSetters {
    using UnityEngine;

    public class StringSetter<T> : TypeSetter<T, string> {
        protected override string[] Regexes => new string[] { ".*" };
        public StringSetter(Action<T, string> setter) : base(setter) { }
        public StringSetter(ActionRef<T, string> setter) : base(setter) { }

        public override void SetFromResource(T obj, string resourcePath) {
            Setter(obj, Resources.Load<TextAsset>(resourcePath).text);
        }

        public override void SetFromResource(ref T obj, string resourcePath) {
            RefSetter(ref obj, Resources.Load<TextAsset>(resourcePath).text);
        }
        
        public override string Parse(string value) {
            return value;
        }

        public override void Set<SetType>(T obj, SetType value) {
            base.Set(obj, (value?.ToString() ?? throw new Exception($"Orbit encountered null when string was expected")));
        }
        public override void Set<SetType>(ref T obj, SetType value) {
            base.Set(ref obj, (value?.ToString() ?? throw new Exception($"Orbit encountered null when string was expected")));
        }
    }
}
