using System;
using System.Reflection;

namespace Orbit.Parser {
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using Util;

    public abstract class UIValue {
        public event Action OnChange;

        protected UIRenderData renderData;

        public virtual bool HasValue => renderData?.Host != null;

        public UIValue() { }
        public UIValue(UIRenderData renderData) {
            this.renderData = renderData;
        }

        public abstract void SetValue<T>(T value);
        public abstract object GetValue();
        public virtual T GetValue<T>() {
            try {
                if(typeof(T) == typeof(string))
                    return (T)(object)GetValue()?.ToString();
                return (T)GetValue();
            } catch {
                throw new Exception($"Error casting UIValue of type '{GetValue().GetType()}' to type '{typeof(T)}'");
            }
        }

        public abstract Type GetValueType();

        public void InvokeOnChange() => OnChange?.Invoke();
    }

    public class DefinedUIValue<TObjectType> : UIValue {
        public override bool HasValue => true;
        private TObjectType value;
        
        public DefinedUIValue(UIRenderData renderData, TObjectType value) : base(renderData) {
            this.value = value;
        }

        public override object GetValue() {
            return value;
        }

        public override void SetValue<T>(T value) {
            this.value = (TObjectType)(object)value;
            InvokeOnChange();
        }

        public override T GetValue<T>() {
            if(typeof(T) == typeof(string))
                return (T)(object)value.ToString();
            return (T)(object)value;
        }

        public override Type GetValueType() {
            return typeof(TObjectType);
        }
    }

    public class UIFieldValue : UIValue {
        private FieldInfo fieldInfo;
        
        public UIFieldValue(UIRenderData renderData, FieldInfo fieldInfo) : base(renderData){
            this.fieldInfo = fieldInfo;
        }

        public override object GetValue() {
            return fieldInfo.GetValue(renderData.Host);
        }

        public override void SetValue<T>(T value) {
            fieldInfo.SetValue(renderData.Host, value);
            InvokeOnChange();
        }

        public override Type GetValueType() {
            return fieldInfo.FieldType;
        }
    }

    public class UIPropertyValue : UIValue {
        private PropertyInfo propertyInfo;

        private MethodInfo getMethod;
        private MethodInfo GetMethod {
            get {
                if(getMethod == null)
                    getMethod = propertyInfo.GetGetMethod(true);
                return getMethod;
            }
        }

        private MethodInfo setMethod;
        private MethodInfo SetMethod {
            get {
                if(setMethod == null)
                    setMethod = propertyInfo.GetSetMethod(true);
                return setMethod;
            }
        }

        public UIPropertyValue(UIRenderData renderData, PropertyInfo propertyInfo) : base(renderData){
            this.propertyInfo = propertyInfo;
        }

        public override object GetValue() {
            return GetMethod.Invoke(renderData.Host, Array.Empty<object>());
        }
        
        public override void SetValue<T>(T value) {
            SetMethod?.Invoke(renderData.Host, ArrayParameters<object>.Single(value));
            InvokeOnChange();
        }

        public override Type GetValueType() {
            return propertyInfo.PropertyType;
        }
    }

    public class UIHostValue : UIValue {
        public UIHostValue(UIRenderData renderData) : base(renderData) { }

        public override object GetValue() {
            return renderData.Host;
        }

        public override void SetValue<T>(T value) {
            renderData.Host = value;
            InvokeOnChange();
        }

        public override Type GetValueType() {
            return renderData.Host.GetType();
        }
    }
}
