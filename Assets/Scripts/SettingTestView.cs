using Atlas.Orbit.Attributes;
using Atlas.Orbit.Components;
using UnityEngine;

public class SettingTestView : UIView {
    [ValueID("SliderVal")] private float sliderVal;
    [ValueID("ToggleVal")] private bool toggleVal;
    [ValueID("InputVal")] private string inputVal;

    [ListenFor("PrintValues")]
    private void PrintValues() {
        Debug.Log(sliderVal);
        Debug.Log(toggleVal);
        Debug.Log(inputVal);
    }
}