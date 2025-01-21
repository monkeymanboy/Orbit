using TMPro;
using UnityEditor;
using UnityEngine;

namespace Orbit.Parser {
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "OrbitConfig")]
    public class OrbitConfig : ScriptableObject {
        [Serializable]
        public struct OrbitFont {
            public string name;
            public TMP_FontAsset fontAsset;
            public Material fontMaterial;
        }
        
        private static OrbitConfig config;
        public static OrbitConfig Config {
            get {
                if(config == null)
                    config = Load();
                return config;
            }
        }

        private const string configPath = "Assets/Orbit/Resources/OrbitConfig.asset";
        private const string configResource = "OrbitConfig";

        [SerializeField] private OrbitFont[] fonts;
        public OrbitFont DefaultFont { get; private set; }
        public Dictionary<string, OrbitFont> Fonts { get; private set; }

        public static OrbitConfig CreateDefault() {
            OrbitConfig defaultConfig = CreateInstance<OrbitConfig>();
            defaultConfig.Fonts = new() { {"Default", new OrbitFont{ name = "Default", fontAsset = TMP_Settings.defaultFontAsset }} };
#if UNITY_EDITOR
            AssetDatabase.CreateFolder("Assets", "Orbit");
            AssetDatabase.CreateFolder("Assets/Orbit", "Resources");
            Debug.Log(2);
            AssetDatabase.CreateAsset(defaultConfig, configPath);
#endif
            return defaultConfig;
        }

        public static OrbitConfig Load() {
            OrbitConfig config = Resources.Load<OrbitConfig>(configResource) ?? CreateDefault();
            config.Fonts = new();
            config.DefaultFont = config.fonts[0];
            foreach(OrbitFont font in config.fonts) {
                config.Fonts.Add(font.name, font);
            }
            return config;
        }
    }
}