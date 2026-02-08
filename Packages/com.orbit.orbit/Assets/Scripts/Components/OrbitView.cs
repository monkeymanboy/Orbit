#if UNITY_EDITOR
#define ORBIT_HOT_RELOAD
#endif
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO;
using Orbit.Attributes;
using Orbit.Parser;
using System.Threading;
using UnityEditor;

namespace Orbit.Components {

    public class OrbitView : MonoBehaviour, INotifyPropertyChanged {
        protected virtual OrbitParser Parser => OrbitParser.DefaultParser;
        public virtual bool ShouldParse => true;
        public virtual object UIViewHost => this;

        private bool hasParsed = false;
        private string resourceName;
        private OrbitRenderData renderData;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        #if ORBIT_HOT_RELOAD
        private string viewAssetPath;
        private string ViewAssetPath {
            get {
                if(viewAssetPath == null)
                    viewAssetPath = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>(resourceName));
                return viewAssetPath;
            }
        }
        #endif

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
            hasParsed = true;
        }

        public void ForceReloadView(string content) {
            if(renderData != null) {
                foreach(GameObject go in renderData.RootObjects) {
                    Destroy(go);
                }
            }
            try {
                renderData = Parser.Parse(GetXmlString(), gameObject, UIViewHost);
            } catch (Exception ex){
                RenderException(ex);
            }
        }

        public string GetXmlString() {
            ViewLocationAttribute viewLocation = UIViewHost.GetType().GetCustomAttribute<ViewLocationAttribute>();
            resourceName = viewLocation?.FullPath ?? GetDefaultResourceLocation(GetType(), viewLocation?.Location);
            return Resources.Load<TextAsset>(resourceName).text;
        }

        protected void ParseView() {
            try {
                renderData = Parser.Parse(GetXmlString(), gameObject, UIViewHost);
            } catch (Exception ex){
                RenderException(ex);
            }
        }

        protected void ManualParse() {
            if(hasParsed)
                return;
            ParseView();
#if ORBIT_HOT_RELOAD
            string filePath = ViewAssetPath;
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
            if(renderData != null) {
                foreach(GameObject go in renderData.RootObjects) {
                    Destroy(go);
                }
            }
            try {
                renderData = Parser.Parse(File.ReadAllText(ViewAssetPath), gameObject, UIViewHost);
            } catch (Exception ex){
                RenderException(ex);
            }
        }
#endif
        protected virtual void RenderException(Exception ex) {
            Debug.LogError(ex, this);
            foreach(Transform childTransform in transform) {
                Destroy(childTransform.gameObject);
            }
            string errorLine = $"Uncaught Exception while parsing '{GetType()}'\n{ex.GetType()}: {ex.Message}\n{ex.StackTrace}";
            errorLine = errorLine.Replace("\n", "<br>");
            errorLine = System.Security.SecurityElement.Escape(errorLine);
            //We still set renderData here so that the objects get cleared out on next hot reload
            renderData = Parser.Parse($@"<VerticalScrollView ChildControlHeight=""true"" PadAll=""10""><Text Text=""{errorLine}"" FontSize=""20""/></VerticalScrollView>", gameObject);
        }

        protected virtual string GetDefaultResourceLocation(Type type, string location) {
            if(location == null)
                return $"OrbitViews/{type.Name}";
            return $"OrbitViews/{location}/{type.Name}";
        }
    }
}