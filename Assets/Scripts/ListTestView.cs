using Orbit.Attributes;
using Orbit.Components;
using System.Collections.Generic;
using UnityEngine;

public class ListTestView : OrbitView {
    [ValueID] private List<ListItem> listItems;

    [ListenFor("PreParse")]
    private void PreParse() {
        listItems = new List<ListItem>();
        for(int i = 0;i < 100;i++) {
            listItems.Add(new() {
                color = Random.ColorHSV(0, 1),
                text = $"Item #{i}"
            });
        }
    }

    [ListenFor("CellClicked")]
    private void CellClicked(ListItem clickedItem) {
        Debug.Log($"Parent detected click of child item '{clickedItem.text}'");
    }

    private struct ListItem {
        [ValueID] internal Color color;
        [ValueID] internal string text;
        
        [ListenFor("CellClicked")]
        private void CellClicked() {
            Debug.Log($"Detected click of item '{text}'");
        }
    }
}