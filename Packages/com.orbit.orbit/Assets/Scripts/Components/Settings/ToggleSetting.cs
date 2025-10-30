using Orbit.Parser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Orbit.Components.Settings {
    public class ToggleSetting : SettingComponent {
        [SerializeField]
        private Toggle toggle;

        private bool initialized = false;

        public override void PostParse() {
            if(UIValue == null) return;
            initialized = true;
            toggle.onValueChanged.AddListener(SetUIValue);
            UpdateToggleValue();
        }

        private void UpdateToggleValue(bool notify = false) {
            if(notify)
                toggle.isOn = UIValue.GetValue<bool>();
            else 
                toggle.SetIsOnWithoutNotify(UIValue.GetValue<bool>());
        }

        private void SetUIValue(bool val) {
            UIValue.SetValue(val);
            OnChangeEvent?.Invoke();
        }

        protected override void OnUIValueAssigned() {
            if(!initialized) return;
            UpdateToggleValue();
        }

        protected override void OnValueChanged() {
            if(!initialized) return;
            UpdateToggleValue(NotifyValueChanged);
        }
    }
}
