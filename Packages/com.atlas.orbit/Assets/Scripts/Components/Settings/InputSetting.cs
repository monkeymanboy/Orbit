using TMPro;
using UnityEngine;

namespace Atlas.Orbit.Components.Settings {
    using System;

    public class InputSetting : SettingComponent {
        public Action OnEndEditEvent { get; set; }
        
        [SerializeField]
        private TMP_InputField inputField;

        private bool initialized = false;
        private ValueType valueType;

        public override void PostParse() {
            initialized = true;

            valueType = ValueType.STRING;
            
            object value = UIValue.GetValue();
            switch(value) {
                case int:
                    inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                    valueType = ValueType.INT;
                    break;
                case float:
                    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    valueType = ValueType.FLOAT;
                    break;
            }
            
            inputField.onValueChanged.AddListener(SetUIValue);
            UpdateInputText();
        }

        public void EndEdit(string text) {
            OnEndEditEvent?.Invoke();
        }

        private void UpdateInputText() {
            switch(valueType) {
                case ValueType.INT:
                    inputField.SetTextWithoutNotify(UIValue.GetValue<int>().ToString());
                    break;
                case ValueType.FLOAT:
                    inputField.SetTextWithoutNotify(UIValue.GetValue<float>().ToString());
                    break;
                case ValueType.STRING:
                    inputField.SetTextWithoutNotify(UIValue.GetValue<string>());
                    break;
            }
        }

        private void SetUIValue(string val) {
            switch(valueType) {
                case ValueType.INT:
                    UIValue.SetValue(int.Parse(val));
                    break;
                case ValueType.FLOAT:
                    UIValue.SetValue(float.Parse(val));
                    break;
                case ValueType.STRING:
                    UIValue.SetValue(val);
                    break;
            }
            OnChangeEvent?.Invoke();
        }

        protected override void OnUIValueAssigned() {
            if(!initialized) return;
            UpdateInputText();
        }

        protected override void OnValueChanged() {
            if(!initialized) return;
            UpdateInputText();
        }

        public enum ValueType {
            STRING, FLOAT, INT
        }
    }
}