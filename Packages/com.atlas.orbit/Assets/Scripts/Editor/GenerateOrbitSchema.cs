using Atlas.Orbit.Schema;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class GenerateOrbitSchema {
    [MenuItem("Tools/Generate Orbit Schema")]
    private static void Generate() {
        var setup = EditorSceneManager.GetSceneManagerSetup();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        SchemaGenerator.Generate();
        EditorSceneManager.RestoreSceneManagerSetup(setup);
    }
}