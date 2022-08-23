using Atlas.Orbit.Parser;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Atlas.Orbit.Components {
    public abstract class RepeatingList : MonoBehaviour {
        public UIRenderData ParentData { get; internal set; }
        public List<object> Hosts { get; set; }
        public XmlNode ItemXml { get; set; }

        public int ItemCount => Hosts.Count;
        protected abstract int VisibleItems { get; }
        protected abstract Transform CellContainer { get; }
        protected abstract bool CanSnap { get; }
        public bool ScrollLocked { get; set; }
        public bool IsGrabbed { get; set; }


        protected ListItem[] childBuffer;
        protected int childBufferStartIndex = 0;
        protected int hostStartIndex;

        protected const int rowsAboveBelow = 0;

        [SerializeField] protected ListItem cellPrefab;
        
        [SerializeField]
        protected int middleIndex;
        [SerializeField]
        protected float extraScroll;

        private int lastCenteredIndex = -1;

        private bool isApproachingTarget;
        private int targetMiddleIndex;

        [SerializeField]
        protected float friction = 3f;
        protected float velocity = 0;

        [SerializeField]
        protected float snapSpeed = 10;

        private int middleLastFrame = 0;

        public virtual void Refresh() {
            ReorganiseContent(true);
        }

        public virtual void ScrollToRow(int row) {
            isApproachingTarget = true;
            targetMiddleIndex = row;
            OnIndexCentered(WrapItemIndex(row));
        }

        public virtual void ClickScrollToRow(int row) {
            if(ScrollLocked)
                return;
            isApproachingTarget = true;
            targetMiddleIndex = row;
            OnIndexCentered(WrapItemIndex(row));
        }

        protected virtual bool CheckChildItems() {
            
            int childCount = VisibleItems + rowsAboveBelow * 2;

            if((childBuffer != null && childBuffer.Length == childCount))
                return false;
            
            if(childBuffer == null)
                childBuffer = new ListItem[childCount];
            else if(childCount > childBuffer.Length)
                Array.Resize(ref childBuffer, childCount);

            for(int i = 0;i < childBuffer.Length;++i) {
                if(childBuffer[i] == null) {
                    childBuffer[i] = CreatePrefab();
                }
            }

            return true;
        }

        protected ListItem CreatePrefab() {
            ListItem item = Instantiate(cellPrefab, CellContainer, false);
            OnCreatePrefab(item);
            item.Parse(ParentData.Parser, ItemXml, Hosts[0], ParentData);
            item.gameObject.SetActive(false);
            return item;
        }

        protected abstract void OnCreatePrefab(ListItem listItem);

        protected virtual void ReorganiseContent(bool clearContents) {
            if(clearContents) {
                middleIndex = 0;
                extraScroll = 0;
                velocity = 0;
                OnIndexCentered(0);
            }
            int toAddIndex = (int)extraScroll;
            middleIndex += toAddIndex;
            extraScroll -= toAddIndex;

            bool childrenChanged = CheckChildItems();
            bool populateAll = childrenChanged || clearContents;

            int firstVisibleIndex = Mathf.FloorToInt(middleIndex - VisibleItems / 2f);

            int newRowStart = firstVisibleIndex - rowsAboveBelow;

            // If we've moved too far to be able to reuse anything, same as init case
            int diff = newRowStart - hostStartIndex;
            if(populateAll || Mathf.Abs(diff) >= childBuffer.Length) {

                hostStartIndex = newRowStart;
                childBufferStartIndex = 0;
                int rowIdx = newRowStart;
                foreach(var item in childBuffer) {
                    UpdateChild(item, rowIdx++);
                }

            } else if(diff != 0) {
                // moves the window of the circular buffer so that only old cells that still exist don't need to be updated
                int newBufferStart = (childBufferStartIndex + diff) % childBuffer.Length;

                if(diff < 0) {
                    // window moved backwards
                    for(int i = 1;i <= -diff;++i) {
                        int bufi = WrapChildIndex(childBufferStartIndex - i);
                        int rowIdx = hostStartIndex - i;
                        UpdateChild(childBuffer[bufi], rowIdx);
                    }
                } else {
                    // window moved forwards
                    int prevLastBufIdx = childBufferStartIndex + childBuffer.Length - 1;
                    int prevLastRowIdx = hostStartIndex + childBuffer.Length - 1;
                    for(int i = 1;i <= diff;++i) {
                        int bufi = WrapChildIndex(prevLastBufIdx + i);
                        int rowIdx = prevLastRowIdx + i;
                        UpdateChild(childBuffer[bufi], rowIdx);
                    }
                }

                hostStartIndex = newRowStart;
                childBufferStartIndex = newBufferStart;
            }

        }

        private int WrapChildIndex(int idx) {
            while(idx < 0)
                idx += childBuffer.Length;

            return idx % childBuffer.Length;
        }

        private int WrapItemIndex(int idx) {
            while(idx < 0)
                idx += ItemCount;
            return idx %= ItemCount;
        }

        protected virtual void UpdateChild(ListItem child, int rowIdx) {
            int itemCount = ItemCount;

            UpdateChildPosition(child, rowIdx);
            int posRow = rowIdx;
            while(rowIdx < 0)
                rowIdx += itemCount;
            rowIdx %= itemCount;
            child.SetHost(posRow, rowIdx, Hosts[rowIdx]);

            child.gameObject.SetActive(true);
        }

        protected abstract void UpdateChildPosition(ListItem child, int rowIdx);

        protected virtual void Update() {
            extraScroll += velocity * Time.deltaTime;
            if(velocity < 0) {
                velocity += friction * Time.deltaTime;
                if(velocity > 0) {
                    velocity = 0;
                    if(IsAlreadyCentered()) CheckIfNewIndexCentered();
                }
            }
            if(velocity > 0) {
                velocity -= friction * Time.deltaTime;
                if(velocity < 0) {
                    velocity = 0;
                    if(IsAlreadyCentered()) CheckIfNewIndexCentered();
                }
            }
            if(!isApproachingTarget && CanSnap && Mathf.Abs(velocity) < 0.005f && !IsAlreadyCentered() && !IsGrabbed) {
                CheckIfNewIndexCentered();
                extraScroll = Mathf.Lerp(extraScroll, 0, snapSpeed * Time.deltaTime);
            }
            if(isApproachingTarget) {
                extraScroll = Mathf.Lerp(middleIndex + extraScroll, targetMiddleIndex, snapSpeed * Time.deltaTime) - middleIndex;//Mathf.Round(extraScroll), 10 * Time.deltaTime);
                if(Mathf.Abs(targetMiddleIndex - (extraScroll+middleIndex)) < 0.005f) {
                    isApproachingTarget = false;
                    OnIndexCentered(WrapItemIndex(targetMiddleIndex));
                }
            }
            int middle = Mathf.RoundToInt(extraScroll + middleIndex);
            if(middleLastFrame != middle) {
                OnMiddleChanged();
                middleLastFrame = middle;
            }
        }

        protected virtual void OnMiddleChanged() { }

        private bool IsAlreadyCentered() {
            return extraScroll > -0.05f && extraScroll < 0.05f;
        }

        private void CheckIfNewIndexCentered() {
            float scroll = extraScroll + middleIndex;
            int centerIndex = Mathf.RoundToInt(scroll);
            if(centerIndex != lastCenteredIndex) {
                middleIndex = centerIndex;
                extraScroll = scroll - middleIndex;

                lastCenteredIndex = centerIndex;
                centerIndex = WrapItemIndex(centerIndex);
                OnIndexCentered(centerIndex);
            }
        }

        protected abstract void OnIndexCentered(int index);

        public void SetVelocity(float velocity) {
            if(ScrollLocked)
                return;
            this.velocity = velocity;
        }
    }
}