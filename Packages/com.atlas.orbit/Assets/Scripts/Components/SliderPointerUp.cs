namespace Atlas.Orbit.Components {
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Slider))]
    public class SliderPointerUp : MonoBehaviour, IPointerUpHandler {
        public event Action<float> OnFinished;

        private Slider slider;

        private void Awake() {
            slider = GetComponent<Slider>();
        }

        public void OnPointerUp(PointerEventData eventData) {
            OnFinished?.Invoke(slider.value);
        }

        public void SetOnFinished(Action<float> onFinished) {
            OnFinished = onFinished;
        }
    }
}