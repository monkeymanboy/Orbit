using System;
using System.Reflection;

namespace Atlas.Orbit.Parser {
    public abstract class UIValue {
        public event Action OnChange;

        protected UIRenderData renderData;

        public virtual bool HasValue => renderData?.Host != null;

        public UIValue(UIRenderData renderData) {
            this.renderData = renderData;
        }

        public abstract void SetValue(object value);
        public abstract object GetValue();
        public T GetValue<T>() {
            try {
                return (T)GetValue();
            } catch {
                throw new Exception($"Error casting UIValue of type '{GetValue().GetType()}' to type '{typeof(T)}'");
            }
        }

        public void InvokeOnChange() => OnChange?.Invoke();
    }

    public class DefinedUIValue : UIValue {
        public override bool HasValue => true;
        private object value;
        
        public DefinedUIValue(UIRenderData renderData, object value) : base(renderData) {
            this.value = value;
        }

        public override object GetValue() {
            return value;
        }

        public override void SetValue(object value) {
            this.value = value;
            InvokeOnChange();
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

        public override void SetValue(object value) {
            fieldInfo.SetValue(renderData.Host, value);
            InvokeOnChange();
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
        
        public override void SetValue(object value) {
            SetMethod?.Invoke(renderData.Host, new object[] { value });
            InvokeOnChange();
        }
    }

    public class UIHostValue : UIValue {
        public UIHostValue(UIRenderData renderData) : base(renderData) { }

        public override object GetValue() {
            return renderData.Host;
        }

        public override void SetValue(object value) {
            renderData.Host = value;
            InvokeOnChange();
        }
    }
}
