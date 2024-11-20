namespace DefaultNamespace {
    using Orbit.Attributes;
    using Orbit.Components;
    using System;
    using System.Collections.Generic;

    public class DynamicListTestView : OrbitView {
        [ValueID] private List<string> strings = new();
        [ValueID] private string enteredText = "";
        [EventEmitter("RefreshList")] private Action RefreshList;

        [ListenFor("RemoveString")]
        private void RemoveString(string str) {
            strings.Remove(str);
            RefreshList();
        }

        [ListenFor("AddString")]
        private void AddString() {
            strings.Add(enteredText);
            RefreshList();
        }
    }
}