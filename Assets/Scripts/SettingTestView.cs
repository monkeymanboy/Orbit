using Atlas.Orbit.Attributes;
using Atlas.Orbit.Components;
using UnityEngine;

public class SettingTestView : OrbitView {
    private enum DropdownValue {
        FirstOption,
        SecondOption,
        ThirdOption
    }
    
    [ValueID("SliderVal")] private float sliderVal;
    [ValueID("ToggleVal")] private bool toggleVal;
    [ValueID("InputVal")] private string inputVal;
    [ValueID("DropdownVal")] private DropdownValue dropdownVal;

    [ListenFor("PrintValues")]
    private void PrintValues() {
        Debug.Log(sliderVal);
        Debug.Log(toggleVal);
        Debug.Log(inputVal);
        Debug.Log(dropdownVal);
    }
}