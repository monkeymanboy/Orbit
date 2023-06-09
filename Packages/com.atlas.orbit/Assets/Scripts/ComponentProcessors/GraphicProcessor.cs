using System.Collections.Generic;
using UnityEngine.UI;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class GraphicProcessor : ComponentProcessor<Graphic> {
        public override Dictionary<string, TypeSetter<Graphic>> Setters => new() {
            {"RaycastTarget", new BoolSetter<Graphic>((component, value) => component.raycastTarget = value) },
        };
    }
}