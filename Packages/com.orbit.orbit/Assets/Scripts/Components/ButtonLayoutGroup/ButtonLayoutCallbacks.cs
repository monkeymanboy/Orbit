using UnityEngine;

namespace Orbit.Components.ButtonLayoutGroup {
    using UnityEngine.Events;

    /// <summary>
    /// This component will create alternate sprites for a FullRect sprite in order to cut off edges while maintaining batching
    /// </summary>
    public class ButtonLayoutCallbacks : MonoBehaviour, IButtonLayoutGroupElement {
        [SerializeField] private UnityEvent onTop;
        [SerializeField] private UnityEvent onVerticalCenter;
        [SerializeField] private UnityEvent onBottom;
        [SerializeField] private UnityEvent onLeft;
        [SerializeField] private UnityEvent onHorizontalCenter;
        [SerializeField] private UnityEvent onRight;
        [SerializeField] private UnityEvent onSingle;

        public void SetTop() => onTop.Invoke();
        public void SetVerticalCenter() => onVerticalCenter.Invoke();
        public void SetBottom() => onBottom.Invoke();
        public void SetLeft() => onLeft.Invoke();
        public void SetHorizontalCenter() => onHorizontalCenter.Invoke();
        public void SetRight() => onRight.Invoke();
        public void SetSingle() => onSingle.Invoke();
    }
}
