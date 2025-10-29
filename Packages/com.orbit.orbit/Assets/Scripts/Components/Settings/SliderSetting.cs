using Orbit.Parser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Orbit.Components.Settings {
    using Parser;

    public class SliderSetting : SettingComponent {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private TextMeshProUGUI valueMesh;

        private int increments = -1;
        public int Increments {
            get => increments;
            set {
                increments = value;
                if(increments == 0) increments = -1; //Allows 0 increments to mean stepless
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
            slider.maxValue = Mathf.Abs(increments);
            slider.wholeNumbers = increments != -1;
            UpdateSliderValue();
        }

        private void UpdateSliderValue(bool notify = false) {
            float value = useInt
                ? (float)(UIValue.GetValue<int>())
                : UIValue.GetValue<float>();
            
            
            value = increments != -1
                ? Mathf.RoundToInt(((value - MinValue) / (MaxValue - MinValue)) * increments)
                : Mathf.InverseLerp(MinValue, MaxValue, value);
            //value = Mathf.RoundToInt(((value - MinValue) / (MaxValue - MinValue)) * Mathf.Abs(Increments));

            if(notify)
                slider.value = value;
            else 
                slider.SetValueWithoutNotify(value);

            UpdateText(slider.value);
        }

        private void UpdateText(float val) {
            if(!valueMesh) return;
            if(useInt) valueMesh.text = (Formatter?.Invoke(Mathf.RoundToInt(GetRealValue(val))) as string) ?? Mathf.RoundToInt(GetRealValue(val)).ToString("0");
            else valueMesh.text = (Formatter?.Invoke(GetRealValue(val)) as string) ?? GetRealValue(val).ToString("0.00");
        }
        private float GetRealValue(float val) {
            return (val / Mathf.Abs(increments)) * (MaxValue - MinValue) + MinValue;
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
            UpdateSliderValue(NotifyValueChanged);
        }
    }
}
