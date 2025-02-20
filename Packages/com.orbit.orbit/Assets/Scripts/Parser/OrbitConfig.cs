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
        [Serializable]
        public struct ColorDefintion {
            public string name;
            public Color color;
        }
        [Serializable]
        public struct GlobalsCsv {
            public TextAsset textAsset;
            public bool vertical;
        }
        
        private static OrbitConfig config;
        public static OrbitConfig Config {
            get {
                if(config == null)
                    config = Load();
                return config;
            }
        }

        private const string configPath = "Resources/OrbitConfig.asset";
        private const string configResource = "OrbitConfig";

        [SerializeField] private OrbitFont[] fonts;
        [SerializeField] private ColorDefintion[] colors;
        [SerializeField] private GlobalsCsv[] globalsCsvs;
        public OrbitFont DefaultFont { get; private set; }
        public Dictionary<string, OrbitFont> Fonts { get; private set; }
        public ColorDefintion[] Colors => colors;

        public static OrbitConfig CreateDefault() {
            OrbitConfig defaultConfig = CreateInstance<OrbitConfig>();
            defaultConfig.Fonts = new() { {"Default", new OrbitFont{ name = "Default", fontAsset = TMP_Settings.defaultFontAsset }} };
#if UNITY_EDITOR
            AssetDatabase.CreateFolder("Assets", "Orbit");
            AssetDatabase.CreateFolder("Assets/Orbit", "Resources");
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
            foreach(GlobalsCsv globalsCSV in config.globalsCsvs) {
                CSVHelper.PopulateGlobals(OrbitParser.DefaultParser,globalsCSV.textAsset, globalsCSV.vertical);
            }
            return config;
        }
    }
}