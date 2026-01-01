namespace Orbit.ComponentProcessors {
    using System.Collections.Generic;
    using Components.Graphic;
    using TypeSetters;

    public class RoundedRectProcessor : ComponentProcessor<RoundedRect> {
        public override Dictionary<string, TypeSetter<RoundedRect>> Setters => new() {
            {"RectFillMode", new EnumSetter<RoundedRect,RoundedRect.FillMode>((component, value) => component.fillMode = value) },
            {"EdgeThickness", new FloatSetter<RoundedRect>((component, value) => component.EdgeThickness = value) },
            {"TopLeftColor", new ColorSetter<RoundedRect>((component, value) => component.TopLeftColor = value) },
            {"TopLeftRadius", new FloatSetter<RoundedRect>((component, value) => component.TopLeftRadius = value) },
            {"TopRightColor", new ColorSetter<RoundedRect>((component, value) => component.TopRightColor = value) },
            {"TopRightRadius", new FloatSetter<RoundedRect>((component, value) => component.TopRightRadius = value) },
            {"BottomLeftColor", new ColorSetter<RoundedRect>((component, value) => component.BottomLeftColor = value) },
            {"BottomLeftRadius", new FloatSetter<RoundedRect>((component, value) => component.BottomLeftRadius = value) },
            {"BottomRightColor", new ColorSetter<RoundedRect>((component, value) => component.BottomRightColor = value) },
            {"BottomRightRadius", new FloatSetter<RoundedRect>((component, value) => component.BottomRightRadius = value) },
            {"CornerRadius", new FloatSetter<RoundedRect>((component, value) => {
                component.TopLeftRadius = value;
                component.TopRightRadius = value;
                component.BottomLeftRadius = value;
                component.BottomRightRadius = value;
            }) },
            {"TopColor", new ColorSetter<RoundedRect>((component, value) => {
                component.TopLeftColor = value;
                component.TopRightColor = value;
            }) },
            {"BottomColor", new ColorSetter<RoundedRect>((component, value) => {
                component.BottomLeftColor = value;
                component.BottomRightColor = value;
            }) },
            {"LeftColor", new ColorSetter<RoundedRect>((component, value) => {
                component.TopLeftColor = value;
                component.BottomLeftColor = value;
            }) },
            {"RightColor", new ColorSetter<RoundedRect>((component, value) => {
                component.TopRightColor = value;
                component.BottomRightColor = value;
            }) }
        };
    }
}