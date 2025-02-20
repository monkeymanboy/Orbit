using Orbit.Components;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

namespace Orbit.Schema {
    using ComponentProcessors;
    using Components;
    using Macros;
    using Parser;
    using System.Text;

    public static class SchemaGenerator {
        public static void Generate() {
            OrbitParser orbitParser = OrbitParser.DefaultParser;
            orbitParser.Init(OrbitConfig.Config);
            XmlSchema schema = new();

            XmlSchemaSimpleType valueBoundType = new();
            XmlSchemaSimpleTypeRestriction valueBoundTypeRestriction = new();
            valueBoundTypeRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            XmlSchemaPatternFacet valueBoundPattern = new();
            valueBoundPattern.Value = "([~].*)"; //Matches the pattern ~ followed by anything else
            valueBoundTypeRestriction.Facets.Add(valueBoundPattern);
            XmlSchemaPatternFacet resourceBoundPattern = new();
            resourceBoundPattern.Value = "([@].*)"; //Matches the pattern @ followed by anything else
            valueBoundTypeRestriction.Facets.Add(resourceBoundPattern);
            valueBoundType.Content = valueBoundTypeRestriction;
            XmlSchemaPatternFacet globalBoundPattern = new();
            globalBoundPattern.Value = "([$].*)"; //Matches the pattern $ followed by anything else
            valueBoundTypeRestriction.Facets.Add(globalBoundPattern);
            valueBoundType.Content = valueBoundTypeRestriction;
            valueBoundType.Name = "BoundType";
            schema.Items.Add(valueBoundType);

            List<string> addedTags = new();
            List<string> addedAttributeGroups = new();
            #region Prefab Tags
            foreach(Object resource in Resources.LoadAll("OrbitPrefabs")) {
                if(addedTags.Contains(resource.name))
                    continue;
                XmlSchemaElement currentElement = new() { Name = resource.name };

                string typeName = $"{resource.name}Tag";
                XmlSchemaComplexType complexType = new() { Name = typeName };
                XmlSchemaSequence sequence = new() { MinOccurs = 0, MaxOccursString = "unbounded" };
                XmlSchemaAny any = new() { MinOccurs = 0 };
                sequence.Items.Add(any);
                complexType.Particle = sequence;
                
                GameObject nodeGO = resource as GameObject;
                MarkupPrefab markupPrefab = nodeGO.GetComponent<MarkupPrefab>();
                foreach(ComponentProcessor processor in orbitParser.ComponentProcessors) {
                    if (markupPrefab == null) {
                        Debug.LogWarning($"{resource.name} is missing it's MarkupPrefab component");
                        continue;
                    }
                    Component component = markupPrefab.FindComponent(processor.ComponentType);
                    if(component != null) {
                        string attributeGroupName = $"{processor.ComponentType.Name}Component";
                        if(!addedAttributeGroups.Contains(attributeGroupName)) {
                            XmlSchemaAttributeGroup group = new() { Name = attributeGroupName };
                            foreach(XmlSchemaAttribute attribute in processor.GenerateSchemaAttributes()) {
                                group.Attributes.Add(attribute);
                            }
                            schema.Items.Add(group);
                            addedAttributeGroups.Add(attributeGroupName);
                        }
                        complexType.Attributes.Add(new XmlSchemaAttributeGroupRef() { RefName = new XmlQualifiedName(attributeGroupName) });
                    }
                }
                
                currentElement.SchemaTypeName = new XmlQualifiedName(typeName);
                //currentElement.SchemaType = complexType;
                schema.Items.Add(currentElement);
                schema.Items.Add(complexType);
                addedTags.Add(resource.name);
            }
            #endregion
            #region Macro Tags
            foreach(KeyValuePair<string, Macro> pair in orbitParser.Macros) {
                XmlSchemaElement currentElement = new() { Name = pair.Key };

                string typeName = $"{pair.Key}Macro";
                XmlSchemaComplexType complexType = new() { Name = typeName };
                XmlSchemaSequence sequence = new() { MinOccurs = 0, MaxOccursString = "unbounded" };
                XmlSchemaAny any = new() { MinOccurs = 0 };
                sequence.Items.Add(any);
                complexType.Particle = sequence;
                
                foreach(XmlSchemaAttribute attribute in pair.Value.GenerateSchemaAttributes()) {
                    complexType.Attributes.Add(attribute);
                }

                currentElement.SchemaTypeName = new XmlQualifiedName(typeName);
                //currentElement.SchemaType = complexType;
                schema.Items.Add(currentElement);
                schema.Items.Add(complexType);
            }
            #endregion

            XmlSchemaSet schemaSet = new();

            schemaSet.Add(schema);
            schemaSet.Compile();
            foreach(XmlSchema compiledSchema in schemaSet.Schemas())
                schema = compiledSchema;

            using StreamWriter writer = new("OrbitSchema.xsd", false, Encoding.UTF8);
            schema.Write(writer);
        }
    }
}
