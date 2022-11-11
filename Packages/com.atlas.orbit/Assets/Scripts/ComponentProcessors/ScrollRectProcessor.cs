using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Atlas.Orbit.ComponentProcessors {
    public class ScrollRectProcessor : ComponentProcessor<ScrollRect> {
        public override Dictionary<string, TypeSetter<ScrollRect>> Setters => new() {
            { "MovementType", new EnumSetter<ScrollRect, ScrollRect.MovementType>((component, value) => component.movementType = value) },
            { "Elasticity", new FloatSetter<ScrollRect>((component, value) => component.elasticity = value) },
            { "Horizontal", new BoolSetter<ScrollRect>((component, value) => component.horizontal = value) },
            { "Vertical", new BoolSetter<ScrollRect>((component, value) => component.vertical = value) },
            { "Inertia", new BoolSetter<ScrollRect>((component, value) => component.inertia = value) },
            { "DecelerationRate", new FloatSetter<ScrollRect>((component, value) => component.decelerationRate = value) },
            { "ScrollSensitivity", new FloatSetter<ScrollRect>((component, value) => component.scrollSensitivity = value) },
        };
    }
}