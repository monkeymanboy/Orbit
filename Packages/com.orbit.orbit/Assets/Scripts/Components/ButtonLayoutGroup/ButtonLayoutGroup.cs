using UnityEngine.UI;

namespace Orbit.Components.ButtonLayoutGroup {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ButtonLayoutGroup : HorizontalOrVerticalLayoutGroup {
        private List<IButtonLayoutGroupElement> elements = new();
        
        private bool isVertical;
        public bool IsVertical {
            get => isVertical;
            set {
                isVertical = value;
            }
        }

        protected override void Awake() {
            base.Awake();
            m_ChildControlHeight = true;
            m_ChildControlWidth = true;
            m_ChildForceExpandHeight = true;
            m_ChildForceExpandWidth = true;
        }

        public override void CalculateLayoutInputHorizontal() {
            base.CalculateLayoutInputHorizontal();
            CalcAlongAxis(0, isVertical);
        }

        public override void CalculateLayoutInputVertical() {
            CalcAlongAxis(1, isVertical);
        }

        public override void SetLayoutHorizontal() {
            SetChildrenAlongAxis(0, isVertical);
        }

        public override void SetLayoutVertical() {
            int startIndex = m_ReverseArrangement ? rectChildren.Count - 1 : 0;
            int endIndex = m_ReverseArrangement ? 0 : rectChildren.Count - 1;
            int increment = m_ReverseArrangement ? -1 : 1;
            if(startIndex == endIndex) {
                rectChildren[0].GetComponent<IButtonLayoutGroupElement>()?.SetSingle();
                return;
            }

            for(int i = startIndex;m_ReverseArrangement ? i >= endIndex : i <= endIndex;i += increment) {
                elements.Clear();
                rectChildren[i].GetComponents(elements);
                for (int elementIndex = 0; elementIndex < elements.Count; elementIndex++)
                {
                    IButtonLayoutGroupElement spriteSwapper = elements[elementIndex];
                    if(isVertical) {
                        if(i == startIndex)
                            spriteSwapper.SetBottom();
                        else if(i == endIndex)
                            spriteSwapper.SetTop();
                        else
                            spriteSwapper.SetVerticalCenter();
                    } else {
                        if(i == startIndex)
                            spriteSwapper.SetLeft();
                        else if(i == endIndex)
                            spriteSwapper.SetRight();
                        else
                            spriteSwapper.SetHorizontalCenter();
                    }   
                }
            }
            SetChildrenAlongAxis(1, isVertical);
        }
    }
}