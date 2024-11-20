using Orbit.TypeSetters;
using System.Collections.Generic;
using UnityEngine;

namespace Orbit.ComponentProcessors {
    using TypeSetters;

    public class RectTransformProcessor : ComponentProcessor<RectTransform> {
        public enum AnchorPreset {
            FillParent,
            LeftAlign,
            RightAlign,
            TopAlign,
            BottomAlign,
            TopLeftAlign,
            BottomLeftAlign,
            TopRightAlign,
            BottomRightAlign,
            Center,
            LeftHalf,
            RightHalf,
            TopHalf,
            BottomHalf
        }

        public override Dictionary<string, TypeSetter<RectTransform>> Setters => new() {
            {"AnchorPreset", new EnumSetter<RectTransform, AnchorPreset>(ApplyAnchorPreset) },
            {"AnchorMin", new Vector2Setter<RectTransform>((component, value) => component.anchorMin = value) },
            {"AnchorMinX", new FloatSetter<RectTransform>((component, value) => component.anchorMin = new Vector2(value, component.anchorMin.y)) },
            {"AnchorMinY", new FloatSetter<RectTransform>((component, value) => component.anchorMin = new Vector2(component.anchorMin.x, value)) },
            {"AnchorMax", new Vector2Setter<RectTransform>((component, value) => component.anchorMax = value) },
            {"AnchorMaxX", new FloatSetter<RectTransform>((component, value) => component.anchorMax = new Vector2(value, component.anchorMax.y)) },
            {"AnchorMaxY", new FloatSetter<RectTransform>((component, value) => component.anchorMax = new Vector2(component.anchorMax.x, value)) },
            {"AnchorPos", new Vector2Setter<RectTransform>((component, value) => component.anchoredPosition = value) },
            {"AnchorPosX", new FloatSetter<RectTransform>((component, value) => component.anchoredPosition = new Vector2(value, component.anchoredPosition.y)) },
            {"AnchorPosY", new FloatSetter<RectTransform>((component, value) => component.anchoredPosition = new Vector2(component.anchoredPosition.x, value)) },
            {"SizeDelta", new Vector2Setter<RectTransform>((component, value) => component.sizeDelta = value) },
            {"SizeDeltaX", new FloatSetter<RectTransform>((component, value) => component.sizeDelta = new Vector2(value, component.sizeDelta.y)) },
            {"SizeDeltaY", new FloatSetter<RectTransform>((component, value) => component.sizeDelta = new Vector2(component.sizeDelta.x, value)) },
            {"Pivot", new Vector2Setter<RectTransform>((component, value) => component.pivot = value) },
            {"PivotX", new FloatSetter<RectTransform>((component, value) => component.pivot = new Vector2(value, component.pivot.y)) },
            {"PivotY", new FloatSetter<RectTransform>((component, value) => component.pivot = new Vector2(component.pivot.x, value)) },
        };

        private void ApplyAnchorPreset(RectTransform rectTransform, AnchorPreset preset) {
            switch(preset) {
                case AnchorPreset.FillParent:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    break;
                case AnchorPreset.LeftAlign:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    rectTransform.pivot = new Vector2(0, 0.5f);
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
                    break;
                case AnchorPreset.RightAlign:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(1, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(1, 0.5f);
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
                    break;
                case AnchorPreset.TopAlign:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.y);
                    break;
                case AnchorPreset.BottomAlign:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    rectTransform.pivot = new Vector2(0.5f, 0);
                    rectTransform.sizeDelta = new Vector2(0, rectTransform.sizeDelta.y);
                    break;
                case AnchorPreset.Center:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.TopLeftAlign:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    rectTransform.pivot = new Vector2(0, 1);
                    break;
                case AnchorPreset.BottomLeftAlign:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(0, 0);
                    rectTransform.pivot = new Vector2(0, 0);
                    break;
                case AnchorPreset.TopRightAlign:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(1, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(1, 1);
                    break;
                case AnchorPreset.BottomRightAlign:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(1, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    rectTransform.pivot = new Vector2(1, 0);
                    break;
                case AnchorPreset.LeftHalf:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 1);
                    rectTransform.pivot = new Vector2(0, 0.5f);
                    break;
                case AnchorPreset.RightHalf:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0.5f, 0);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(1, 0.5f);
                    break;
                case AnchorPreset.BottomHalf:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 0.5f);
                    rectTransform.pivot = new Vector2(0.5f, 0);
                    break;
                case AnchorPreset.TopHalf:
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.anchorMin = new Vector2(0, 0.5f);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    break;
            }
        }
    }
}
