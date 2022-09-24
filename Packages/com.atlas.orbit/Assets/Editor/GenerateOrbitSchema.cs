using Atlas.Orbit.Schema;
using UnityEditor;

public static class GenerateOrbitSchema {
    #if !OMIT_ORBIT_SCHEMA_GEN
    [MenuItem("Tools/Generate Orbit Schema")]
    private static void Generate() {
        SchemaGenerator.Generate();
    }
    #endif
}