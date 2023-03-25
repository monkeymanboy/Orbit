using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class ImageProcessor : ComponentProcessor<Image> {
        public override Dictionary<string, TypeSetter<Image>> Setters => new() {
            {"ImageColor", new ColorSetter<Image>((component, value) => component.color = value) },
            {"ImageSprite", new ObjectSetter<Image, Sprite>((component, value) => component.sprite = value) },
            {"PreserveAspect", new BoolSetter<Image>((component, value) => component.preserveAspect = value) },
        };
    }
}
