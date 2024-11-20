using Orbit.Attributes;
using Orbit.Components;
using System.Collections.Generic;
using UnityEngine;

public class ListTestView : OrbitView {
    [ValueID] private List<object> listItems;

    [ListenFor("PreParse")]
    private void PreParse() {
        listItems = new List<object>();
        for(int i = 0;i < 100;i++) {
            ListItem listItem = new ListItem();
            listItem.color = Random.ColorHSV(0, 1);
            listItem.text = $"Item #{i}";
            listItems.Add(listItem);
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