﻿using Atlas.Orbit.TypeSetters;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class SelectableProcessor : ComponentProcessor<Selectable> {
        public override Dictionary<string, TypeSetter<Selectable>> Setters => new() {
            {"Interactable", new BoolSetter<Selectable>((component, value) => component.interactable = value) },
        };
    }
}
