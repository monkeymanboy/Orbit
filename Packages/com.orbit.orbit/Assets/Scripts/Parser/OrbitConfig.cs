using TMPro;
using UnityEditor;
using UnityEngine;

namespace Orbit.Parser {
    using System;
    using System.Collections.Generic;
    using System.IO;

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

        private const string configPath = "Assets/Resources/OrbitConfig.asset";
        private const string configResource = "OrbitConfig";

        [SerializeField] private string prefabLocation = "OrbitPrefabs";
        [SerializeField] private OrbitFont[] fonts;
        [SerializeField] private ColorDefintion[] colors;
        [SerializeField] private GlobalsCsv[] globalsCsvs;
        [SerializeField] private Material defaultRoundedRectMaterial;
        public OrbitFont DefaultFont { get; private set; }
        public Dictionary<string, OrbitFont> Fonts { get; private set; }
        public ColorDefintion[] Colors => colors;
        public Material DefaultRoundedRectMaterial => defaultRoundedRectMaterial;
        public string PrefabLocation => prefabLocation;

        public static OrbitConfig CreateDefault() {
            OrbitConfig defaultConfig = CreateInstance<OrbitConfig>();
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets("OrbitRounded t:Material");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                defaultConfig.defaultRoundedRectMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
            }
#endif
            OrbitFont defaultFont = new OrbitFont{ name = "Default", fontAsset = TMP_Settings.defaultFontAsset };
            defaultConfig.Fonts = new() {{"Default", defaultFont }};
            defaultConfig.fonts = new[] { defaultFont };
#if UNITY_EDITOR
            if(!AssetDatabase.IsValidFolder("Assets/Resources")) {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            AssetDatabase.CreateAsset(defaultConfig, configPath);
#endif
            return defaultConfig;
        }

        public static OrbitConfig Load() {
            OrbitConfig config = Resources.Load<OrbitConfig>(configResource) ?? CreateDefault();
            config.Fonts = new();
            if(config.fonts.Length > 0) {
                config.DefaultFont = config.fonts[0];
                foreach(OrbitFont font in config.fonts) {
                    config.Fonts.Add(font.name, font);
                }
            }
            if(config.globalsCsvs != null) {
                foreach(GlobalsCsv globalsCSV in config.globalsCsvs) {
                    CSVHelper.PopulateGlobals(OrbitParser.DefaultParser,globalsCSV.textAsset, globalsCSV.vertical);
                }
            }
            return config;
        }
    }
}