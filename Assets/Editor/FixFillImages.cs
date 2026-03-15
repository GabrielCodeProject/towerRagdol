using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class FixFillImages
{
    public static void Execute()
    {
        // Get the default UI sprite (white square)
        var defaultSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        string[] fillPaths = {
            "Canvas/PlayerStats/HealthBarFill",
            "Canvas/PlayerStats/ManaBarFill",
            "Canvas/PlayerStats/XpBarFill"
        };

        foreach (var path in fillPaths)
        {
            var go = GameObject.Find(path);
            if (go == null) { Debug.LogError($"Not found: {path}"); continue; }

            var img = go.GetComponent<Image>();
            img.sprite = defaultSprite;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            img.fillAmount = 1f;
            EditorUtility.SetDirty(img);
            Debug.Log($"Fixed fill image: {path}");
        }

        // Also fix spell cooldown fills
        for (int i = 0; i < 4; i++)
        {
            var go = GameObject.Find($"Canvas/SpellToolbar/Slot{i}/CooldownFill");
            if (go == null) continue;
            var img = go.GetComponent<Image>();
            img.sprite = defaultSprite;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Vertical;
            img.fillAmount = 0f;
            EditorUtility.SetDirty(img);
        }

        // Force save
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("All fill images fixed and scene saved!");
    }
}
