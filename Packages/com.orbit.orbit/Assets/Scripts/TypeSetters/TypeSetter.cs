using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Orbit.TypeSetters {
    using Exceptions;
    using Parser;
    using UnityEngine;

    public abstract class TypeSetter<T> {
        public abstract void SetFromResource(T obj, string resourcePath);
        public abstract void SetFromResource(ref T obj, string resourcePath);
        public abstract void SetFromString(T obj, string value);
        public abstract void SetFromString(ref T obj, string value);
        public abstract void Set<SetType>(T obj, SetType value);
        public abstract void Set(T obj, UIValue value);
        public abstract void Set<SetType>(ref T obj, SetType value);
        public abstract void Set(ref T obj, UIValue value);

        public abstract XmlSchemaSimpleType GenerateSchemaType();
    }

    public abstract class TypeSetter<T, U> : TypeSetter<T> {
        protected virtual string[] Regexes => Array.Empty<string>();
        public Action<T, U> Setter { get; private set; }
        public ActionRef<T, U> RefSetter { get; private set; }
        public delegate void ActionRef<T, U>(ref T item, U value);
        
        public TypeSetter(Action<T, U> setter) {
            Setter = setter;
        }
        public TypeSetter(ActionRef<T, U> refSetter) {
            RefSetter = refSetter;
        }

        public override void SetFromResource(T obj, string resourcePath) {
            Setter(obj, (U)(object)Resources.Load(resourcePath, typeof(U)));
        }
        public override void SetFromResource(ref T obj, string resourcePath) {
            RefSetter(ref obj, (U)(object)Resources.Load(resourcePath, typeof(U)));
        }

        public override void SetFromString(T obj, string value) {
            U parsedValue;
            try {
                parsedValue = Parse(value);
            } catch (Exception ex){
                throw ex is ParseValueException ? ex : new ParseValueException(typeof(U), value, ex.Message);
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

        public override void Set<SetType>(T obj, SetType value) => Setter(obj, (U)(object)value);
        public override void Set(T obj, UIValue value) => Set(obj, value.GetValue<U>());

        public override void Set<SetType>(ref T obj, SetType value) => RefSetter(ref obj, (U)(object)value);
        public override void Set(ref T obj, UIValue value) => Set(ref obj, value.GetValue<U>());

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
            union.MemberTypes[0] = new XmlQualifiedName("BoundType");
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
