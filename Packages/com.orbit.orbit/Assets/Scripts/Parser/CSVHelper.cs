namespace Orbit.Parser {
    using System;
    using UnityEngine;

    public static class CSVHelper {
        public static void PopulateGlobals(OrbitParser parser, TextAsset textAsset, bool vertical = false) {
            ForEachPair(textAsset, vertical, parser.SetGlobal);
        }
        
        public static void ForEachPair(TextAsset textAsset, bool vertical, Action<string, string> callback) {
            string[] lines = textAsset.text.Split('\n');
            if(vertical) {
                for(int i = 0;i < lines.Length;i++) {
                    string line = lines[i];
                    int commaIndex = line.IndexOf(',');
                    if(commaIndex == -1)
                        throw new Exception("Malformed CSV");
                    callback?.Invoke(line.Remove(commaIndex), line.Substring(commaIndex+1));
                }
            } else {
                string[] keyLine = lines[0].TrimEnd('\r').Split(',');
                string[] valueLine = lines[1].TrimEnd('\r').Split(',');
                int totalValues = Mathf.Min(keyLine.Length, valueLine.Length);
                for(int i = 0;i < totalValues;i++) {
                    callback?.Invoke(keyLine[i], valueLine[i]);
                }
            }
        }
    }
}