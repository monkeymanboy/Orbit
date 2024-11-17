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
using System.IO;

namespace Atlas.Orbit.Components {
    using System.Threading;

    public class OrbitView : MonoBehaviour, INotifyPropertyChanged {
        protected virtual OrbitParser Parser => OrbitParser.DefaultParser;
        public virtual bool ShouldParse => true;
        public virtual object UIViewHost => this;

        private bool hasParsed = false;
        private string resourceName;
        private UIRenderData renderData;
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnEnable() {
            if(hasParsed || !ShouldParse)
                return;
            ManualParse();
        }

        [Obsolete("This will reparse the entire view, you should never need this if you call OnPropertyChanged in your property setters. If you still want all bound values to update, you should emit the 'RefreshAll' event instead of using this.")]
        public void ForceReloadView() {
            if(renderData != null) {
                foreach(GameObject go in renderData.RootObjects) {
                    Destroy(go);
                }
            }
            ParseView();
        }

        protected void ParseView() {
            ViewLocationAttribute viewLocation = UIViewHost.GetType().GetCustomAttribute<ViewLocationAttribute>();
            resourceName = viewLocation?.FullPath ?? GetDefaultResourceLocation(GetType(), viewLocation?.Location);
            renderData = Parser.Parse(Resources.Load<TextAsset>(resourceName).text, gameObject, UIViewHost);
        }

        protected void ManualParse() {
            if(hasParsed)
                return;
            ParseView();
#if ORBIT_HOT_RELOAD
            string filePath = $"Assets/Resources/{resourceName}.xml";
            if(File.Exists(filePath)) {
                FileSystemWatcher watcher = new(Path.GetDirectoryName(filePath)) {
                    Path = Path.GetDirectoryName(filePath),
                    Filter = Path.GetFileName(filePath),
                    NotifyFilter = NotifyFilters.Attributes
                                   | NotifyFilters.CreationTime
                                   | NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.Security
                                   | NotifyFilters.Size
                };
                SynchronizationContext syncContext = SynchronizationContext.Current;
                watcher.Changed += (sender, args) => {
                    if(this == null) { //Handles view being destroyed or exiting play mode
                        watcher.Dispose();
                        return;
                    }
                    if(args.ChangeType != WatcherChangeTypes.Changed)
                        return;
                    syncContext.Post(_ => {
                        HotReloadView();
                    },null);
                };
                watcher.EnableRaisingEvents = true;
            }
#endif
            hasParsed = true;
        }

#if ORBIT_HOT_RELOAD
        private void HotReloadView() {
            foreach(GameObject go in renderData.RootObjects) {
                Destroy(go);
            }

            string filePath = $"Assets/Resources/{resourceName}.xml";
            renderData = Parser.Parse(File.ReadAllText(filePath), gameObject, UIViewHost);
        }
#endif

        protected virtual string GetDefaultResourceLocation(Type type, string location) {
            if(location == null)
                return $"Orbit/Views/{type.Name}";
            return $"Orbit/Views/{location}/{type.Name}";
        }
    }
}