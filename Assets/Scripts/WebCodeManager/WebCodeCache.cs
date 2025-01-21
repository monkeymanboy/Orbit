#if UNITY_EDITOR
using Orbit.Components;
using Orbit.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class WebCodeCache : IPreprocessBuildWithReport {
    public int callbackOrder { get; } = 0;
    public void OnPreprocessBuild(BuildReport report) {
        WebCodeCacheSO codeCache = Resources.Load<WebCodeCacheSO>("WebCodeCache");
        codeCache.code = new();
        foreach(Type type in Assembly.GetExecutingAssembly().GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(OrbitView)))) {
            codeCache.code.Add(new WebCodeCacheSO.CodeData(){type = type.Name, code = File.ReadAllText($"Assets/Scripts/{type.Name}.cs")});
        }
        EditorUtility.SetDirty(codeCache);
    }
}
#endif