using TMPro;
using UnityEngine;

namespace Atlas.Orbit.Components.Settings {
    using System;
    using System.Collections.Generic;

    public class DropdownSetting : SettingComponent {
        [SerializeField]
        private TMP_Dropdown dropdown;

        public List<TMP_Dropdown.OptionData> Options { get; set; }
        private Array enumValues;
        
        private bool initialized = false;

        public override void PostParse() {
            initialized = true;
            if(Options == null && UIValue.GetValue() is Enum valueEnum) {
                Options = new();
                enumValues = Enum.GetValues(valueEnum.GetType());
                foreach(object option in enumValues) {
                    Options.Add(new TMP_Dropdown.OptionData(option.ToString()));
                }
            }
            dropdown.ClearOptions();
            dropdown.AddOptions(Options);
            dropdown.onValueChanged.AddListener(SetUIValue);
            UpdateDropdownValue();
        }

        private void UpdateDropdownValue() {
            if(enumValues != null) 
                dropdown.SetValueWithoutNotify(Array.IndexOf(enumValues, UIValue.GetValue()));
            else 
                dropdown.SetValueWithoutNotify(Options.IndexOf(UIValue.GetValue<TMP_Dropdown.OptionData>()));
        }

        private void SetUIValue(int index) {
            if(enumValues != null) 
                UIValue.SetValue(enumValues.GetValue(index));
            else 
                UIValue.SetValue(Options[index]);
            OnChangeEvent?.Invoke();
        }

        protected override void OnUIValueAssigned() {
            if(!initialized) return;
            UpdateDropdownValue();
        }

        protected override void OnValueChanged() {
            if(!initialized) return;
            UpdateDropdownValue();
        }
    }
}