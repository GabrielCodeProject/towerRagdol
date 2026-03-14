using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
    public static class DebugBoneNames
    {
        [MenuItem("Ragdoll Realms/Debug Bone Names")]
        public static void Execute()
        {
            var guids = AssetDatabase.FindAssets("t:Model", new[] { "Assets/Models" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (model == null) continue;

                Debug.Log($"=== Model: {path} ===");
                PrintHierarchy(model.transform, 0);
            }
        }

        private static void PrintHierarchy(Transform t, int depth)
        {
            Debug.Log($"{"".PadLeft(depth * 2)}Bone: '{t.name}'");
            for (int i = 0; i < t.childCount; i++)
            {
                PrintHierarchy(t.GetChild(i), depth + 1);
            }
        }
    }
}
