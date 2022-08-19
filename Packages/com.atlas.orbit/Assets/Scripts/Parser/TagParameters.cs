using System.Collections.Generic;

namespace Atlas.Orbit.Parser {
    public struct TagParameters {
        public UIRenderData RenderData { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public Dictionary<string, UIValue> Values { get; set; }
    }
}
