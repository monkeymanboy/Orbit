using System.Collections.Generic;
using System.Xml;

namespace Orbit.Parser {
    public struct TagParameters {
        public struct BoundData {
            public string data;
            public UIValue boundValue;
            public bool isDataResourcePath;

            public BoundData(string data, bool isDataResourcePath = false) {
                this.data = data;
                this.boundValue = null;
                this.isDataResourcePath = isDataResourcePath;
            }
            public BoundData(UIValue boundValue) {
                this.data = null;
                this.boundValue = boundValue;
                this.isDataResourcePath = false;
            }
        }
        public UIRenderData RenderData { get; set; }
        public Dictionary<string, BoundData> Data { get; set; }
        public XmlNode Node { get; set; }
    }
}
