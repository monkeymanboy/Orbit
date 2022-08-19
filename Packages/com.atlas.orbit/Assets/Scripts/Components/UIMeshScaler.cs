using UnityEngine;

namespace Atlas.Orbit.Components {
    [ExecuteInEditMode]
    public class UIMeshScaler : MonoBehaviour {
        [SerializeField]
        private RectTransform meshTransform;
        [SerializeField]
        private RectTransform parentTransform;
        [SerializeField]
        private float scaleFactor = 1;

        private void OnRectTransformDimensionsChange() {
            if(meshTransform == null || parentTransform == null) return;
            Rect parentRect = parentTransform.rect;
            Rect meshRect = meshTransform.rect;
            meshTransform.localScale = new Vector3(parentRect.width * scaleFactor, parentRect.height * scaleFactor, 1);
        }
    }
}