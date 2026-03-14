using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
public class FixCameraOffset
{
    public static void Execute()
    {
        var guids = AssetDatabase.FindAssets("t:RagdollRealms.Content.PlayerConfigDefinition");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            var serialized = new SerializedObject(so);

            var offset = serialized.FindProperty("_cameraOffset");
            if (offset != null)
            {
                offset.vector3Value = new Vector3(0f, 2f, -3.5f);
                Debug.Log("[Fix] cameraOffset → (0, 2, -3.5)");
            }

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
        }
    }
}
}
