using Orbit.TypeSetters;
using System.Collections.Generic;
using Orbit.Components;
using Orbit.Parser;

namespace Orbit.ComponentProcessors {
    using Components;
    using Parser;
    using TypeSetters;

    public class PageIndicatorProcessor : ComponentProcessor<PageIndicator> {
        public override Dictionary<string, TypeSetter<PageIndicator>> Setters => new() {
            {"CurrentPage", new IntSetter<PageIndicator>((comonent, value) => comonent.CurrentPage = value) },
            {"PageCount", new IntSetter<PageIndicator>((comonent, value) => comonent.PageCount = value) },
            {"MaxPages", new IntSetter<PageIndicator>((comonent, value) => comonent.MaxPages = value) },
            {"ColorFunction", new ObjectSetter<PageIndicator, UIFunction>((comonent, value) => comonent.ColorFunction = value) },
            {"SelectPageFunction", new ObjectSetter<PageIndicator, UIFunction>((comonent, value) => comonent.SelectPageFunction = value) },
        };
    }
}
