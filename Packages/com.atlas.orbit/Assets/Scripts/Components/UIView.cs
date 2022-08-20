#if UNITY_EDITOR
#define ORBIT_HOT_RELOAD
#endif
using Atlas.Orbit.Attributes;
using Atlas.Orbit.Parser;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Atlas.Orbit.Components {
    using System.IO;

    public class UIView : MonoBehaviour, INotifyPropertyChanged {
        protected virtual UIParser Parser => OrbitParser.DefaultParser;
        public virtual bool ShouldParse => true;
        public virtual object UIViewHost => this;

        private bool hasParsed = false;
        private string resourceName;
        private UIRenderData renderData;
        private bool reload;
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
            resourceName = viewLocation?.FullPath ?? GetDefaultResourceLocation(GetType(), viewLocation?.Location);
            renderData = Parser.Parse(Resources.Load<TextAsset>(resourceName).text, gameObject, UIViewHost);
#if ORBIT_HOT_RELOAD
            string filePath = $"Assets/Resources/{resourceName}.xml";
            if(File.Exists(filePath)) {
                FileSystemWatcher watcher = new(Path.GetDirectoryName(filePath));
                watcher.Path = Path.GetDirectoryName(filePath);
                watcher.Filter = Path.GetFileName(filePath);
                watcher.NotifyFilter = NotifyFilters.Attributes
                                       | NotifyFilters.CreationTime
                                       | NotifyFilters.DirectoryName
                                       | NotifyFilters.FileName
                                       | NotifyFilters.LastAccess
                                       | NotifyFilters.LastWrite
                                       | NotifyFilters.Security
                                       | NotifyFilters.Size;

                watcher.Changed += (sender, args) => {
                    if(args.ChangeType != WatcherChangeTypes.Changed) {
                        return;
                    }

                    reload = true;
                };
                watcher.EnableRaisingEvents = true;
            }
#endif
            hasParsed = true;
        }

#if ORBIT_HOT_RELOAD
        private void Update() {
            if(reload) { //TODO(David): Would be better if this didn't use update, need to make this code run on the main thread though
                foreach(GameObject go in renderData.RootObjects) {
                    Destroy(go);
                }

                string filePath = $"Assets/Resources/{resourceName}.xml";
                renderData = Parser.Parse(File.ReadAllText(filePath), gameObject, UIViewHost);
                reload = false;
            }
        }
#endif

        protected virtual string GetDefaultResourceLocation(Type type, string location) {
            if(location == null)
                return $"Orbit/Views/{type.Name}";
            return $"Orbit/Views/{location}/{type.Name}";
        }
    }
}