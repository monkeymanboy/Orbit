using TMPro;
using UnityEngine;
using Orbit.Parser;

namespace Orbit.Components {
    /// <summary>
    /// This is handled by a separate component from rather than TMP_TextProcessor so that objects with many text components can share a font without sharing other TMP_Text properties
    /// </summary>
    public class OrbitFontUpdater : MonoBehaviour {
        [SerializeField] private TMP_Text text;

        private bool setFont;
        public OrbitConfig.OrbitFont Font {
            set {
                setFont = true;
                text.font = value.fontAsset;
                if(value.fontMaterial != null) 
                    text.fontSharedMaterial = value.fontMaterial;
            }
        }
        
        public FontWeight FontWeight {
            set {
                text.fontWeight = value;
            }
        }
        
        public void ApplyDefault() {
            if(!setFont)
                Font = OrbitConfig.Config.DefaultFont;
        }
    }
}