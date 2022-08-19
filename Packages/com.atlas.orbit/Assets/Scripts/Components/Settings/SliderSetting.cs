using Atlas.Orbit.Parser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atlas.Orbit.Components.Settings {
    public class SliderSetting : SettingComponent {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private TextMeshProUGUI valueMesh;

        private int increments = 1;
        public int Increments {
            get => increments;
            set {
                increments = value;
                UpdateSlider();
            }
        }
        private float maxValue = 0;
        public float MaxValue {
            get => maxValue;
            set {
                maxValue = value;
                UpdateSlider();
            }
        }
        private float minValue = 0;
        public float MinValue {
            get => minValue;
            set {
                minValue = value;
                UpdateSlider();
            }
        }
        private bool useInt = false;
        public bool UseInt {
            get => useInt;
            set {
                useInt = value;
                UpdateSlider();
            }
        }

        public UIFunction Formatter { get; set; }

        private bool initialized = false;

        public override void PostParse() {
            initialized = true;
            slider.onValueChanged.AddListener(SetUIValue);
            UpdateSlider();
        }

        private void UpdateSlider() {
            if(!initialized) return;
            slider.minValue = 0;
            slider.maxValue = increments;
            slider.wholeNumbers = true;
            UpdateSliderValue();
        }

        private void UpdateSliderValue() {
            if(UseInt) slider.SetValueWithoutNotify(Mathf.RoundToInt(((UIValue.GetValue<int>() - MinValue) / (MaxValue - MinValue)) * Increments));
            else slider.SetValueWithoutNotify(Mathf.RoundToInt(((UIValue.GetValue<float>() - MinValue) / (MaxValue - MinValue)) * Increments));
            UpdateText(slider.value);
        }

        public void Increment(){
            slider.value = Mathf.RoundToInt((Mathf.Clamp(slider.value + 1, 0f, increments)));
        }

        private void UpdateText(float val) {
            if(!valueMesh) return;
            if(UseInt) valueMesh.text = (Formatter?.Invoke(Mathf.RoundToInt(GetRealValue(val))) as string) ?? Mathf.RoundToInt(GetRealValue(val)).ToString("0");
            else valueMesh.text = (Formatter?.Invoke(GetRealValue(val)) as string) ?? GetRealValue(val).ToString("0.00");
        }
        private float GetRealValue(float val) {
            return (val / increments) * (MaxValue - MinValue) + MinValue;
        }

        private void SetUIValue(float val) {
            if(UseInt) UIValue.SetValue(Mathf.RoundToInt(GetRealValue(val)));
            else UIValue.SetValue(GetRealValue(val));
            OnChangeEvent?.Invoke();
        }

        protected override void OnUIValueAssigned() {
            if(!initialized) return;
            UpdateSliderValue();
        }

        protected override void OnValueChanged() {
            if(!initialized) return;
            UpdateSliderValue();
        }
    }
}
