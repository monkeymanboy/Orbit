namespace Orbit.ComponentProcessors {
    using System.Collections.Generic;
    using UnityEngine;
    using Components.Graphic;
    using TypeSetters;

    public class RoundedImageProcessor : ComponentProcessor<RoundedImage> {
        public override Dictionary<string, TypeSetter<RoundedImage>> Setters => new() {
            {"ImageColor", new ColorSetter<RoundedImage>((component, value) => component.color = value) },
            {"ImageSprite", new ObjectSetter<RoundedImage, Sprite>((component, value) => component.Sprite = value) },
            {"ImageMaterial", new ObjectSetter<RoundedImage, Material>((component, value) => component.material = value) },
            {"PreserveAspect", new BoolSetter<RoundedImage>((component, value) => component.PreserveAspect = value) }
        };
    }
}