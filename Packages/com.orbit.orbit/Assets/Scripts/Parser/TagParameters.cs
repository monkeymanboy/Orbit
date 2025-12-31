using System.Collections.Generic;
using System.Xml;
using System;

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
            public static BoundData FromString(OrbitRenderData renderData, string value)
            {
                switch (value[0])
                {
                    case OrbitParser.RETRIEVE_VALUE_PREFIX:
                        return new BoundData(renderData.GetValueFromID(value.Substring(1)));
                    case OrbitParser.RESOURCE_VALUE_PREFIX:
                        return new BoundData(value.Substring(1), true);
                    case OrbitParser.GLOBAL_VALUE_PREFIX:
                        if (!renderData.Parser.Globals.TryGetValue(value.Substring(1), out UIValue uiValue))
                            throw new Exception($"Attempted to access '{value}' but no such global exists");
                        return new BoundData(uiValue);
                    default:
                        return new BoundData(value);
                }
            }
        }
        public OrbitRenderData RenderData { get; set; }
        public Dictionary<string, BoundData> Data { get; set; }
        public Dictionary<string, BoundData> DefaultData { get; set; }
        public XmlNode Node { get; set; }
    }
}
