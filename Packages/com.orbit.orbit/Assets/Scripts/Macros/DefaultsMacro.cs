using Orbit.Macros;
using Orbit.Parser;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

public class DefaultsMacro : Macro {
    public override string Tag => "DEFAULTS";
    public override bool CanHaveChildren => true;
    
    public override void Execute(GameObject parent, TagParameters parameters) {
        Dictionary<string, TagParameters.BoundData> originalDefaults = parameters.RenderData.CurrentDefaultProperties;
        Dictionary<string, TagParameters.BoundData> newDefaults;
        if(parameters.DefaultData == null) {
            newDefaults = new(parameters.Data);
        } else {
            newDefaults = new(parameters.DefaultData);
            foreach((string key, TagParameters.BoundData boundData) in parameters.Data) {
                newDefaults[key] = boundData;
            }
        }
        if(newDefaults.TryGetValue("ID", out TagParameters.BoundData boundID) && newDefaults.Remove("ID")) {
            parameters.RenderData.SetValue(boundID.data, newDefaults);
        }
        // Due to how this is currently set up the type is unknown by the macro and is parsed for every occurence of it, though it will at least not be parsed if the tag does not use that attribute
        
        
        parameters.RenderData.CurrentDefaultProperties = newDefaults;
        foreach (XmlNode childNode in parameters.Node.ChildNodes) {
            Parser.RenderNode(childNode, parent, parameters.RenderData);
        }
        parameters.RenderData.CurrentDefaultProperties = originalDefaults;
    }

    public override List<XmlSchemaAttribute> GenerateSchemaAttributes() {
        return Parser.Macros.Where(x => x.Value != this).SelectMany(x => x.Value.GenerateSchemaAttributes()).Union(Parser.ComponentProcessors.SelectMany(x => x.GenerateSchemaAttributes())).GroupBy(x => x.Name).Select(MergeAttributes).ToList();
    }

    private XmlSchemaAttribute MergeAttributes(IEnumerable<XmlSchemaAttribute> attributes) {
        XmlSchemaAttribute combinedAttribute = attributes.First();
        XmlText combinedDocumentationText = GetDocumentationText(combinedAttribute.SchemaType);
        HashSet<string> documentationTypes = new();
        documentationTypes.Add(combinedDocumentationText.Value);
        XmlSchemaSimpleTypeUnion schemaTypeContent = combinedAttribute.SchemaType.Content as XmlSchemaSimpleTypeUnion;
        foreach (XmlSchemaAttribute attribute in attributes.Skip(1)) {
            if(documentationTypes.Add(GetDocumentationText(attribute.SchemaType).Value)) {
                XmlSchemaSimpleTypeUnion attributeTypeContent = attribute.SchemaType.Content as XmlSchemaSimpleTypeUnion;
                foreach (XmlSchemaSimpleType simpleType in attributeTypeContent.BaseTypes.OfType<XmlSchemaSimpleType>().ToList()) {
                    schemaTypeContent.BaseTypes.Add(simpleType);
                }
            }
        }
        combinedDocumentationText.Value = string.Join(", ", documentationTypes);
        combinedAttribute.Use = XmlSchemaUse.None;
        return combinedAttribute;
    }
    
    private XmlText GetDocumentationText(XmlSchemaSimpleType simpleType) => simpleType.Annotation?.Items?.OfType<XmlSchemaDocumentation>().First().Markup.OfType<XmlText>().First();
}
