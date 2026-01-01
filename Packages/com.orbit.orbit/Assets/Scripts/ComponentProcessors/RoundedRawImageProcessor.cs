namespace Orbit.ComponentProcessors {
    using System.Collections.Generic;
    using TypeSetters;
    using UnityEngine;
    using Components.Graphic;

    public class RoundedRawImageProcessor : ComponentProcessor<RoundedRawImage> {
        public override Dictionary<string, TypeSetter<RoundedRawImage>> Setters => new() {
            {"ImageColor", new ColorSetter<RoundedRawImage>((component, value) => component.color = value) },
            {"ImageTexture", new ObjectSetter<RoundedRawImage, Texture>((component, value) => component.Texture = value) }
        };
    }
}