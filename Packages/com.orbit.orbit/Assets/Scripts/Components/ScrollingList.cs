using System;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using Orbit.Parser;
using System.Collections;
using System.Collections.Specialized;

namespace Orbit.Components {
    using UnityEngine.Serialization;

    [RequireComponent(typeof(ScrollRect))]
    public class ScrollingList : MonoBehaviour {
        public enum ScrollDirection {
            Vertical,
            Horizontal
        }
        public OrbitRenderData ParentData { get; internal set; }
        
        private INotifyCollectionChanged observedCollection;
        private IList hosts;
        public IList Hosts {
            get => hosts;
            set {
                if(observedCollection != null)
                    observedCollection.CollectionChanged -= OnCollectionChanged;
                hosts = value;
                observedCollection = hosts as INotifyCollectionChanged;
                if(observedCollection != null)
                    observedCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        public XmlNode ItemXml { get; set; }

        [SerializeField]
        private ScrollDirection direction;
        public ScrollDirection Direction {
            get => direction;
            set {
                if(direction == value) return;
                direction = value;
                RectTransform contentRect = scrollRect.content;
                switch(direction) {
                    case ScrollDirection.Vertical:
                        contentRect.anchorMin = new Vector2(0,1);
                        contentRect.anchorMax = new Vector2(1,1);
                        contentRect.sizeDelta = new Vector2(0,0);
                        break;
                    case ScrollDirection.Horizontal:
                        contentRect.anchorMin = new Vector2(1,0);
                        contentRect.anchorMax = new Vector2(1,1);
                        contentRect.sizeDelta = new Vector2(0,0);
                        break;
                }
            }
        }

        [field: SerializeField]
        public float CellSize { get; set; }
        [field: SerializeField]
        public float CellSpacing { get; set; }

        public int RowCount => Hosts.Count;

        protected ScrollRect scrollRect;

        // circular buffer of child items which are reused
        protected ListItem[] childItems;
        // the current start index of the circular buffer
        protected int childBufferStart = 0;
        // the index into source data which childBufferStart refers to 
        protected int sourceDataRowStart;
        
        protected float previousBuildLength = 0;
        protected const int rowsAboveBelow = 1;
        

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if(this == null) { //Handles this component being destroyed
                observedCollection.CollectionChanged -= OnCollectionChanged;
                return;
            }
            Refresh();
        }
        
        /// <summary>
        /// Trigger the refreshing of the list content (e.g. if you've changed some values).
        /// Use this if the number of rows hasn't changed but you want to update the contents
        /// for some other reason. All active items will have the ItemCallback invoked. 
        /// </summary>
        public virtual void Refresh() {
            UpdateContentLength();
            ReorganiseContent(true);
        }

        /// <summary>
        /// Scroll the viewport so that a given row is in view, preferably centred vertically.
        /// </summary>
        /// <param name="row"></param>
        public virtual void ScrollToRow(int row) {
            scrollRect.verticalNormalizedPosition = GetRowScrollPosition(row);
        }

        /// <summary>
        /// Get the normalised vertical scroll position which would centre the given row in the view,
        /// as best as possible without scrolling outside the bounds of the content.
        /// Use this instead of ScrollToRow if you want to control the actual scrolling yourself.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public float GetRowScrollPosition(int row) {
            float rowCentre = (row + 0.5f) * CellLength();
            float vpHeight = ViewportLength();
            float halfVpHeight = vpHeight * 0.5f;
            // Clamp to top of content
            float vpTop = Mathf.Max(0, rowCentre - halfVpHeight);
            float vpBottom = vpTop + vpHeight;
            float contentHeight = scrollRect.content.sizeDelta.y;
            // clamp to bottom of content
            if(vpBottom > contentHeight) // if content is shorter than vp always stop at 0
                vpTop = Mathf.Max(0, vpTop - (vpBottom - contentHeight));

            // Range for our purposes is between top (0) and top of vp when scrolled to bottom (contentHeight - vpHeight)
            // ScrollRect normalised position is 0 at bottom, 1 at top
            // so inverted range because 0 is bottom and our calc is top-down
            return Mathf.InverseLerp(contentHeight - vpHeight, 0, vpTop);
        }

        protected virtual void Awake() {
            if(scrollRect == null) 
                scrollRect = GetComponent<ScrollRect>();
        }

        protected virtual bool CheckChildItems() {
            float buildLength = ViewportLength();
            if(!(childItems == null || buildLength > previousBuildLength))
                return false;

            // create a fixed number of children, we'll re-use them when scrolling
            // figure out how many we need, round up
            int childCount = Mathf.RoundToInt(0.5f + buildLength / CellLength());
            childCount += rowsAboveBelow * 2; // X before, X after

            if(childItems == null)
                childItems = new ListItem[childCount];
            else if(childCount > childItems.Length)
                Array.Resize(ref childItems, childCount);

            for(int i = 0;i < childItems.Length;++i) {
                if(childItems[i] == null) {
                    childItems[i] = CreatePrefab();
                }
                childItems[i].transform.SetParent(scrollRect.content, false);
                childItems[i].gameObject.SetActive(false);
            }

            previousBuildLength = buildLength;

            return true;
        }

        protected ListItem CreatePrefab() {
            GameObject prefabObject = new("ListItem");
            RectTransform prefabTransform = prefabObject.AddComponent<RectTransform>();
            switch(Direction) {
                case ScrollDirection.Vertical:
                    prefabTransform.anchorMin = new Vector2(0, 1);
                    prefabTransform.anchorMax = new Vector2(1, 1);
                    prefabTransform.sizeDelta = new Vector2(0, CellSize);
                    break;
                case ScrollDirection.Horizontal:
                    prefabTransform.anchorMin = new Vector2(0, 0);
                    prefabTransform.anchorMax = new Vector2(0, 1);
                    prefabTransform.sizeDelta = new Vector2(CellSize, 0);
                    break;
            }
            ListItem item = prefabObject.AddComponent<ListItem>();
            item.Parse(ParentData.Parser, ItemXml, Hosts[0], ParentData);
            return item;
        }

        protected virtual void OnEnable() {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }

        protected virtual void OnDisable() {
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }

        protected virtual void OnScrollChanged(Vector2 normalisedPos) {
            ReorganiseContent(false);
        }

        protected virtual void ReorganiseContent(bool clearContents) {
            if(clearContents) {
                scrollRect.StopMovement();
                switch(Direction) {
                    case ScrollDirection.Vertical:
                        scrollRect.verticalNormalizedPosition = 1;
                        break;
                    case ScrollDirection.Horizontal:
                        scrollRect.horizontalNormalizedPosition = 0;
                        break;
                }
            }

            if(Hosts == null || Hosts.Count == 0) {
                if(childItems == null) return;
                foreach(ListItem child in childItems) {
                    child?.gameObject.SetActive(false);
                }
                return;
            }
            
            bool childrenChanged = CheckChildItems();
            bool populateAll = childrenChanged || clearContents;

            // Figure out which is the first virtual slot visible
            float minPos = Direction switch {
                ScrollDirection.Vertical => scrollRect.content.localPosition.y,
                ScrollDirection.Horizontal => -scrollRect.content.localPosition.x
            };

            // round down to find first visible
            int firstVisibleIndex = (int)(minPos / CellLength());

            // we always want to start our buffer before
            int newRowStart = firstVisibleIndex - rowsAboveBelow;

            // If we've moved too far to be able to reuse anything, same as init case
            int diff = newRowStart - sourceDataRowStart;
            if(populateAll || Mathf.Abs(diff) >= childItems.Length) {
                sourceDataRowStart = newRowStart;
                childBufferStart = 0;
                int rowIdx = newRowStart;
                foreach(var item in childItems) {
                    UpdateChild(item, rowIdx++);
                }

            } else if(diff != 0) {
                // we scrolled forwards or backwards within the tolerance that we can re-use some of what we have
                // Move our window so that we just re-use from back and place in front
                // children which were already there and contain correct data won't need changing
                int newBufferStart = (childBufferStart + diff) % childItems.Length;

                if(diff < 0) {
                    // window moved backwards
                    for(int i = 1;i <= -diff;++i) {
                        int bufi = WrapChildIndex(childBufferStart - i);
                        int rowIdx = sourceDataRowStart - i;
                        UpdateChild(childItems[bufi], rowIdx);
                    }
                } else {
                    // window moved forwards
                    int prevLastBufIdx = childBufferStart + childItems.Length - 1;
                    int prevLastRowIdx = sourceDataRowStart + childItems.Length - 1;
                    for(int i = 1;i <= diff;++i) {
                        int bufi = WrapChildIndex(prevLastBufIdx + i);
                        int rowIdx = prevLastRowIdx + i;
                        UpdateChild(childItems[bufi], rowIdx);
                    }
                }

                sourceDataRowStart = newRowStart;
                childBufferStart = newBufferStart;
            }

        }

        private int WrapChildIndex(int idx) {
            while(idx < 0)
                idx += childItems.Length;

            return idx % childItems.Length;
        }

        private float CellLength() {
            return CellSpacing + CellSize;
        }

        private float ViewportLength() {
            return Direction switch {
                ScrollDirection.Vertical => scrollRect.viewport.rect.height,
                ScrollDirection.Horizontal => scrollRect.viewport.rect.width
            };
        }

        protected virtual void UpdateChild(ListItem child, int rowIdx) {
            if(rowIdx < 0 || rowIdx >= RowCount) {
                child.gameObject.SetActive(false);
                return;
            }

            RectTransform childTransform = child.transform as RectTransform;
            float pos = CellLength() * rowIdx;
            float pivotOffset;
            switch(Direction) {
                case ScrollDirection.Vertical:
                    pivotOffset = (1f - childTransform.pivot.y) * CellSize;
                    (child.transform as RectTransform).anchoredPosition = new Vector2(0, -(pos + pivotOffset));
                    break;
                case ScrollDirection.Horizontal:
                    pivotOffset = (1f - childTransform.pivot.x) * CellSize;
                    (child.transform as RectTransform).anchoredPosition = new Vector2((pos + pivotOffset), 0);
                    break;
            }

            child.SetHost(rowIdx, rowIdx, Hosts[rowIdx]);

            child.gameObject.SetActive(true);
        }

        protected virtual void UpdateContentLength() {
            if(scrollRect == null) 
                scrollRect = GetComponent<ScrollRect>();
            float length = CellSize * RowCount + (RowCount - 1) * CellSpacing;
            switch(Direction) {
                case ScrollDirection.Vertical:
                    scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, length);
                    break;
                case ScrollDirection.Horizontal:
                    scrollRect.content.sizeDelta = new Vector2(length, scrollRect.content.sizeDelta.y);
                    break;
            }
        }
    }
}