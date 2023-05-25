using Atlas.Orbit.Attributes;
using Atlas.Orbit.Components;
using System.Collections.Generic;
using UnityEngine;

public class SettingTestView : OrbitView {
    private enum DropdownValue {
        FirstOption,
        SecondOption,
        ThirdOption
    }
    
    [ValueID("SliderVal")] private float sliderVal;
    [ValueID("ToggleVal")] private bool toggleVal;
    [ValueID("InputStringVal")] private string inputVal;
    [ValueID("InputFloatVal")] private float inputFloatVal = 1.5f;
    [ValueID("InputIntVal")] private int inputIntVal;
    [ValueID("DropdownVal")] private DropdownValue dropdownVal;
    [ValueID("ListDropdownVal")] private string listdropdownVal = "List";
    [ValueID("ListDropdownVals")] private List<string> listdropdownVals = new() {"Values", "Coming", "From", "List"};

    [ListenFor("PrintValues")]
    private void PrintValues() {
        Debug.Log(sliderVal);
        Debug.Log(toggleVal);
        Debug.Log(inputVal);
        Debug.Log(inputFloatVal);
        Debug.Log(inputIntVal);
        Debug.Log(dropdownVal);
        Debug.Log(listdropdownVal);
    }
}