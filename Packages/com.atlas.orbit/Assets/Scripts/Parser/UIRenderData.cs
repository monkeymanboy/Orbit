using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Atlas.Orbit.Parser {
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
                RefreshAllValues();
            }
        }

        public UIRenderData ParentRenderData { get; set; }

        internal Dictionary<string, UIValue> Values { get; } = new();
        internal Dictionary<string, UIValue> Properties { get; } = new();
        internal Dictionary<string, PropertyInfo> PropertyInfoCache { get; } = new();
        internal Dictionary<string, Action> Events { get; } = new();
        internal Dictionary<string, Action<object>> ChildEvents { get; } = new();
        internal OrbitParser Parser { get; set; }
        public List<GameObject> RootObjects { get; } = new();

        internal UIValue GetValueFromID(string id) {
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
            if(ParentRenderData != null) //TODO(David): Right now emitting events in a child render data sends the events to the parent, should the opposite be true?
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
            PropertyInfo propInfo = PropertyInfoCache[e.PropertyName];
            UIValue uiValue = Properties[e.PropertyName];
            uiValue.InvokeOnChange();
        }
    }
}
