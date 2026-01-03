using Orbit.Attributes;
using Orbit.Components;
using System.Collections.Generic;
using UnityEngine;

[OrbitClass(Access = OrbitMemberAccess.Public)]
public class SettingTestView : OrbitView {
    public enum DropdownValue {
        FirstOption,
        SecondOption,
        ThirdOption,
        FourthOption,
        FifthOption,
        SixthOption,
        SeventhOption,
        EighthOption,
    }
    
    public float SliderVal;
    public bool ToggleVal;
    public string InputStringVal;
    public float InputFloatVal = 1.5f;
    public int InputIntVal;
    public DropdownValue DropdownVal;
    public string ListDropdownVal = "List";
    public List<string> ListDropdownVals = new() {"Values", "Coming", "From", "List"};

    public void PrintValues() {
        Debug.Log(SliderVal);
        Debug.Log(ToggleVal);
        Debug.Log(InputStringVal);
        Debug.Log(InputFloatVal);
        Debug.Log(InputIntVal);
        Debug.Log(DropdownVal);
        Debug.Log(ListDropdownVal);
    }
}