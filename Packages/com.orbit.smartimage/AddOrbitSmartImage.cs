namespace Orbit.SmartImage {
    using Orbit.Parser;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;

    [InitializeOnLoad]
#endif
    public static class AddOrbitSmartImage {
#if UNITY_EDITOR
        static AddOrbitSmartImage() {
            OrbitParser.DefaultParser.AddAssemblyTypes(typeof(AddOrbitSmartImage).Assembly);
        }
#else
    private static bool initialized = false;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad() {
        if(initialized) return;
        OrbitParser.DefaultParser.AddAssemblyTypes(typeof(AddOrbitSmartImage).Assembly);
        initialized = true;
    }
#endif
    }
}