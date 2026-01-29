using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Orbit.Macros {
    using Parser;
    using Schema.Attributes;
    using TypeSetters;

    [RequiresProperty("Parent")]
    public class AsParentMacro : Macro<AsParentMacro.AsParentMacroData> {
        public struct AsParentMacroData {
            public Component ParentComponent;
        }
        public override string Tag => "AS_PARENT";
        public override bool CanHaveChildren => true;

        public override Dictionary<string, TypeSetter<AsParentMacroData>> Setters => new() {
            {"Parent", new ObjectSetter<AsParentMacroData, Component>((ref AsParentMacroData data, Component value) => data.ParentComponent = value) },
        };

        public override void Execute(XmlNode node, GameObject parent, OrbitRenderData renderData, AsParentMacroData data) {
            foreach (XmlNode childNode in node.ChildNodes) {
                Parser.RenderNode(childNode, data.ParentComponent.gameObject, renderData);
            }
        }
    }
}