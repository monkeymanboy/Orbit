using TMPro;
using UnityEngine;

namespace Atlas.Orbit.Components.Settings {
    public class InputSetting : SettingComponent {
        [SerializeField]
        private TMP_InputField inputField;

        private bool initialized = false;

        public override void PostParse() {
            initialized = true;
            inputField.onValueChanged.AddListener(SetUIValue);
            UpdateInputText();
        }

        private void UpdateInputText() {
            inputField.SetTextWithoutNotify(UIValue.GetValue<string>());
        }

        private void SetUIValue(string val) {
            UIValue.SetValue(val);
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
    }
}