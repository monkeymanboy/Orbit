using Atlas.Orbit.Schema;
using UnityEditor;

public static class GenerateOrbitSchema {
    #if UNITY_EDITOR
    [MenuItem("Tools/Generate Orbit Schema")]
    private static void Generate() {
        SchemaGenerator.Generate();
    }
    #endif
}