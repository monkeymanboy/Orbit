using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Orbit.Parser {
    using Attributes.TagGenerators;
    using Components;
    using UnityEngine;

    public class UIRenderData {
        private object host;
        public object Host {
            get => host;
            set {
                if(host == value) return;
                if(host is INotifyPropertyChanged oldHost) {
                    oldHost.PropertyChanged -= HandlePropertyChanged;
                }
                host = value;
                if(host is INotifyPropertyChanged newHost) {
                    newHost.PropertyChanged += HandlePropertyChanged;
                }
                if(EventEmitterFields != null)
                    InjectEventEmitterFields();
                RefreshAllValues();
            }
        }

        public UIRenderData ParentRenderData { get; set; }

        internal Dictionary<string, UIValue> Values { get; } = new();
        internal Dictionary<string, UIPropertyValue> Properties { get; } = new();
        internal Dictionary<string, Action> Events { get; } = new();
        internal Dictionary<string, Action<object>> ChildEvents { get; } = new();
        internal OrbitParser Parser { get; set; }
        public GameObject RootParent { get; set; }
        public List<GameObject> RootObjects { get; set; } = new();
        public bool Disabled { get; set; }
        public List<(string, FieldInfo)> EventEmitterFields { get; set; }

        public List<(string,TagGenerator)> TagGenerators { get; set; } = new();


        private Dictionary<string, TagParameters.BoundData> currentDefaultProperties;

        public Dictionary<string, TagParameters.BoundData> CurrentDefaultProperties {
            get => currentDefaultProperties ?? ParentRenderData?.CurrentDefaultProperties;
            set {
                currentDefaultProperties = value;
            }
        }

        public UIValue GetValueFromID(string id) {
            if(id.StartsWith(OrbitParser.PARENT_HOST_VALUE_PREFIX)) {
                if(ParentRenderData == null)
                    throw new Exception("Trying to use ^ when there is no parent host");
                string valueID = id.Substring(1);
                return ParentRenderData.GetValueFromID(valueID);
            }
            if(Values.TryGetValue(id, out UIValue value))
                return value;
            if(id.StartsWith(OrbitParser.NEGATE_VALUE_PREFIX)) {
                string negatedId = id;
                id = id.Substring(1);
                if(!Values.TryGetValue(id, out UIValue valueToNegate))
                    throw new Exception($"Could not negate nonexistent value '{id}'");
                UIValue negatedValue = SetValue(negatedId, !valueToNegate.GetValue<bool>());
                BindNegatedValue(negatedValue, valueToNegate);
                return negatedValue;
            }
            if(ParentRenderData?.Values.ContainsKey(id) ?? false)
                throw new Exception($"Could not find requested UIValue: '{id}' -- Detected possible typo, '~{id}' might need to be '~^{id}'");
            throw new Exception($"Could not find requested UIValue: '{id}'");
        }
        
        internal bool TryGetValueFromID(string id, out UIValue value) {
            return Values.TryGetValue(id, out value);
        }

        private void BindNegatedValue(UIValue negatedValue, UIValue valueToNegate) {
            void OnNegatedValueChange() {
                bool negated = negatedValue.GetValue<bool>();
                if(valueToNegate.GetValue<bool>() == negated) valueToNegate.SetValue(!negated);
            }
            negatedValue.OnChange += OnNegatedValueChange;

            void OnValueToNegateChange() {
                negatedValue.SetValue(!valueToNegate.GetValue<bool>());
            }
            valueToNegate.OnChange += OnValueToNegateChange;
        }

        public UIValue SetValue<T>(string id, T value) {
            if(id.StartsWith(OrbitParser.PARENT_HOST_VALUE_PREFIX)) {
                if(ParentRenderData == null)
                    throw new Exception("Trying to use ^ when there is no parent host");
                string valueID = id.Substring(1);
                return SetValue(valueID, value);
            }
            if(Values.TryGetValue(id, out UIValue uiValue)) {
                uiValue.SetValue(value);
                return uiValue;
            }
            UIValue definedValue = new DefinedUIValue<T>(this, value);
            Values.Add(id, definedValue);
            return definedValue;
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

        public void InjectEventEmitterFields() {
            foreach(var pair in EventEmitterFields) {
                pair.Item2.SetValue(host, new Action(() => EmitEvent(pair.Item1)));
            }
        }
    }
}
