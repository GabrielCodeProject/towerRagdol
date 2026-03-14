using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
public class FixJointStrength
{
    public static void Execute()
    {
        var guids = AssetDatabase.FindAssets("t:RagdollRealms.Content.RagdollConfigDefinition");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            var serialized = new SerializedObject(so);

            // Low spring = wobbly ragdoll (Gang Beasts feel)
            // High spring = stiff animation-like (no ragdoll feel)
            var spring = serialized.FindProperty("_defaultSpring");
            if (spring != null) { spring.floatValue = 400f; Debug.Log($"[Fix] spring → 400"); }

            var damper = serialized.FindProperty("_defaultDamper");
            if (damper != null) { damper.floatValue = 40f; Debug.Log($"[Fix] damper → 40"); }

            var maxForce = serialized.FindProperty("_maxSpringForce");
            if (maxForce != null) { maxForce.floatValue = 5000f; Debug.Log($"[Fix] maxForce → 5000"); }

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(so);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Fix] Updated {path}");
        }
    }
}
}
