using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
    /// <summary>
    /// Obsolete — the dual skeleton AnimationFollower auto-initializes bones at runtime.
    /// Kept for reference. No manual bone assignment needed.
    /// </summary>
    public class FixAnimatedBones
    {
        [MenuItem("Ragdoll Realms/Debug/Verify Ragdoll Bones")]
        public static void Execute()
        {
            var go = Selection.activeGameObject;
            if (go == null) { Debug.LogError("[FixBones] No GameObject selected."); return; }

            var joints = go.GetComponentsInChildren<ConfigurableJoint>();
            if (joints.Length == 0)
            {
                Debug.LogError("[FixBones] No ConfigurableJoints found on selected object.");
                return;
            }

            Debug.Log($"[FixBones] Found {joints.Length} ConfigurableJoints:");
            for (int i = 0; i < joints.Length; i++)
            {
                var connected = joints[i].connectedBody != null ? joints[i].connectedBody.name : "none";
                Debug.Log($"  [{i}] {joints[i].name} → connected to: {connected}");
            }

            var af = go.GetComponent<RagdollRealms.Systems.Ragdoll.AnimationFollower>();
            if (af != null)
                Debug.Log($"[FixBones] AnimationFollower present. Dual skeleton initializes automatically at runtime.");
            else
                Debug.LogWarning("[FixBones] AnimationFollower not found on this object.");
        }
    }
}
