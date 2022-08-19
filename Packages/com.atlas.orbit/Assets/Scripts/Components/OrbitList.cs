using Atlas.Orbit.Parser;
using UnityEngine;
using UnityEngine.EventSystems;

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

        protected override bool CanSnap => activeSpinTransform == null;

        [SerializeField]
        private Collider spinCollider;

        private bool isFirstFrame = true;
        float lastAngle;
        private Transform activeSpinTransform;

        public UIFunction OnCellCentered { get; set; }
        public bool ExpandHoverEffect { get; set; }

        private void Awake() {
            if(cellContainer == null)
                cellContainer = transform;

            /*
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.InitializePotentialDrag;
            pointerDown.callback.AddListener((data) => {
                PointerEventData pointerData = data as PointerEventData;
                Debug.Log(pointerData.position);
                Debug.Log(pointerData.pressPosition);
            });
            GetComponent<EventTrigger>().triggers.Add(pointerDown);*/
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
            //TODO(David): Not a big fan of this way of doing input, would be a good idea to switch out to somehing better

            /*
            if(ControllerCache.Left.uiPressInteractionState.activatedThisFrame) {
                isFirstFrame = true;
                activeSpinTransform = ControllerCache.Left.transform;
            }
            if(ControllerCache.Right.uiPressInteractionState.activatedThisFrame) {
                isFirstFrame = true;
                activeSpinTransform = ControllerCache.Right.transform;
            }
            if(!ControllerCache.Left.uiPressInteractionState.active && !ControllerCache.Right.uiPressInteractionState.active)
                activeSpinTransform = null;
            if(activeSpinTransform != null && Physics.Raycast(activeSpinTransform.position, activeSpinTransform.transform.forward, out RaycastHit hit)) {
                Vector3 hitPos = transform.InverseTransformPoint(hit.point);
                float angle = Mathf.Atan2(hitPos.y, hitPos.x);
                angle = (angle / Mathf.PI) * 180 + cellContainer.localEulerAngles.z;
                if(!isFirstFrame) {
                    float angleDelta = Mathf.DeltaAngle(angle, lastAngle);
                    velocity = 10* angleDelta / (360f / visibleItems);
                }
                isFirstFrame = false;
                lastAngle = angle;
            } else {
                if(isFirstFrame) {
                    activeSpinTransform = null;
                    isFirstFrame = false;
                }
            }*/

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
            (listItem.transform as RectTransform).pivot = Vector2.zero;
            (listItem.transform as RectTransform).sizeDelta = Vector2.zero;


            Canvas canvas = listItem.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
            
            if(ExpandHoverEffect) {
                EventTrigger eventTrigger = listItem.transform.gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
                pointerEnter.eventID = EventTriggerType.PointerEnter;
                pointerEnter.callback.AddListener((data) => {
                    listItem.ScaleTo(1.05f);
                });
                eventTrigger.triggers.Add(pointerEnter);
                EventTrigger.Entry pointerExit = new EventTrigger.Entry();
                pointerExit.eventID = EventTriggerType.PointerExit;
                pointerExit.callback.AddListener((data) => {
                    listItem.ScaleTo(1f);
                });
                eventTrigger.triggers.Add(pointerExit);
            }
        }

        protected override void OnIndexCentered(int index) {
            OnCellCentered?.Invoke(Hosts[index]);
        }
    }
}
