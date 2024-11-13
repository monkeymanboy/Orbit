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
        private bool wholeNumbers = true;
        public bool WholeNumbers {
            get => wholeNumbers;
            set {
                wholeNumbers = value;
                UpdateSlider();
            }
        }

        public bool ImmediateUpdate { get; set; } = true;

        public UIFunction Formatter { get; set; }

        private bool initialized = false;
        private bool useInt = false;

        public override void PostParse() {
            initialized = true;
            if(ImmediateUpdate) {
                slider.onValueChanged.AddListener(SetUIValue);
            } else {
                slider.gameObject.AddComponent<SliderPointerUp>().SetOnFinished(SetUIValue);
                slider.onValueChanged.AddListener(UpdateText);
            }
            switch(UIValue.GetValue()) {
                case int:
                    useInt = true;
                    break;
                case float:
                    useInt = false;
                    break;
            }
            UpdateSlider();
        }

        private void UpdateSlider() {
            if(!initialized) return;
            slider.minValue = 0;
            slider.maxValue = increments;
            slider.wholeNumbers = WholeNumbers;
            UpdateSliderValue();
        }

        private void UpdateSliderValue() {
            float value = useInt
                ? (float)(UIValue.GetValue<int>())
                : UIValue.GetValue<float>();

            value = WholeNumbers
                ? Mathf.RoundToInt(((value - MinValue) / (MaxValue - MinValue)) * Increments)
                : Mathf.Clamp(value - MinValue, 0, MaxValue);

            slider.SetValueWithoutNotify(value);

            UpdateText(slider.value);
        }

        public void Increment(){
            slider.value = Mathf.RoundToInt((Mathf.Clamp(slider.value + 1, 0f, wholeNumbers ? increments : MaxValue)));
        }

        private void UpdateText(float val) {
            if(!valueMesh) return;
            if(useInt) valueMesh.text = (Formatter?.Invoke(Mathf.RoundToInt(GetRealValue(val))) as string) ?? Mathf.RoundToInt(GetRealValue(val)).ToString("0");
            else valueMesh.text = (Formatter?.Invoke(GetRealValue(val)) as string) ?? GetRealValue(val).ToString("0.00");
        }
        private float GetRealValue(float val) {
            return (val / increments) * (MaxValue - MinValue) + MinValue;
        }

        private void SetUIValue(float val) {
            if(useInt) UIValue.SetValue(Mathf.RoundToInt(GetRealValue(val)));
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
