using Atlas.Orbit.Attributes;
using Atlas.Orbit.Parser;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Atlas.Orbit.Components {
    public class UIView : MonoBehaviour, INotifyPropertyChanged {
        protected virtual UIParser Parser => OrbitParser.DefaultParser;

        private bool hasParsed = false;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnEnable() {
            if(hasParsed || !ShouldParse)
                return;
            ManualParse();
        }

        protected void ManualParse() {
            if(hasParsed)
                return;
            ViewLocationAttribute viewLocation = UIViewHost.GetType().GetCustomAttribute<ViewLocationAttribute>();
            string resourceName = viewLocation?.FullPath ??  GetDefaultResourceLocation(GetType(), viewLocation?.Location);
            Parser.Parse(Resources.Load<TextAsset>(resourceName).text, gameObject, UIViewHost);
            hasParsed = true;
        }

        public virtual bool ShouldParse => true;
        public virtual object UIViewHost => this;
        protected virtual string GetDefaultResourceLocation(Type type, string location) {
            if(location == null) 
                return $"Orbit/Views/{type.Name}";
            return $"Orbit/Views/{location}/{type.Name}";
        }
    }
}
