using System;
using UnityEngine;

public class LocaleCSVs : MonoBehaviour{
    public static TextAsset EnglishCSV;
    public static TextAsset SpanishCSV;
    
    [SerializeField] private TextAsset englishCSV;
    [SerializeField] private TextAsset spanishCSV;

    private void Awake() {
        EnglishCSV = englishCSV;
        SpanishCSV = spanishCSV;
    }
}