using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SaveCurrentScene
{
    public static void Execute()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        Debug.Log($"[SaveScene] Active scene: {scene.name} at {scene.path}");
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"[SaveScene] Saved to {scene.path}");
    }
}
