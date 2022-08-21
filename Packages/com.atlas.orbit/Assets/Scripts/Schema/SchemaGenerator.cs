using Atlas.Orbit.Components;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

namespace Atlas.Orbit.Schema {
    using ComponentProcessors;
    using Macros;
    using Parser;

    public static class SchemaGenerator {
        public static void Generate() {
            OrbitParser orbitParser = OrbitParser.DefaultParser;
            XmlSchema schema = new XmlSchema();
            orbitParser.Init();

            XmlSchemaSimpleType valueBoundType = new XmlSchemaSimpleType();
            XmlSchemaSimpleTypeRestriction valueBoundTypeRestriction = new XmlSchemaSimpleTypeRestriction();
            XmlSchemaPatternFacet valueBoundPattern = new XmlSchemaPatternFacet();
            valueBoundTypeRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            valueBoundPattern.Value = "([~].*)"; //Matches the pattern ~ followed by anything else
            valueBoundTypeRestriction.Facets.Add(valueBoundPattern);
            valueBoundType.Content = valueBoundTypeRestriction;
            valueBoundType.Name = "ValueBoundType";
            schema.Items.Add(valueBoundType);

            List<string> addedTags = new();
            #region Prefab Tags
            foreach(Object resource in Resources.LoadAll("Orbit/Prefabs")) {
                if(addedTags.Contains(resource.name))
                    continue;
                XmlSchemaElement currentElement = new() { Name = resource.name };

                XmlSchemaComplexType complexType = new();
                XmlSchemaSequence sequence = new();
                XmlSchemaAny any = new() { MinOccurs = 0, MaxOccursString = "unbounded" };
                sequence.Items.Add(any);
                complexType.Particle = sequence;
                
                GameObject nodeGO = resource as GameObject;
                MarkupPrefab markupPrefab = nodeGO.GetComponent<MarkupPrefab>();
                foreach(ComponentProcessor processor in orbitParser.ComponentProcessors) {
                    Component component = markupPrefab.FindComponent(processor.ComponentType);
                    if(component != null) {
                        foreach(XmlSchemaAttribute attribute in processor.GenerateSchemaAttributes()) {
                            complexType.Attributes.Add(attribute);
                        }
                    }
                }

                currentElement.SchemaType = complexType;
                schema.Items.Add(currentElement);
                addedTags.Add(resource.name);
            }
            #endregion
            #region Macro Tags
            foreach(KeyValuePair<string, Macro> pair in orbitParser.Macros) {
                XmlSchemaElement currentElement = new XmlSchemaElement();
                currentElement.Name = pair.Key;

                XmlSchemaComplexType complexType = new XmlSchemaComplexType();
                XmlSchemaSequence sequence = new XmlSchemaSequence();
                XmlSchemaAny any = new XmlSchemaAny();
                any.MinOccurs = 0;
                any.MaxOccursString = "unbounded";
                sequence.Items.Add(any);
                complexType.Particle = sequence;
                
                foreach(XmlSchemaAttribute attribute in pair.Value.GenerateSchemaAttributes()) {
                    complexType.Attributes.Add(attribute);
                }

                currentElement.SchemaType = complexType;
                schema.Items.Add(currentElement);
            }
            #endregion

            XmlSchemaSet schemaSet = new XmlSchemaSet();

            schemaSet.Add(schema);
            schemaSet.Compile();
            foreach(XmlSchema compiledSchema in schemaSet.Schemas())
                schema = compiledSchema;

            StringWriter schemaWriter = new StringWriter();
            schema.Write(schemaWriter);
            File.WriteAllText("OrbitSchema.xsd", schemaWriter.ToString().Substring(41));//substring 41 skips writing <?xml version="1.0" encoding="utf-16"?>
        }
    }
}
