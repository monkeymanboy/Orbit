using UnityEngine;

namespace Orbit.Components.Settings {
    public class OrbitLabelWidthAdjuster : MonoBehaviour {
        [SerializeField] private RectTransform labelTransform;
        [SerializeField] private RectTransform otherTransform;

        public float LabelWidth {
            set {
                Vector2 labelSizeDelta = labelTransform.sizeDelta;
                labelSizeDelta.x = value;
                labelTransform.sizeDelta = labelSizeDelta;
                Vector2 otherSizeDelta = otherTransform.sizeDelta;
                otherSizeDelta.x = -value;
                otherTransform.sizeDelta = otherSizeDelta;
            }
        }
    }
}