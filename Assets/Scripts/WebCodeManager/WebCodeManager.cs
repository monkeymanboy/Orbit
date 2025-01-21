using Orbit.Components;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebCodeManager : MonoBehaviour {
    private OrbitView targetView;
    private Dictionary<string, string> typeToCode = new();
    private bool initialized;
    private bool ignoreNextUpdate;
    
    [DllImport("__Internal")]
    private static extern void SetActive(string xml, string csharp);
    [DllImport("__Internal")]
    private static extern void SubscribeToCodeChanged(string objectId, string callback);

    public void Init() {
        if(initialized) {
            return;
        }
        initialized = true;
#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
        SubscribeToCodeChanged(name, nameof(UpdateView));
#endif
        foreach(WebCodeCacheSO.CodeData codeData in Resources.Load<WebCodeCacheSO>("WebCodeCache").code) {
            typeToCode.Add(codeData.type, codeData.code);
        }
    }

    public void SetCode(OrbitView view) {
        #if !UNITY_WEBGL || UNITY_EDITOR
        return;
        #endif
        Init();
        targetView = view;
        ignoreNextUpdate = true;
        SetActive(view.GetXmlString(), typeToCode[view.GetType().Name]);
    }

    public void UpdateView(string content) {
        if(ignoreNextUpdate) { //Keeps it from parsing twice when updating the initial text
            ignoreNextUpdate = false;
            return;
        }
        targetView.ForceReloadView(content);
    }
}