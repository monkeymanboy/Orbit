using Atlas.Orbit.Schema;
using UnityEditor;

public static class GenerateOrbitSchema {
    [MenuItem("Tools/Generate Orbit Schema")]
    private static void Generate() {
        SchemaGenerator.Generate();
    }
}