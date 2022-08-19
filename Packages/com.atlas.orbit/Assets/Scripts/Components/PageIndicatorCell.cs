using System;
using UnityEngine;
using UnityEngine.UI;

namespace Atlas.Orbit.Components {
    public class PageIndicatorCell : MonoBehaviour {
        public event Action<int> OnClick;

        public int Index { get; set; }

        [SerializeField]
        private Image outline;

        [SerializeField]
        private Image fill;

        public void SetColor(Color color) {
            outline.color = color;
        }

        public void SetSelected(bool selected) {
            fill.gameObject.SetActive(selected);
        }

        public void ButtonClicked() {
            OnClick?.Invoke(Index);
        }
    }
}
