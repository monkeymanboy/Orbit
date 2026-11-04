namespace Orbit.ComponentProcessors {
    using System.Collections.Generic;
    using TypeSetters;
    using UnityEngine;
    using UnityEngine.UI;

    public class RawImageProcessor : ComponentProcessor<RawImage> {
        public override Dictionary<string, TypeSetter<RawImage>> Setters => new() {
            {"ImageColor", new ColorSetter<RawImage>((component, value) => component.color = value) },
            {"ImageTexture", new ObjectSetter<RawImage, Texture>((component, value) => component.texture = value) },
            {"ImageUV", new RectSetter<RawImage>((component, value) => component.uvRect = value) },
            {"ImageUVX", new FloatSetter<RawImage>((component, value) => {
                Rect rect = component.uvRect;
                rect.x = value;
                component.uvRect = rect;
            })},
            {"ImageUVY", new FloatSetter<RawImage>((component, value) => {
                Rect rect = component.uvRect;
                rect.y = value;
                component.uvRect = rect;
            })},
            {"ImageUVWidth", new FloatSetter<RawImage>((component, value) => {
                Rect rect = component.uvRect;
                rect.width = value;
                component.uvRect = rect;
            })},
            {"ImageUVHeight", new FloatSetter<RawImage>((component, value) => {
                Rect rect = component.uvRect;
                rect.height = value;
                component.uvRect = rect;
            })}
        };
    }
}