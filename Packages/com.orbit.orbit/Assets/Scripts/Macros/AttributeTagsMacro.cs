namespace Orbit.Macros {
    using Attributes.TagGenerators;
    using Parser;
    using System.Collections.Generic;
    using System.Xml;
    using TypeSetters;
    using UnityEngine;

    public class AttributeTagsMacro : Macro<AttributeTagsMacro.AttributeTagsMacroData> {
        public struct AttributeTagsMacroData {
            public string Group;
        }
        public override string Tag => "ATTRIBUTE_TAGS";

        public override Dictionary<string, TypeSetter<AttributeTagsMacroData>> Setters => new() {
            {"Group", new StringSetter<AttributeTagsMacroData>((ref AttributeTagsMacroData data, string value) => data.Group = value) }
        };

        public override void Execute(XmlNode node, GameObject parent, OrbitRenderData renderData, AttributeTagsMacroData data) {
            foreach((string valueID, TagGenerator tagGenerator) in renderData.TagGenerators) {
                if(tagGenerator.Group != data.Group) continue;
                OrbitParser.DefaultParser.RenderNode(tagGenerator.GenerateTag(node.OwnerDocument, valueID), parent, renderData);
            }
        }
    }
}