using UnityEngine;
using UnityEngine.InputSystem;
using RagdollRealms.Core;
using RagdollRealms.Systems.Ragdoll;

namespace RagdollRealms.Debugging
{
    /// <summary>
    /// Debug helper for testing per-limb collision wobble.
    ///
    /// All keys require Right Shift held to avoid conflicts with other debug tools.
    ///
    /// Keys (hold RShift +):
    ///   1 = Nudge left arm (sub-threshold force)
    ///   2 = Nudge right arm
    ///   3 = Nudge left leg
    ///   4 = Nudge right leg
    ///   5 = Nudge head
    ///   G = Toggle gravity (lets ragdoll flop to see physics)
    ///   T = Toggle animation (disable AnimationFollower to see pure physics)
    ///   V = Toggle dual skeleton visibility (show hidden anim skeleton)
    ///   H = Cycle spring strength (100%, 50%, 25%, 10%)
    ///
    /// Also shows real-time OnGUI overlay with per-joint impact weights.
    /// </summary>
    public class CollisionWobbleTestHelper : MonoBehaviour
    {
    [Header("Test Force (sub-threshold nudge)")]
    [SerializeField] private float _nudgeForce = 5f;

    private IRagdollController _controller;
    private AnimationFollower _animFollower;
    private RagdollCollisionHandler _collisionHandler;
    private bool _initialized;
    private bool _animEnabled = true;
    private bool _dualSkeletonVisible;
    private int _springLevel; // 0=100%, 1=50%, 2=25%, 3=10%
    private readonly float[] _springLevels = { 1f, 0.5f, 0.25f, 0.1f };
    private GameObject _animSkeleton;

    // Bone name fragments to find specific limbs
    private static readonly string[] ArmLeftNames = { "LeftUpperArm", "LeftLowerArm", "LeftArm", "LeftForeArm" };
    private static readonly string[] ArmRightNames = { "RightUpperArm", "RightLowerArm", "RightArm", "RightForeArm" };
    private static readonly string[] LegLeftNames = { "LeftUpperLeg", "LeftLowerLeg", "LeftUpLeg", "LeftLeg" };
    private static readonly string[] LegRightNames = { "RightUpperLeg", "RightLowerLeg", "RightUpLeg", "RightLeg" };
    private static readonly string[] HeadNames = { "Head" };

    private void LateUpdate()
    {
        if (!_initialized)
        {
            _controller = GetComponent<IRagdollController>();
            _animFollower = GetComponent<AnimationFollower>();
            _collisionHandler = GetComponent<RagdollCollisionHandler>();
            if (_controller != null)
                _initialized = true;
            return;
        }

        var kb = Keyboard.current;
        if (kb == null) return;

        // All wobble test keys require Right Shift held to avoid conflicts
        if (!kb.rightShiftKey.isPressed) return;

        // Nudge specific body parts with sub-threshold forces (RShift + 1-5)
        if (kb.digit1Key.wasPressedThisFrame) NudgeBone(ArmLeftNames, Vector3.right);
        if (kb.digit2Key.wasPressedThisFrame) NudgeBone(ArmRightNames, Vector3.left);
        if (kb.digit3Key.wasPressedThisFrame) NudgeBone(LegLeftNames, Vector3.forward);
        if (kb.digit4Key.wasPressedThisFrame) NudgeBone(LegRightNames, Vector3.forward);
        if (kb.digit5Key.wasPressedThisFrame) NudgeBone(HeadNames, Vector3.back);

        // Toggle gravity (RShift + G)
        if (kb.gKey.wasPressedThisFrame)
        {
            bool useGravity = !_controller.AllBodies[0].useGravity;
            foreach (var body in _controller.AllBodies)
                body.useGravity = useGravity;
            Debug.Log($"[WobbleTest] Gravity: {(useGravity ? "ON" : "OFF")}");
        }

        // Toggle animation follower (RShift + T)
        // When disabled: zeroes all springs so ragdoll goes fully limp
        if (kb.tKey.wasPressedThisFrame)
        {
            _animEnabled = !_animEnabled;
            _animFollower?.SetEnabled(_animEnabled);
            if (!_animEnabled)
            {
                // Zero all springs so the ragdoll goes limp (no joint forces)
                for (int i = 0; i < _controller.JointCount; i++)
                    _controller.SetJointSpringMultiplier(i, 0f);
                _controller.SetJointSpringMultiplier(0f);
            }
            else
            {
                // Restore springs
                _controller.SetJointSpringMultiplier(1f);
            }
            Debug.Log($"[WobbleTest] AnimationFollower: {(_animEnabled ? "ON" : "OFF (springs zeroed, ragdoll limp)")}");
        }

        // Toggle dual skeleton visibility (RShift + V)
        if (kb.vKey.wasPressedThisFrame)
        {
            ToggleDualSkeletonVisibility();
        }

        // Cycle spring strength (RShift + H)
        if (kb.hKey.wasPressedThisFrame)
        {
            _springLevel = (_springLevel + 1) % _springLevels.Length;
            _controller.SetJointSpringMultiplier(_springLevels[_springLevel]);
            Debug.Log($"[WobbleTest] Spring: {_springLevels[_springLevel] * 100f}%");
        }
    }

    private void NudgeBone(string[] nameFragments, Vector3 direction)
    {
        foreach (var body in _controller.AllBodies)
        {
            foreach (var name in nameFragments)
            {
                if (body.gameObject.name.Contains(name))
                {
                    var joint = body.GetComponent<ConfigurableJoint>();
                    if (joint == null)
                    {
                        Debug.LogError($"[WobbleTest] No ConfigurableJoint on {body.gameObject.name}!");
                        return;
                    }

                    // DIAGNOSTIC: print full state before nudge
                    int jointIndex = _controller.GetJointIndex(joint);
                    var srcAnimator = GetComponent<Animator>();
                    Debug.Log($"[WobbleTest] === NUDGE DIAGNOSTIC ===\n" +
                        $"  Bone: {body.gameObject.name}\n" +
                        $"  JointIndex: {jointIndex}\n" +
                        $"  Body isKinematic: {body.isKinematic}\n" +
                        $"  Source Animator enabled: {(srcAnimator != null ? srcAnimator.enabled.ToString() : "NULL")}\n" +
                        $"  AnimFollower initialized: {(_animFollower != null ? _animFollower.IsEnabled.ToString() : "NULL")}\n" +
                        $"  CollisionHandler: {(_collisionHandler != null ? "OK" : "NULL")}\n" +
                        $"  Joint angularXDrive spring: {joint.angularXDrive.positionSpring}\n" +
                        $"  Joint slerpDrive spring: {joint.slerpDrive.positionSpring}\n" +
                        $"  Joint rotationDriveMode: {joint.rotationDriveMode}\n" +
                        $"  Config CollisionSpringReduction: {_controller.Config.CollisionSpringReduction}\n" +
                        $"  Config MinCollisionForce: {_controller.Config.MinCollisionForce}");

                    // Apply force
                    Vector3 force = direction * _nudgeForce;
                    body.AddForce(force, ForceMode.Impulse);

                    // Trigger wobble
                    if (_collisionHandler != null && jointIndex >= 0)
                    {
                        _collisionHandler.ApplyImpact(jointIndex, force.magnitude, force, body.position);

                        // DIAGNOSTIC: check blend weight immediately after
                        float blendAfter = _animFollower != null ? _animFollower.GetJointBlendWeight(jointIndex) : -1f;
                        Debug.Log($"[WobbleTest] After ApplyImpact:\n" +
                            $"  BlendWeight[{jointIndex}]: {blendAfter}\n" +
                            $"  Joint spring NOW: {joint.slerpDrive.positionSpring}");
                    }

                    #if UNITY_EDITOR
                    // BRUTE FORCE TEST: directly zero the joint drive to prove physics works
                    Debug.Log("[WobbleTest] BRUTE FORCE: zeroing joint drive directly for 2 seconds");
                    StartCoroutine(BruteForceWobble(joint, jointIndex, body, force));
                    #endif
                    return;
                }
            }
        }
        Debug.LogWarning("[WobbleTest] Bone not found for nudge");
    }

    #if UNITY_EDITOR
    private System.Collections.IEnumerator BruteForceWobble(ConfigurableJoint joint, int jointIndex, Rigidbody body, Vector3 force)
    {
        // Completely kill the joint drive — bone should go fully limp
        var zeroDrive = new JointDrive
        {
            positionSpring = 0f,
            positionDamper = 0f,
            maximumForce = 0f
        };
        joint.angularXDrive = zeroDrive;
        joint.angularYZDrive = zeroDrive;
        joint.slerpDrive = zeroDrive;

        // Disable animation follower so it can't overwrite the drives
        bool wasEnabled = _animFollower != null && _animFollower.IsEnabled;
        _animFollower?.SetEnabled(false);

        // Re-apply force now that drives are zeroed
        body.AddForce(force, ForceMode.Impulse);

        Debug.Log($"[WobbleTest] BRUTE FORCE active — drives zeroed, anim disabled. Spring={joint.slerpDrive.positionSpring}");

        yield return new WaitForSeconds(2f);

        // Restore
        if (wasEnabled) _animFollower?.SetEnabled(true);
        Debug.Log("[WobbleTest] BRUTE FORCE ended — restoring animation");
    }
    #endif

    private void ToggleDualSkeletonVisibility()
    {
        if (_animSkeleton == null && _animFollower != null)
        {
            // Find the hidden animation skeleton
            var targetAnim = _animFollower.TargetAnimator;
            if (targetAnim != null)
                _animSkeleton = targetAnim.gameObject;
        }

        if (_animSkeleton != null)
        {
            _dualSkeletonVisible = !_dualSkeletonVisible;
            _animSkeleton.hideFlags = _dualSkeletonVisible ? HideFlags.None : HideFlags.HideInHierarchy;

            // Add visible gizmos via line renderers or just log
            Debug.Log($"[WobbleTest] Dual skeleton: {(_dualSkeletonVisible ? "VISIBLE in hierarchy" : "HIDDEN")}");
        }
        else
        {
            Debug.LogWarning("[WobbleTest] Animation skeleton not found");
        }
    }

    private void OnGUI()
    {
        if (!_initialized || _controller == null) return;

        var headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.cyan }
        };
        var normalStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            normal = { textColor = Color.white }
        };

        float panelWidth = 450f;
        float x = Screen.width - panelWidth - 10f;
        float y = 10;

        GUI.Label(new Rect(x, y, panelWidth, 20), "=== Collision Wobble Test ===", headerStyle);
        y += 18;
        GUI.Label(new Rect(x, y, panelWidth, 20), "Hold RShift + 1-5=Nudge G=Grav T=Anim V=Skel H=Spring", normalStyle);
        y += 18;
        GUI.Label(new Rect(x, y, panelWidth, 20),
            $"Spring: {_springLevels[_springLevel] * 100f}%  Anim: {(_animEnabled ? "ON" : "OFF")}",
            normalStyle);
        y += 22;

        // Per-joint impact weights
        GUI.Label(new Rect(x, y, panelWidth, 20), "Per-Joint Blend Weights:", headerStyle);
        y += 18;

        if (_animFollower != null)
        {
            var joints = _controller.AllJoints;
            for (int i = 0; i < joints.Count; i++)
            {
                float blendWeight = _animFollower.GetJointBlendWeight(i);
                string boneName = joints[i].gameObject.name;

                // Color: green=1.0, yellow=0.5, red=0.0
                Color barColor = Color.Lerp(Color.red, Color.green, blendWeight);
                var barStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 11,
                    normal = { textColor = barColor }
                };

                // Bar visualization
                string bar = new string('█', Mathf.RoundToInt(blendWeight * 20));
                string empty = new string('░', 20 - Mathf.RoundToInt(blendWeight * 20));
                GUI.Label(new Rect(x, y, panelWidth, 16),
                    $"  {boneName,-20} {bar}{empty} {blendWeight:F2}", barStyle);
                y += 15;
            }
        }
    }
}
}
