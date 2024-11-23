using Orbit.Attributes;
using Orbit.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DynamicListTestView : OrbitView {
    //By using ObservableCollection instead of List, we don't need to worry about refreshing the list at all, it will happen automatically when the list changes
    [ValueID] private ObservableCollection<string> strings = new();
    [ValueID] private string enteredText = "";

    [ListenFor("RemoveString")]
    private void RemoveString(string str) {
        strings.Remove(str);
    }

    [ListenFor("AddString")]
    private void AddString() {
        strings.Add(enteredText);
    }

    [ListenFor("ClearStrings")]
    private void ClearStrings() {
        strings.Clear();
    }
}