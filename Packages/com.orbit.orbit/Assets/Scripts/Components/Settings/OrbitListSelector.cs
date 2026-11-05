using TMPro;
using UnityEngine;

namespace Orbit.Components.Settings {
    using System;
    using System.Collections;

    public class OrbitListSelector : SettingComponent {
        [SerializeField]
        private TMP_Text valueText;

        private IList optionsList;
        public IList OptionsList {
            get => optionsList;
            set {
                optionsList = value;
                SetIndexToCurrentValue();
            }
        }
        private int currentIndex;
        
        private bool initialized = false;

        public override void PostParse() {
            if(UIValue == null) return;
            initialized = true;
            if(OptionsList == null && UIValue.GetValue() is Enum valueEnum) {
                OptionsList = Enum.GetValues(valueEnum.GetType());
            }
            
            SetIndexToCurrentValue();
            UpdateText();
        }

        public void Previous() {
            currentIndex--;
            if(currentIndex < 0) currentIndex = OptionsList.Count - 1;
            SetUIValue(currentIndex);
        }

        public void Next() {
            currentIndex++;
            if(currentIndex >= OptionsList.Count) currentIndex = 0;
            SetUIValue(currentIndex);
        }

        private void SetIndexToCurrentValue() {
            if(UIValue == null) return;
            currentIndex = OptionsList.IndexOf(UIValue.GetValue());
        }

        private void UpdateText() {
            object value = UIValue.GetValue();
            valueText.text = value == null ? "" : value.ToString();
        }

        private void SetUIValue(int index) {
            UIValue.SetValue(OptionsList[index]);
            OnChangeEvent?.Invoke();
        }

        protected override void OnUIValueAssigned() {
            if(!initialized) return;
            UpdateText();
        }

        protected override void OnValueChanged() {
            if(!initialized) return;
            UpdateText();
        }
    }
}