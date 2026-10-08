using System.Collections.Generic;

namespace Orbit.ComponentProcessors {
    using TypeSetters;
    using UnityEngine;

    public class CanvasGroupProcessor : ComponentProcessor<CanvasGroup> {
        public override Dictionary<string, TypeSetter<CanvasGroup>> Setters => new() {
            {"Interactable", new BoolSetter<CanvasGroup>((component, value) => component.interactable = value) },
            {"Alpha", new FloatSetter<CanvasGroup>((component, value) => component.alpha = value) },
            {"BlocksRaycasts", new BoolSetter<CanvasGroup>((component, value) => component.blocksRaycasts = value) }
        };
    }
}