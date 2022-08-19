using Atlas.Orbit.Parser;
using System;
using UnityEngine;

namespace Atlas.Orbit.Components.Settings {
    public abstract class SettingComponent : MonoBehaviour {
        public Action OnChangeEvent { get; set; }
        private UIValue uiValue;
        public UIValue UIValue {
            get => uiValue;
            set {
                if(uiValue == value) return;
                if(uiValue != null)
                    uiValue.OnChange -= OnValueChanged;
                uiValue = value;
                uiValue.OnChange += OnValueChanged;
                OnUIValueAssigned();
            }
        }

        public abstract void PostParse();
        protected abstract void OnUIValueAssigned();
        protected abstract void OnValueChanged();
    }
}
