using Orbit.Components.ButtonLayoutGroup;
using Orbit.Components.Graphic;
using UnityEngine;

public class ButtonLayoutRectCornerUpdater : MonoBehaviour, IButtonLayoutGroupElement {
    [SerializeField] private RoundedRect roundedRect;
    [SerializeField] private float cornerRadius = 0;

    private float? originalTopLeftRadius;
    private float? originalTopRightRadius;
    private float? originalBottomLeftRadius;
    private float? originalBottomRightRadius;

    public void SetTop() {
        originalBottomLeftRadius ??= roundedRect.BottomLeftRadius;
        roundedRect.BottomLeftRadius = cornerRadius;
        originalBottomRightRadius ??= roundedRect.BottomRightRadius;
        roundedRect.BottomRightRadius = cornerRadius;
        if(originalTopLeftRadius.HasValue) {
            roundedRect.TopLeftRadius = originalTopLeftRadius.Value;
            originalTopLeftRadius = null;
        }
        if(originalTopRightRadius.HasValue) {
            roundedRect.TopRightRadius = originalTopRightRadius.Value;
            originalTopRightRadius = null;
        }
    }

    public void SetVerticalCenter() {
        SetHorizontalCenter();
    }

    public void SetBottom() {
        originalTopLeftRadius ??= roundedRect.TopLeftRadius;
        roundedRect.TopLeftRadius = cornerRadius;
        originalTopRightRadius ??= roundedRect.TopRightRadius;
        roundedRect.TopRightRadius = cornerRadius;
        if(originalBottomLeftRadius.HasValue) {
            roundedRect.BottomLeftRadius = originalBottomLeftRadius.Value;
            originalBottomLeftRadius = null;
        }
        if(originalBottomRightRadius.HasValue) {
            roundedRect.BottomRightRadius = originalBottomRightRadius.Value;
            originalBottomRightRadius = null;
        }
    }

    public void SetLeft() {
        originalBottomRightRadius ??= roundedRect.BottomRightRadius;
        roundedRect.BottomRightRadius = cornerRadius;
        originalTopRightRadius ??= roundedRect.TopRightRadius;
        roundedRect.TopRightRadius = cornerRadius;
        if(originalBottomLeftRadius.HasValue) {
            roundedRect.BottomLeftRadius = originalBottomLeftRadius.Value;
            originalBottomLeftRadius = null;
        }
        if(originalTopLeftRadius.HasValue) {
            roundedRect.TopLeftRadius = originalTopLeftRadius.Value;
            originalTopLeftRadius = null;
        }
    }

    public void SetHorizontalCenter() {
        originalBottomLeftRadius ??= roundedRect.BottomLeftRadius;
        roundedRect.BottomLeftRadius = cornerRadius;
        originalBottomRightRadius ??= roundedRect.BottomRightRadius;
        roundedRect.BottomRightRadius = cornerRadius;
        originalTopLeftRadius ??= roundedRect.TopLeftRadius;
        roundedRect.TopLeftRadius = cornerRadius;
        originalTopRightRadius ??= roundedRect.TopRightRadius;
        roundedRect.TopRightRadius = cornerRadius;
    }

    public void SetRight() {
        originalTopLeftRadius ??= roundedRect.TopLeftRadius;
        roundedRect.TopLeftRadius = cornerRadius;
        originalBottomLeftRadius ??= roundedRect.BottomLeftRadius;
        roundedRect.BottomLeftRadius = cornerRadius;
        if(originalTopRightRadius.HasValue) {
            roundedRect.TopRightRadius = originalTopRightRadius.Value;
            originalTopRightRadius = null;
        }
        if(originalBottomRightRadius.HasValue) {
            roundedRect.BottomRightRadius = originalBottomRightRadius.Value;
            originalBottomRightRadius = null;
        }
    }

    public void SetSingle() {
        if(originalTopLeftRadius.HasValue) {
            roundedRect.TopLeftRadius = originalTopLeftRadius.Value;
            originalTopLeftRadius = null;
        }
        if(originalTopRightRadius.HasValue) {
            roundedRect.TopRightRadius = originalTopRightRadius.Value;
            originalTopRightRadius = null;
        }
        if(originalBottomLeftRadius.HasValue) {
            roundedRect.BottomLeftRadius = originalBottomLeftRadius.Value;
            originalBottomLeftRadius = null;
        }
        if(originalBottomRightRadius.HasValue) {
            roundedRect.BottomRightRadius = originalBottomRightRadius.Value;
            originalBottomRightRadius = null;
        }
    }
}
