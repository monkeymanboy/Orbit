﻿using Atlas.Orbit.TypeSetters;
using Atlas.Orbit.Parser;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Atlas.Orbit.ComponentProcessors {
    using Parser;
    using TypeSetters;

    public class ButtonProcessor : ComponentProcessor<Button> {
        public override Dictionary<string, TypeSetter<Button>> Setters => new() {
            {"ClickEvent", new StringSetter<Button>(ApplyClickEvent) },
        };

        private void ApplyClickEvent(Button button, string events) {
            UIRenderData data = CurrentData;
            button.onClick.AddListener(() => {
                data.EmitEvent(events);
            });
        }
    }
}
