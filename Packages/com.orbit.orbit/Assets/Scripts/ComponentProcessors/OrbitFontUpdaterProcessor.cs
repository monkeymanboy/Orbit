using Orbit.Components;
using Orbit.TypeSetters;
using System.Collections.Generic;
using TMPro;

namespace Orbit.ComponentProcessors {
    using Parser;
    using UnityEngine;

    public class OrbitFontUpdaterProcessor : ComponentProcessor<OrbitFontUpdater> {
        public override Dictionary<string, TypeSetter<OrbitFontUpdater>> Setters => new() {
            {"Font", new FontSetter<OrbitFontUpdater>((component, value) => component.Font = value) },
            {"FontWeight", new EnumSetter<OrbitFontUpdater, FontWeight>((component, value) => component.FontWeight = value) }
        };

        public override void Process(Component genericComponent, TagParameters processorParams) {
            base.Process(genericComponent, processorParams);
            ((OrbitFontUpdater)genericComponent).ApplyDefault();
        }
    }
}