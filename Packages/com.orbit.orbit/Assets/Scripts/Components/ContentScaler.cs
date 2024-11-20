using UnityEngine;

namespace Orbit.Components {
    [RequireComponent(typeof(RectTransform))]
    public class ContentScaler : MonoBehaviour {
        private float contentScale = 1;
        public float ContentScale {
            get => contentScale;
            set {
                contentScale = value;
                UpdateScale();
            }
        }

        private RectTransform rectTransform;

        private void Start() {
            rectTransform = transform as RectTransform;
            UpdateScale();
        }

        private void UpdateScale() {
            if(rectTransform == null) return;
            rectTransform.localScale = new Vector3(contentScale, contentScale, contentScale);
            float anchorExtra = ((1 / contentScale) - 1) / 2;
            rectTransform.anchorMin = new Vector2(-anchorExtra, -anchorExtra);
            rectTransform.anchorMax = new Vector2(1 + anchorExtra, 1 + anchorExtra);
        }
    }

}