using System.Collections.Generic;
using System.Xml;
using UnityEngine;

//TODO Would be kind of cool to create these with a source generator out of every unique TypeSetter but that would be kind of overkill
namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    public class SetBool : SetMacro<bool> {
        public override string Tag => "SET_BOOL";

        public override TypeSetter<SetMacroData> ValueSetter => new BoolSetter<SetMacroData>((ref SetMacroData data, bool value) => data.Value = value);
    }

    public class SetString : SetMacro<string> {
        public override string Tag => "SET_STRING";

        public override TypeSetter<SetMacroData> ValueSetter => new StringSetter<SetMacroData>((ref SetMacroData data, string value) => data.Value = value);
    }

    public class SetInt : SetMacro<int> {
        public override string Tag => "SET_INT";

        public override TypeSetter<SetMacroData> ValueSetter => new IntSetter<SetMacroData>((ref SetMacroData data, int value) => data.Value = value);
    }

    public class SetFloat : SetMacro<float> {
        public override string Tag => "SET_FLOAT";

        public override TypeSetter<SetMacroData> ValueSetter => new FloatSetter<SetMacroData>((ref SetMacroData data, float value) => data.Value = value);
    }

    public class SetColor : SetMacro<Color> {
        public override string Tag => "SET_COLOR";

        public override TypeSetter<SetMacroData> ValueSetter => new ColorSetter<SetMacroData>((ref SetMacroData data, Color value) => data.Value = value);
    }

    public class SetVector2 : SetMacro<Vector2> {
        public override string Tag => "SET_VECTOR2";

        public override TypeSetter<SetMacroData> ValueSetter => new Vector2Setter<SetMacroData>((ref SetMacroData data, Vector2 value) => data.Value = value);
    }

    public class SetVector3 : SetMacro<Vector3> {
        public override string Tag => "SET_VECTOR3";

        public override TypeSetter<SetMacroData> ValueSetter => new Vector3Setter<SetMacroData>((ref SetMacroData data, Vector3 value) => data.Value = value);
    }

    public class SetVector4 : SetMacro<Vector4> {
        public override string Tag => "SET_VECTOR4";

        public override TypeSetter<SetMacroData> ValueSetter => new Vector4Setter<SetMacroData>((ref SetMacroData data, Vector4 value) => data.Value = value);
    }

    public class SetFont : SetMacro<OrbitConfig.OrbitFont> {
        public override string Tag => "SET_FONT";

        public override TypeSetter<SetMacroData> ValueSetter => new FontSetter<SetMacroData>((ref SetMacroData data, OrbitConfig.OrbitFont value) => data.Value = value);
    }

    public class SetSprite : SetMacro<Sprite> {
        public override string Tag => "SET_SPRITE";

        public override TypeSetter<SetMacroData> ValueSetter => new ObjectSetter<SetMacroData, Sprite>((ref SetMacroData data, Sprite value) => data.Value = value);
    }

    [RequiresProperty("ID")]
    public abstract class SetMacro<T> : Macro<SetMacro<T>.SetMacroData> {
        public struct SetMacroData {
            public string ID;
            public string Event;
            public T Value;
            public UIValue UIValue;
        }

        public abstract TypeSetter<SetMacroData> ValueSetter { get; }

        public override Dictionary<string, TypeSetter<SetMacroData>> Setters => new() {
            {"ID", new StringSetter<SetMacroData>((ref SetMacroData data, string value) => data.ID = value) },
            {"OnEvent", new StringSetter<SetMacroData>((ref SetMacroData data, string value) => data.Event = value) },
            {"Value", ValueSetter }
        };

        public override Dictionary<string, TypeSetter<SetMacroData, UIValue>> ValueSetters => new() {
            {"Value", new ObjectSetter<SetMacroData, UIValue>((ref SetMacroData data, UIValue value) => data.UIValue = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, OrbitRenderData renderData, SetMacroData data) {
            if(data.Event == null) {
                renderData.SetValue(data.ID, data.Value);
            } else {
                if(data.UIValue == null) {
                    renderData.AddEvent(data.Event, () => renderData.SetValue(data.ID, data.Value));
                } else {
                    renderData.AddEvent(data.Event, () => renderData.SetValue(data.ID, data.UIValue.GetValue<T>()));
                }
            }
        }
    }
}
