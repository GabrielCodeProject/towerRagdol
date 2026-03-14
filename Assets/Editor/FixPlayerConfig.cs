using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
public class FixPlayerConfig
{
    public static void Execute()
    {
        var guids = AssetDatabase.FindAssets("t:RagdollRealms.Content.PlayerConfigDefinition");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            var serialized = new SerializedObject(so);

            var groundDist = serialized.FindProperty("_groundCheckDistance");
            if (groundDist != null)
            {
                groundDist.floatValue = 1.8f;
                Debug.Log($"[Fix] Set groundCheckDistance to 1.8");
            }

            var groundMask = serialized.FindProperty("_groundLayerMask");
            if (groundMask != null)
            {
                // Layer 0 = Default, bitmask = 1
                groundMask.intValue = 1;
                Debug.Log($"[Fix] Set groundLayerMask to Default (1)");
            }

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Fix] Updated {path}");
        }
    }
}
}
