using System.Collections.Generic;
using System.Xml;

namespace Atlas.Orbit.Parser {
    public struct TagParameters {
        public struct BoundData {
            public string data;
            public UIValue boundValue;

            public BoundData(string data) {
                this.data = data;
                this.boundValue = null;
            }
            public BoundData(UIValue boundValue) {
                this.data = null;
                this.boundValue = boundValue;
            }
        }
        public UIRenderData RenderData { get; set; }
        public Dictionary<string, BoundData> Data { get; set; }
        public XmlNode Node { get; set; }
    }
}
