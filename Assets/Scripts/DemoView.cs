using Orbit.Attributes;
using Orbit.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DemoView : OrbitView {
    [SerializeField] private WebCodeManager webCodeManager;
    
    [ValueID]
    protected List<ViewData> Views = new(){
        new("Test", typeof(TestView)),
        new("Resources & Globals", typeof(ResourcesAndGlobalsView)),
        new("Setting", typeof(SettingTestView)),
        new("List", typeof(ListTestView)),
        new("Dynamic List", typeof(DynamicListTestView)),
        new("Attributes", typeof(AttributeTagsView)),
    };

    private ViewData currentView;
    private void Awake() {
        currentView = Views.First();
    }

    [ListenFor("PostParse")]
    private void ShowActiveTab() {
        currentView.View.gameObject.SetActive(true);
        currentView.Active = true;
        webCodeManager.SetCode(currentView.View);
    }

    [ListenFor("SelectView")]
    private void SelectView(ViewData viewData) {
        if(currentView != null) {
            currentView.Active = false;
        }
        currentView = viewData;
        currentView.Active = true;
        webCodeManager.SetCode(viewData.View);
    }
    
    public class ViewData : INotifyPropertyChanged {
        [ValueID]
        public string Name { get; }
        [ValueID]
        public Type ViewType { get; }
        [ValueID]
        public OrbitView View { get; private set; }

        private bool active;
        [ValueID]
        public bool Active {
            get => active;
            set {
                active = value;
                OnPropertyChanged();
            }
        }
        
        public ViewData(string name, Type viewType) {
            Name = name;
            ViewType = viewType;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}