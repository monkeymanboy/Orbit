using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WebCodeCacheSO : ScriptableObject {
    [Serializable]
    public struct CodeData {
        public string type;
        public string code;
    }
    public List<CodeData> code;
}