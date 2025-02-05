using Orbit.Attributes;
using Orbit.Components;
using Orbit.Parser;
using System;

public class ResourcesAndGlobalsView : OrbitView {
    private void Awake() {
        //Globals should be set before parsing happens, it's safe to set them in Awake but really they should be defined by some outer class
        OrbitParser.DefaultParser.SetGlobal("GlobalText", "This text is defined globally");
    }

    [ListenFor("ChangeGlobal")]
    private void ChangeGlobal() {
        OrbitParser.DefaultParser.SetGlobal("GlobalText", "Global text changed");
    }
}