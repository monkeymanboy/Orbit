using System.Collections.Generic;
using System.Xml;
using Atlas.Orbit.TypeSetters;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using TypeSetters;

    public class DefineBool : DefineMacro<bool> {
        public override string Tag => "DEFINE_BOOL";

        public override TypeSetter<DefineMacro<bool>> ValueSetter => new BoolSetter<DefineMacro<bool>>((data, value) => data.Value = value);
    }

    public class DefineFloat : DefineMacro<float> {
        public override string Tag => "DEFINE_FLOAT";

        public override TypeSetter<DefineMacro<float>> ValueSetter => new FloatSetter<DefineMacro<float>>((data, value) => data.Value = value);
    }

    public class DefineColor : DefineMacro<Color> {
        public override string Tag => "DEFINE_COLOR";

        public override TypeSetter<DefineMacro<Color>> ValueSetter => new ColorSetter<DefineMacro<Color>>((data, value) => data.Value = value);
    }

    public abstract class DefineMacro<T> : Macro<DefineMacro<T>> {
        public string ID { get; set; }
        public T Value { get; set; }

        public abstract TypeSetter<DefineMacro<T>> ValueSetter { get; }

        public override Dictionary<string, TypeSetter<DefineMacro<T>>> Setters => new Dictionary<string, TypeSetter<DefineMacro<T>>>() {
            {"ID", new StringSetter<DefineMacro<T>>((data, value) => data.ID = value) },
            {"Value", ValueSetter }
        };

        public override void Execute(XmlNode node, GameObject parent, DefineMacro<T> data) {
            CurrentData.SetValue(ID, Value);
        }

        public override void SetToDefault() {
            ID = null;
        }
    }
}
