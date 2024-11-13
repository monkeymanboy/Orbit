using System.Collections.Generic;
using System.Xml;
using Atlas.Orbit.TypeSetters;
using UnityEngine;

namespace Atlas.Orbit.Macros {
    using Parser;
    using TypeSetters;

    public class DefineBool : DefineMacro<bool> {
        public override string Tag => "DEFINE_BOOL";

        public override TypeSetter<DefineMacroData> ValueSetter => new BoolSetter<DefineMacroData>((ref DefineMacroData data, bool value) => data.Value = value);
    }

    public class DefineFloat : DefineMacro<float> {
        public override string Tag => "DEFINE_FLOAT";

        public override TypeSetter<DefineMacroData> ValueSetter => new FloatSetter<DefineMacroData>((ref DefineMacroData data, float value) => data.Value = value);
    }

    public class DefineColor : DefineMacro<Color> {
        public override string Tag => "DEFINE_COLOR";

        public override TypeSetter<DefineMacroData> ValueSetter => new ColorSetter<DefineMacroData>((ref DefineMacroData data, Color value) => data.Value = value);
    }

    public abstract class DefineMacro<T> : Macro<DefineMacro<T>.DefineMacroData> {
        public struct DefineMacroData {
            public string ID;
            public T Value;
        }

        public abstract TypeSetter<DefineMacroData> ValueSetter { get; }

        public override Dictionary<string, TypeSetter<DefineMacroData>> Setters => new() {
            {"ID", new StringSetter<DefineMacroData>((ref DefineMacroData data, string value) => data.ID = value) },
            {"Value", ValueSetter }
        };

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, DefineMacroData data) {
            renderData.SetValue(data.ID, data.Value);
        }
    }
}
