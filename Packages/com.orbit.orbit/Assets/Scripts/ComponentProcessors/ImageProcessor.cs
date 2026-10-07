namespace Orbit.ComponentProcessors {
    using System.Collections.Generic;
    using UnityEngine.UI;
    using UnityEngine;
    using TypeSetters;

    public class ImageProcessor : ComponentProcessor<Image> {
        public enum FillOrigin {
            Bottom = 1, Right = 2, Top = 3, Left = 4
        }

        public override Dictionary<string, TypeSetter<Image>> Setters => new() {
            {"ImageColor", new ColorSetter<Image>((component, value) => component.color = value) },
            {"ImageSprite", new ObjectSetter<Image, Sprite>((component, value) => component.sprite = value) },
            {"ImageMaterial", new ObjectSetter<Image, Material>((component, value) => component.material = value) },
            {"PreserveAspect", new BoolSetter<Image>((component, value) => component.preserveAspect = value) },
            {"ImageType", new EnumSetter<Image, Image.Type>((component, value) => component.type = value) },
            {"ImageFillMethod", new EnumSetter<Image, Image.FillMethod>((component, value) => component.fillMethod = value) },
            {"ImageFillOrigin", new EnumSetter<Image, FillOrigin>((component, value) => component.fillOrigin = (int)value) },
            {"ImageFill", new FloatSetter<Image>((component, value) => component.fillAmount = value) },
            {"ImageFillClockwise", new BoolSetter<Image>((component, value) => component.fillClockwise = value) }
        };
    }
}
