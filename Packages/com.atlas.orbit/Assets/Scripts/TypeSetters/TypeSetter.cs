using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Atlas.Orbit.TypeSetters {
    using Exceptions;

    public abstract class TypeSetter<T> {
        public abstract void SetFromString(T obj, string value);
        public abstract void SetFromString(ref T obj, string value);
        public abstract void Set(T obj, object value);
        public abstract void Set(ref T obj, object value);

        public abstract XmlSchemaSimpleType GenerateSchemaType();
    }

    public abstract class TypeSetter<T, U> : TypeSetter<T> {
        protected virtual string[] Regexes => new string[0];
        public Action<T, U> Setter { get; private set; }
        public ActionRef<T, U> RefSetter { get; private set; }
        public delegate void ActionRef<T, U>(ref T item, U value);
        
        public TypeSetter(Action<T, U> setter) {
            Setter = setter;
        }
        public TypeSetter(ActionRef<T, U> refSetter) {
            RefSetter = refSetter;
        }

        public override void SetFromString(T obj, string value) {
            U parsedValue;
            try {
                parsedValue = Parse(value);
            } catch (Exception ex){
                throw ex is ParseValueException ? ex : new ParseValueException(typeof(U), value);
            }
            Setter(obj, parsedValue);
        }
        public override void SetFromString(ref T obj, string value) {
            U parsedValue;
            try {
                parsedValue = Parse(value);
            } catch (Exception ex){
                throw ex is ParseValueException ? ex : new ParseValueException(typeof(U), value);
            }
            RefSetter(ref obj, parsedValue);
        }

        public override void Set(T obj, object value) {
            Setter(obj, (U)value);
        }
        public override void Set(ref T obj, object value) {
            RefSetter(ref obj, (U)value);
        }

        public abstract U Parse(string value);
        
        public override XmlSchemaSimpleType GenerateSchemaType() {
            XmlSchemaSimpleType simpleType = new XmlSchemaSimpleType();

            XmlSchemaAnnotation annotation = new XmlSchemaAnnotation();
            simpleType.Annotation = annotation;

            XmlSchemaDocumentation schemaDoc = new XmlSchemaDocumentation();
            annotation.Items.Add(schemaDoc);
            XmlDocument doc = new XmlDocument();
            schemaDoc.Markup = new XmlNode[1] { doc.CreateTextNode(typeof(U).Name) };

            XmlSchemaSimpleTypeUnion union = new XmlSchemaSimpleTypeUnion();

            union.MemberTypes = new XmlQualifiedName[1];
            union.MemberTypes[0] = new XmlQualifiedName("ValueBoundType");
            foreach(string regex in Regexes) {
                XmlSchemaSimpleType regexType = new XmlSchemaSimpleType();
                XmlSchemaSimpleTypeRestriction regexTypeRestriction = new XmlSchemaSimpleTypeRestriction();
                XmlSchemaPatternFacet regexPattern = new XmlSchemaPatternFacet();
                regexTypeRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
                regexPattern.Value = regex;
                regexTypeRestriction.Facets.Add(regexPattern);
                regexType.Content = regexTypeRestriction;
                union.BaseTypes.Add(regexType);
            }
            IEnumerable<string> enumerations = GenerateEnumerations();
            if(enumerations != null) {
                XmlSchemaSimpleType enumerationType = new XmlSchemaSimpleType();
                XmlSchemaSimpleTypeRestriction enumerationTypeRestriction = new XmlSchemaSimpleTypeRestriction();
                enumerationTypeRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
                foreach(string enumeration in enumerations) {
                    XmlSchemaEnumerationFacet enumerationFacet = new XmlSchemaEnumerationFacet();
                    enumerationFacet.Value = enumeration;
                    enumerationTypeRestriction.Facets.Add(enumerationFacet);
                }
                enumerationType.Content = enumerationTypeRestriction;
                union.BaseTypes.Add(enumerationType);
            }
            simpleType.Content = union;
            return simpleType;
        }

        protected virtual IEnumerable<string> GenerateEnumerations() => null;
    }
}
