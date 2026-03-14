using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
    public class DiagnosePlayer
    {
        [MenuItem("Ragdoll Realms/Debug/Diagnose Selected Player")]
        public static void Execute()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("[Diag] Must be in Play mode!");
                return;
            }

            var go = Selection.activeGameObject;
            if (go == null) { Debug.LogError("[Diag] No GameObject selected. Select a player in the hierarchy."); return; }

            // Check Animator state
            var animator = go.GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log($"[Diag] Animator: enabled={animator.enabled}, speed={animator.speed}, updateMode={animator.updateMode}");
                Debug.Log($"[Diag] Animator: applyRootMotion={animator.applyRootMotion}, hasRootMotion={animator.hasRootMotion}");
                Debug.Log($"[Diag] Animator: Speed param={animator.GetFloat("Speed")}");

                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                Debug.Log($"[Diag] Animator state: normalizedTime={stateInfo.normalizedTime}, length={stateInfo.length}, speed={stateInfo.speed}");
                Debug.Log($"[Diag] Animator state: isName(Locomotion)={stateInfo.IsName("Locomotion")}");
            }

            // Check AnimationFollower
            var af = go.GetComponent<RagdollRealms.Systems.Ragdoll.AnimationFollower>();
            if (af != null)
            {
                Debug.Log($"[Diag] AnimationFollower: enabled={af.enabled}, IsEnabled={af.IsEnabled}, BlendWeight={af.BlendWeight}");

                var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                var initializedField = af.GetType().GetField("_initialized", flags);
                if (initializedField != null)
                    Debug.Log($"[Diag] AnimationFollower._initialized={initializedField.GetValue(af)}");
            }
        }
    }
}
