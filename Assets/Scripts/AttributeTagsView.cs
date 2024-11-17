using Atlas.Orbit.Attributes;
using Atlas.Orbit.Attributes.TagGenerators;
using Atlas.Orbit.Components;
using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttributeTagsView : OrbitView {
    private enum DropdownValue {
        FirstOption,
        SecondOption,
        ThirdOption
    }
    
    [OrbitSlider(0, 1, WholeNumbers = false)]
    private float SliderVal { get; set; }
    
    [OrbitSlider(0, 100, Increments = 100)]
    private int SliderValInt { get; set; }
    
    [OrbitDropdown(Text = "Custom Label")]
    private DropdownValue DropdownVal { get; set; }
    
    [OrbitToggle]
    private bool ToggleVal { get; set; }
    
    [OrbitInputField]
    private string InputVal { get; set; }

    private string randomText;
    [OrbitText(FontSize = 40, AutoMinFontSize = 20, FontStyle = FontStyles.Bold | FontStyles.Italic, FontColor = "red", Group = "RandomText")]
    private string RandomText {
        get => randomText;
        set {
            randomText = value;
            OnPropertyChanged();
        }
    }
    
    [OrbitTag("<Button Text='~$VALUE_ID' ClickEvent='PrintValues'/>")]
    private string ButtonText => "Print Values";
    
    [OrbitTag("<Button Text='~$VALUE_ID' ClickEvent='RandomizeText'/>", Group = "RandomText")]
    private string RandomizeTextButton => "Randomize Text";
    
    [ListenFor("PreParse,RandomizeText")]
    private void RandomizeText() {
        RandomText = Random.Range(0, 3) switch {
            0 => "This text is random",
            1 => "Random text",
            2 => "I was randomly picked",
            _ => RandomText
        };
    }
    
    [ListenFor("PrintValues")]
    private void PrintValues() {
        Debug.Log(SliderVal);
        Debug.Log(SliderValInt);
        Debug.Log(DropdownVal);
        Debug.Log(ToggleVal);
        Debug.Log(InputVal);
    }
}