using Orbit.Attributes;
using Orbit.Components;
using System.Collections.Generic;
using UnityEngine;

[OrbitClass(Access = OrbitMemberAccess.Private)]
public class ListTestView : OrbitView {
    private List<ListItem> listItems;

    private void PreParse() {
        listItems = new List<ListItem>();
        //List will efficiently only create the minimum visible number of cells and then will reuse those as you scroll so no heavy performance impact with many elements
        for(int i = 0;i < 250;i++) {
            listItems.Add(new() {
                color = Random.ColorHSV(0, 1, 0.7f, 0.9f, 0.6f, 0.7f),
                text = $"Item #{i}"
            });
        }
    }
    
    private void CellClicked(ListItem clickedItem) {
        Debug.Log($"Parent detected click of child item '{clickedItem.text}'");
    }

    [OrbitClass(Access = OrbitMemberAccess.Private)]
    private struct ListItem {
        internal Color color;
        internal string text;
        
        private void CellClicked() {
            Debug.Log($"Detected click of item '{text}'");
        }
    }
}