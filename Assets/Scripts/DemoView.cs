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
    protected List<IViewData> Views = new(){
        new ViewData<TestView>("Test"),
        new ViewData<ResourcesAndGlobalsView>("Resources & Globals"),
        new ViewData<SettingTestView>("Setting"),
        new ViewData<ListTestView>("List"),
        new ViewData<DynamicListTestView>("Dynamic List"),
        new ViewData<AttributeTagsView>("Attributes"),
        new ViewData<LocalizationView>("Localization"),
        new ViewData<ExpressionsView>("Expressions"),
        new ViewData<SmartImageView>("SmartImage"),
    };

    private IViewData currentView;
    private void Awake() {
        currentView = Views.First();
    }

    [ListenFor("PostParse")]
    private void ShowActiveTab() {
        currentView.Active = true;
        webCodeManager.SetCode(currentView.OrbitView);
    }

    [ListenFor("SelectView")]
    private void SelectView(IViewData viewData) {
        if(currentView != null) {
            currentView.Active = false;
        }
        currentView = viewData;
        currentView.Active = true;
        webCodeManager.SetCode(viewData.OrbitView);
    }

    public interface IViewData {
        public string Name { get; }
        public bool Active { get; set; }
        public OrbitView OrbitView { get; }
    }
    public class ViewData<T> : IViewData, INotifyPropertyChanged where T : OrbitView {
        [ValueID]
        public string Name { get; }
        [ValueID]
        public T View { get; private set; }

        private bool active;
        [ValueID]
        public bool Active {
            get => active;
            set {
                active = value;
                OnPropertyChanged();
            }
        }
        
        public OrbitView OrbitView => View;

        public ViewData(string name) {
            Name = name;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}