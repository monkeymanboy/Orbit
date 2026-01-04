using Orbit.Parser;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public static class AddExpressionMacro
{
#if UNITY_EDITOR
    static AddExpressionMacro() {
        OrbitParser.DefaultParser.AddAssemblyTypes(typeof(AddExpressionMacro).Assembly);
    }
#else
    private static bool initialized = false;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad() {
        if(initialized) return;
        OrbitParser.DefaultParser.AddAssemblyTypes(typeof(AddExpressionMacro).Assembly);
        initialized = true;
    }
#endif
}
