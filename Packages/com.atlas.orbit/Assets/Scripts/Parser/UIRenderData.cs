using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Atlas.Orbit.Parser {
    using Attributes.TagGenerators;
    using Components;
    using UnityEngine;

    public class UIRenderData {
        private object host;
        public object Host {
            get => host;
            set {
                if(host is INotifyPropertyChanged oldHost) {
                    oldHost.PropertyChanged -= HandlePropertyChanged;
                }
                host = value;
                if(host is INotifyPropertyChanged newHost) {
                    newHost.PropertyChanged += HandlePropertyChanged;
                }
                if(ViewComponentFields != null)
                    InjectViewComponentFields();
                if(EventEmitterFields != null)
                    InjectEventEmitterFields();
                RefreshAllValues();
            }
        }

        public UIRenderData ParentRenderData { get; set; }

        internal Dictionary<string, UIValue> Values { get; } = new();
        internal Dictionary<string, UIValue> Properties { get; } = new();
        internal Dictionary<string, Action> Events { get; } = new();
        internal Dictionary<string, Action<object>> ChildEvents { get; } = new();
        internal OrbitParser Parser { get; set; }
        public List<GameObject> RootObjects { get; } = new();
        public bool Disabled { get; set; }
        public List<(string, FieldInfo)> ViewComponentFields { get; set; }
        public List<(string, FieldInfo)> EventEmitterFields { get; set; }

        public UIValue GetValueFromID(string id) {
            if(id.StartsWith(OrbitParser.PARENT_HOST_VALUE_PREFIX)) {
                if(ParentRenderData == null)
                    throw new Exception("Trying to use ^ when there is no parent host");
                string valueID = id.Substring(OrbitParser.PARENT_HOST_VALUE_PREFIX.Length);
                return ParentRenderData.GetValueFromID(valueID);
            }

            if(Values.TryGetValue(id, out UIValue value))
                return value;
            if(ParentRenderData?.Values.ContainsKey(id) ?? false)
                throw new Exception($"Could not find requested UIValue: '{id}' -- Detected possible typo, '~{id}' might need to be '~^{id}'");
            throw new Exception($"Could not find requested UIValue: '{id}'");
        }
        internal bool TryGetValueFromID(string id, out UIValue value) {
            return Values.TryGetValue(id, out value);
        }

        public void SetValue(string id, object value) {
            if(id.StartsWith(OrbitParser.PARENT_HOST_VALUE_PREFIX)) {
                if(ParentRenderData == null)
                    throw new Exception("Trying to use ^ when there is no parent host");
                string valueID = id.Substring(OrbitParser.PARENT_HOST_VALUE_PREFIX.Length);
                SetValue(valueID, value);
                return;
            }
            if(Values.TryGetValue(id, out UIValue uiValue)) {
                uiValue.SetValue(value);
                return;
            }
            Values.Add(id, new DefinedUIValue(this, value));
        }


        /// <summary>
        /// Invokes the callback function <paramref name="action"/> when any of the events in <paramref name="ids"/> are emitted on this render data
        /// </summary>
        /// <param name="ids">Events to listen for</param>
        /// <param name="action">Callback function</param>
        public void AddEvent(string ids, Action action) {
            foreach(string id in ids.Split(',')) {
                if(Events.ContainsKey(id))
                    Events[id] += action;
                else
                    Events.Add(id, action);
            }
        }

        /// <summary>
        /// Invokes the callback function <paramref name="action"/> when any of the events in <paramref name="ids"/> are emitted in a child render data
        /// </summary>
        /// <param name="ids">Events to listen for</param>
        /// <param name="action">Callback function</param>
        public void AddChildEvent(string ids, Action<object> action) {
            foreach(string id in ids.Split(',')) {
                if(ChildEvents.ContainsKey(id))
                    ChildEvents[id] += action;
                else
                    ChildEvents.Add(id, action);
            }
        }

        public void EmitEvent(string ids) {
            foreach(string id in ids.Split(',')) {
                if(Events.ContainsKey(id))
                    Events[id].Invoke();
            }
            if(ParentRenderData != null)
                ParentRenderData.EmitChildEvent(ids, Host);
        }

        public void EmitChildEvent(string ids, object host) {
            foreach(string id in ids.Split(',')) {
                if(ChildEvents.ContainsKey(id))
                    ChildEvents[id].Invoke(host);
            }
            if(ParentRenderData != null)
                ParentRenderData.EmitChildEvent(ids, host);
        }

        internal void RefreshAllValues() {
            foreach(KeyValuePair<string, UIValue> pair in Values) {
                pair.Value.InvokeOnChange();
            }
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e) {
            UIValue uiValue = Properties[e.PropertyName];
            uiValue.InvokeOnChange();
        }

        public void DisableRootObjects() {
            if(Disabled)
                return;
            for(int i=0;i<RootObjects.Count;i++) {
                RootObjects[i].SetActive(false);
            }
            Disabled = true;
        }
        public void EnableRootObjects() {
            if(!Disabled)
                return;
            for(int i=0;i<RootObjects.Count;i++) {
                RootObjects[i].SetActive(true);
            }
            Disabled = false;
        }
        

        public void InjectViewComponentFields() {
            foreach(var pair in ViewComponentFields) {
                if(GetValueFromID(pair.Item1).GetValue() is MarkupPrefab markupPrefab) {
                    pair.Item2.SetValue(host, markupPrefab.FindComponent(pair.Item2.FieldType));
                } else {
                    throw new Exception(
                        "Tried using [ViewComponent] on an ID that is not bound to an object in the view");
                }
            }
        }

        public void InjectEventEmitterFields() {
            foreach(var pair in EventEmitterFields) {
                pair.Item2.SetValue(host, new Action(() => EmitEvent(pair.Item1)));
            }
        }
    }
}
