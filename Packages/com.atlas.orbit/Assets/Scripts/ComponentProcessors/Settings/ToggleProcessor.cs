using Atlas.Orbit.TypeSetters;
using Atlas.Orbit.Parser;
using System.Collections.Generic;
using Atlas.Orbit.Components.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Atlas.Orbit.ComponentProcessors.Settings {
    using TypeSetters;

    public class ToggleProcessor : ComponentProcessor<Toggle> {
        public override Dictionary<string, TypeSetter<Toggle>> Setters => new() {
        };
        
    }
}
