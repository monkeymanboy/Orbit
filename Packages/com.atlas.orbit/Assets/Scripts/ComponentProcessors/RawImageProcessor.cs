namespace Atlas.Orbit.ComponentProcessors {
    using System.Collections.Generic;
    using TypeSetters;
    using UnityEngine;
    using UnityEngine.UI;

    public class RawImageProcessor : ComponentProcessor<RawImage> {
        public override Dictionary<string, TypeSetter<RawImage>> Setters => new() {
            {"ImageColor", new ColorSetter<RawImage>((component, value) => component.color = value) },
            {"ImageTexture", new ObjectSetter<RawImage, Texture>((component, value) => component.texture = value) }
        };
    }
}