using Atlas.Orbit.Parser;
using UnityEngine;

namespace Atlas.Orbit.Components {
    public class OrbitList : RepeatingList {
        [SerializeField]
        public int visibleItems;
        protected override int VisibleItems => visibleItems;

        [SerializeField]
        public float spacing = -1;
        public float Spacing => spacing < 0 ? (360f / visibleItems) : spacing;

        [SerializeField]
        public float extraMiddleSpace = 10;

        [SerializeField]
        public Transform cellContainer;
        protected override Transform CellContainer => cellContainer;

        protected override bool CanSnap => true;

        public UIFunction OnCellCentered { get; set; }

        private void Awake() {
            if(cellContainer == null)
                cellContainer = transform;
        }

        protected override void UpdateChildPosition(ListItem child, int rowIdx) {
            float rotation = Spacing * rowIdx - 90;
            float scroll = middleIndex + extraScroll;
            if(IsGrabbed)
                scroll = middleIndex;
            int index = Mathf.RoundToInt(scroll);
            if(rowIdx < index)
                rotation -= extraMiddleSpace / 2;
            if(rowIdx > index)
                rotation += extraMiddleSpace / 2;
            if(scroll > index) {
                float t = Mathf.InverseLerp(index, index + 1, scroll);
                if(rowIdx == index) {
                    rotation -= t * extraMiddleSpace / 2;
                    child.SetExpansionFactor(1-t);
                    child.SetFocused(true);
                } else {
                    child.SetExpansionFactor(0);
                    child.SetFocused(false);
                }
            } else {
                float t = Mathf.InverseLerp(index, index - 1, scroll);
                if(rowIdx == index) {
                    rotation += t * extraMiddleSpace / 2;
                    child.SetExpansionFactor(1-t);
                    child.SetFocused(true);
                } else {
                    child.SetExpansionFactor(0);
                    child.SetFocused(false);
                }
            }
            /*
            if(extraScroll > 0 && extraScroll < 1) {
                if(rowIdx == middleIndex - 1)
                    rotation -= (1 - extraScroll) * extraMiddleSpace / 2;
                if(rowIdx == middleIndex)
                    rotation -= (extraScroll) * extraMiddleSpace / 2;

            }*/
            child.transform.localEulerAngles = new Vector3(0, 0, rotation);
        }

        protected override void Update() {
            base.Update();

            cellContainer.localEulerAngles = new Vector3(0, 0, -(Spacing * (middleIndex + extraScroll) + 90));
            if(extraMiddleSpace > 0) {
                foreach(ListItem item in childBuffer) {
                    if(item == null)
                        continue;
                    UpdateChildPosition(item, item.Index);
                }
            }
            ReorganiseContent(false);
        }

        protected override void OnCreatePrefab(ListItem listItem) {
            listItem.OnFocusCell += ClickScrollToRow;
        }

        protected override void OnIndexCentered(int index) {
            OnCellCentered?.Invoke(Hosts[index]);
        }
    }
}
