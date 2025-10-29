using TMPro;
using UnityEngine;

namespace Orbit.Components.Settings {
    using System;

    public class InputSetting : SettingComponent {
        public Action OnEndEditEvent { get; set; }
        
        [SerializeField]
        private TMP_InputField inputField;

        private bool initialized = false;
        private ValueType valueType;

        public override void PostParse() {
            initialized = true;

            valueType = ValueType.String;
            
            object value = UIValue.GetValue();
            switch(value) {
                case int:
                    inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                    valueType = ValueType.Int;
                    break;
                case float:
                    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    valueType = ValueType.Float;
                    break;
            }
            
            inputField.onValueChanged.AddListener(SetUIValue);
            UpdateInputText();
        }

        public void EndEdit(string text) {
            OnEndEditEvent?.Invoke();
        }

        private void UpdateInputText(bool notify = false) {
            string text;
            switch(valueType) {
                case ValueType.Int:
                    text = UIValue.GetValue<int>().ToString();
                    break;
                case ValueType.Float:
                    text = UIValue.GetValue<float>().ToString();
                    break;
                case ValueType.String:
                    text = UIValue.GetValue<string>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if(notify)
                inputField.text = text;
            else
                inputField.SetTextWithoutNotify(text);
        }

        private void SetUIValue(string val) {
            switch(valueType) {
                case ValueType.Int:
                    UIValue.SetValue(int.Parse(val));
                    break;
                case ValueType.Float:
                    UIValue.SetValue(float.Parse(val));
                    break;
                case ValueType.String:
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
            UpdateInputText(NotifyValueChanged);
        }

        public enum ValueType {
            String, Float, Int
        }
    }
}