using UnityEngine;
using UnityEditor;

namespace RagdollRealms.Editor
{
    public class DeepDiagnose
    {
        [MenuItem("Ragdoll Realms/Debug/Deep Diagnose Selected")]
        public static void Execute()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("[DD] Must be in Play mode!");
                return;
            }

            var go = Selection.activeGameObject;
            if (go == null) { Debug.LogError("[DD] No GameObject selected. Select a ragdoll in the hierarchy."); return; }

            var af = go.GetComponent<RagdollRealms.Systems.Ragdoll.AnimationFollower>();
            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

            // Check SOURCE animator
            var srcAnim = go.GetComponent<Animator>();
            if (srcAnim != null)
            {
                var srcState = srcAnim.GetCurrentAnimatorStateInfo(0);
                Debug.Log($"[DD] SOURCE: Speed={srcAnim.GetFloat("Speed"):F3}, normalizedTime={srcState.normalizedTime:F3}, length={srcState.length:F3}");
                Debug.Log($"[DD] SOURCE: isLocomotion={srcState.IsName("Locomotion")}, enabled={srcAnim.enabled}");
            }

            // Check TARGET animator
            var targetAnimField = af?.GetType().GetField("_targetAnimator", flags);
            var targetAnim = targetAnimField?.GetValue(af) as Animator;
            if (targetAnim != null)
            {
                var tgtState = targetAnim.GetCurrentAnimatorStateInfo(0);
                Debug.Log($"[DD] TARGET: Speed={targetAnim.GetFloat("Speed"):F3}, normalizedTime={tgtState.normalizedTime:F3}, length={tgtState.length:F3}");
                Debug.Log($"[DD] TARGET: isLocomotion={tgtState.IsName("Locomotion")}");
            }

            // Check joint targets vs target bones - are they different from idle?
            var jointsField = af?.GetType().GetField("_joints", flags);
            var joints = jointsField?.GetValue(af) as ConfigurableJoint[];
            var targetBonesField = af?.GetType().GetField("_targetBones", flags);
            var targetBones = targetBonesField?.GetValue(af) as Transform[];
            var startRotField = af?.GetType().GetField("_startLocalRotations", flags);
            var startRots = startRotField?.GetValue(af) as Quaternion[];

            if (joints != null && targetBones != null && startRots != null)
            {
                for (int i = 0; i < Mathf.Min(4, joints.Length); i++)
                {
                    if (targetBones[i] == null) continue;
                    float startVsTarget = Quaternion.Angle(startRots[i], targetBones[i].localRotation);
                    Debug.Log($"[DD] Bone[{i}] '{joints[i].name}': targetBone.localRot={targetBones[i].localRotation}, startRot={startRots[i]}, diff={startVsTarget:F2}°, jointTarget={joints[i].targetRotation}");
                }
            }

            // Check PlayerMovementController
            var pmc = go.GetComponent<RagdollRealms.Systems.Player.PlayerMovementController>();
            if (pmc != null)
            {
                Debug.Log($"[DD] PMC: dir={pmc.MovementDirection}, sprint={pmc.IsSprinting}");

                var inputField = pmc.GetType().GetField("_inputController", flags);
                var input = inputField?.GetValue(pmc) as RagdollRealms.Systems.Player.PlayerInputController;
                if (input != null)
                    Debug.Log($"[DD] Input: moveInput={input.CurrentMoveInput}, magnitude={input.CurrentMoveInput.magnitude:F3}");
            }
        }
    }
}
