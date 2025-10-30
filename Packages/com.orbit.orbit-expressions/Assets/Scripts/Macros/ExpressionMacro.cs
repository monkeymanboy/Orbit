using System.Collections.Generic;
using UnityEngine;
using Orbit.Macros;
using Orbit.Parser;
using Orbit.Schema.Attributes;
using Orbit.TypeSetters;
using System.Xml;
using Unity.VisualScripting.Dependencies.NCalc;

namespace Orbit.Expressions {
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Unity.VisualScripting;

    [RequiresProperty("ID")]
    [RequiresProperty("Expression")]
    public class ExpressionMacro : Macro<ExpressionMacro.ExpressionMacroData> {
        public struct ExpressionMacroData {
            public string ID;
            public string Expression;
        }

        public override string Tag => "EXPRESSION";

        public override Dictionary<string, TypeSetter<ExpressionMacroData>> Setters => new() {
            { "ID", new StringSetter<ExpressionMacroData>((ref ExpressionMacroData data, string value) => data.ID = value) },
            { "Expression", new StringSetter<ExpressionMacroData>((ref ExpressionMacroData data, string value) => data.Expression = value) }
        };

        private static ValueResolveVisitor visitor = new();

        public override void Execute(XmlNode node, GameObject parent, UIRenderData renderData, ExpressionMacroData data) {
            LogicalExpression expression = Expression.Compile(data.Expression, false);
            visitor.RenderData = renderData;
            visitor.DependentValues.Clear();
            expression.Accept(visitor);
            UIValue value = renderData.SetValue(data.ID,visitor.Result);
            foreach (UIValue visitorValue in visitor.DependentValues) {
                visitorValue.OnChange += () => {
                    visitor.RenderData = renderData;
                    visitor.DependentValues.Clear();
                    expression.Accept(visitor);
                    value.SetValue(visitor.Result);
                };
            }
        }
    }

    public class ValueResolveVisitor : EvaluationVisitor {
        private const BindingFlags BINDING_FLAGS = BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private PropertyInfo resultProperty = typeof(EvaluationVisitor).GetProperty("Result", BINDING_FLAGS);
        public UIRenderData RenderData;
        public List<UIValue> DependentValues = new();
        
        public ValueResolveVisitor() : base(null, EvaluateOptions.IterateParameters) {
        }

        public override void Visit(IdentifierExpression identifier) {
            UIValue value = RenderData.GetValueFromID(identifier.Name);
            DependentValues.Add(value);
            resultProperty.SetValue(this, value.GetValue(), BINDING_FLAGS, null, null, null);
        }
    }
}