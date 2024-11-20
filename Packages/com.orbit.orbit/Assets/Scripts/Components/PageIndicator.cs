using Orbit.Parser;
using System;
using UnityEngine;

namespace Orbit.Components {
    using Parser;

    public class PageIndicator : MonoBehaviour {
        private int currentPage;
        public int CurrentPage {
            get => currentPage;
            set {
                currentPage = value;
                UpdatePages();
            }
        }
        private int pageCount;
        public int PageCount {
            get => pageCount;
            set {
                pageCount = value;
                UpdatePages();
            }
        }
        public int MaxPages {
            get => instantiatedCells?.Length ?? 0;
            set {
                if(instantiatedCells != null)
                    throw new Exception("Page Indicator not set up for dynamically changing MaxPages");
                instantiatedCells = new PageIndicatorCell[value];
                for(int i = 0;i < value;i++) {
                    instantiatedCells[i] = Instantiate(cellPrefab, cellPrefab.transform.parent);
                    instantiatedCells[i].OnClick += PageClicked;
                    instantiatedCells[i].gameObject.SetActive(false);
                }
                UpdatePages();
            }
        }

        public UIFunction ColorFunction { get; set; }
        public UIFunction SelectPageFunction { get; set; }

        private int bottomIndex;
        private int topIndex;

        [SerializeField]
        private PageIndicatorCell cellPrefab;

        private PageIndicatorCell[] instantiatedCells;

        public void UpdatePages() {
            if(instantiatedCells == null || PageCount == 0)
                return;

            int pagesBelow = MaxPages / 2;
            bottomIndex = Math.Max(0, CurrentPage - pagesBelow);
            topIndex = bottomIndex + MaxPages;
            if(topIndex >= PageCount) {
                topIndex = PageCount - 1;
                if(PageCount > MaxPages) {
                    bottomIndex = topIndex - MaxPages;
                    if(bottomIndex < 0)
                        throw new Exception("Page Indicator Invalid State");
                }
            }
            for(int i = 0;i < instantiatedCells.Length;i++) {
                if(bottomIndex + i > topIndex) {
                    instantiatedCells[i].gameObject.SetActive(false);
                    continue;
                }
                instantiatedCells[i].gameObject.SetActive(true);
                if(ColorFunction != null)
                    instantiatedCells[i].SetColor(ColorFunction.Invoke<Color>(bottomIndex + i));
                instantiatedCells[i].Index = bottomIndex + i;
                instantiatedCells[i].SetSelected(bottomIndex + i == CurrentPage);
            }
        }

        private void PageClicked(int index) {
            SelectPageFunction?.Invoke(index);
        }
    }
}
