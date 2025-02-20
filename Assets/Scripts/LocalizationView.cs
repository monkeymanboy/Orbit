using Orbit.Attributes;
using Orbit.Components;
using Orbit.Parser;
using System;
using UnityEngine;

public class LocalizationView : OrbitView {
    private TextAsset englishCSV;
    private TextAsset spanishCSV;

    private void Start() {
        //You can handle storing these however you like, for demo simplicity I'm pulling them from static fields  
        englishCSV = LocaleCSVs.EnglishCSV;
        spanishCSV = LocaleCSVs.SpanishCSV;
    }

    [ListenFor("EnglishClicked")]
    private void EnglishClicked() => CSVHelper.PopulateGlobals(Parser, englishCSV);
    [ListenFor("SpanishClicked")]
    private void SpanishClicked() => CSVHelper.PopulateGlobals(Parser, spanishCSV);
}