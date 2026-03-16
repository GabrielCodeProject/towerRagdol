using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Content;

namespace RagdollRealms.Systems.Ragdoll
{
    /// <summary>
    /// Physics-first procedural walk system.
    /// Alternates feet between Stance (planted, pushing body forward) and
    /// Swing (lifted, reaching toward next step target).
    /// No IK — pure impulse-based. Physics determines the outcome.
    /// </summary>
    public class ProceduralStepController : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundMask;

        private IRagdollController _controller;
        private RagdollConfigDefinition _config;
        private Rigidbody _hipRb;

        private FootSensor _leftFoot;
        private FootSensor _rightFoot;
        private Rigidbody _leftFootRb;
        private Rigidbody _rightFootRb;
        private Rigidbody _leftThighRb;
        private Rigidbody _rightThighRb;
        private Rigidbody _leftShinRb;
        private Rigidbody _rightShinRb;

        // Knee joint indices for spring softening during swing
        private int _leftKneeJointIndex = -1;
        private int _rightKneeJointIndex = -1;
        private const float KneeSwingSpringMultiplier = 0.15f;

        private LegState _leftLeg;
        private LegState _rightLeg;

        private Vector3 _desiredDirection;
        private float _desiredSpeed;
        private bool _initialized;
        private IJointDebugOverride _debugger;

        private enum StepPhase { Stance, Swing }

        private struct LegState
        {
            public StepPhase Phase;
            public float StanceTimer;
            public Vector3 SwingTarget;
        }

        public void SetDesiredMovement(Vector3 direction, float speed)
        {
            _desiredDirection = direction;
            _desiredSpeed = speed;
        }

        public void SetGroundMask(LayerMask mask)
        {
            _groundMask = mask;
        }

        private void Start()
        {
            _controller = GetComponent<IRagdollController>();
            if (_controller == null) return;

            _config = _controller.Config as RagdollConfigDefinition;
            if (_config == null) return;

            _hipRb = _controller.HipRigidbody;
            _debugger = GetComponent<IJointDebugOverride>();

            FindAndAttachFootSensors();

            _leftLeg = new LegState { Phase = StepPhase.Stance };
            _rightLeg = new LegState { Phase = StepPhase.Stance };

            _initialized = _leftFootRb != null && _rightFootRb != null
                        && _leftThighRb != null && _rightThighRb != null;

            if (_initialized)
                Debug.Log("[ProceduralStep] Initialized — legs: thigh/shin/foot found");
            else
                Debug.LogWarning("[ProceduralStep] Could not find all leg rigidbodies");
        }

        private void FindAndAttachFootSensors()
        {
            foreach (var rb in _controller.AllBodies)
            {
                string name = rb.gameObject.name;

                // Feet — get sensor + rigidbody
                if (name.Contains("LeftFoot"))
                {
                    _leftFootRb = rb;
                    _leftFoot = rb.gameObject.AddComponent<FootSensor>();
                    _leftFoot.Initialize(_config.FootGroundCheckDist, _groundMask);
                }
                else if (name.Contains("RightFoot"))
                {
                    _rightFootRb = rb;
                    _rightFoot = rb.gameObject.AddComponent<FootSensor>();
                    _rightFoot.Initialize(_config.FootGroundCheckDist, _groundMask);
                }
                // Thighs (UpLeg) — for knee lift
                else if (name.Contains("LeftUpLeg"))
                {
                    _leftThighRb = rb;
                }
                else if (name.Contains("RightUpLeg"))
                {
                    _rightThighRb = rb;
                }
                // Shins (Leg, but not UpLeg) — for forward swing + knee joint index
                else if (name.Contains("LeftLeg") && !name.Contains("UpLeg"))
                {
                    _leftShinRb = rb;
                    var joint = rb.GetComponent<ConfigurableJoint>();
                    if (joint != null) _leftKneeJointIndex = _controller.GetJointIndex(joint);
                }
                else if (name.Contains("RightLeg") && !name.Contains("UpLeg"))
                {
                    _rightShinRb = rb;
                    var joint = rb.GetComponent<ConfigurableJoint>();
                    if (joint != null) _rightKneeJointIndex = _controller.GetJointIndex(joint);
                }
            }
        }

        /// <summary>
        /// When idle, gently pull foot XZ toward the position directly under the hip.
        /// Prevents feet from staying behind after stopping.
        /// </summary>
        private void ReturnFootUnderHip(Rigidbody footRb)
        {
            Vector3 hipPos = _hipRb.position;
            Vector3 footPos = footRb.position;

            // Target: directly under hip, at current foot Y
            Vector3 target = new Vector3(hipPos.x, footPos.y, hipPos.z);
            Vector3 toTarget = target - footPos;
            toTarget.y = 0f;

            float dist = toTarget.magnitude;
            if (dist > 0.05f)
            {
                // Gentle spring-like pull toward under-hip position
                footRb.AddForce(toTarget.normalized * dist * 8f, ForceMode.Force);
            }
        }

        private bool IsJointDebugLocked(int jointIndex)
        {
            return _debugger != null && _debugger.IsJointLocked(jointIndex);
        }

        private void FixedUpdate()
        {
            if (!_initialized || _controller.IsRagdolling) return;
            if (_debugger != null && _debugger.IsWalkSystemDisabled) return;

            bool wantsToMove = _desiredDirection.sqrMagnitude > 0.01f && _desiredSpeed > 0.01f;

            UpdateLeg(ref _leftLeg, _leftFoot, _leftFootRb, _leftThighRb, _leftShinRb, _leftKneeJointIndex, _rightFoot, wantsToMove);
            UpdateLeg(ref _rightLeg, _rightFoot, _rightFootRb, _rightThighRb, _rightShinRb, _rightKneeJointIndex, _leftFoot, wantsToMove);

            // When idle: dampen hip velocity and pull feet back under hip
            if (!wantsToMove)
            {
                Vector3 vel = _hipRb.linearVelocity;
                _hipRb.linearVelocity = Vector3.Lerp(vel, new Vector3(0f, vel.y, 0f), 5f * Time.fixedDeltaTime);

                ReturnFootUnderHip(_leftFootRb);
                ReturnFootUnderHip(_rightFootRb);
            }
        }

        private void UpdateLeg(ref LegState leg, FootSensor foot, Rigidbody footRb,
            Rigidbody thighRb, Rigidbody shinRb, int kneeJointIndex,
            FootSensor otherFoot, bool wantsToMove)
        {
            switch (leg.Phase)
            {
                case StepPhase.Stance:
                    UpdateStance(ref leg, foot, footRb, thighRb, kneeJointIndex, otherFoot, wantsToMove);
                    break;
                case StepPhase.Swing:
                    UpdateSwing(ref leg, foot, footRb, thighRb, shinRb, kneeJointIndex);
                    break;
            }
        }

        private void UpdateStance(ref LegState leg, FootSensor foot, Rigidbody footRb,
            Rigidbody thighRb, int kneeJointIndex, FootSensor otherFoot, bool wantsToMove)
        {
            leg.StanceTimer += Time.fixedDeltaTime;

            // Restore knee spring to full stiffness during stance (planted leg)
            if (kneeJointIndex >= 0 && !IsJointDebugLocked(kneeJointIndex))
                _controller.SetJointSpringMultiplier(kneeJointIndex, 1f);

            if (!wantsToMove) return;

            // Push hip forward from planted foot
            if (foot.IsGrounded)
            {
                Vector3 pushDir = _desiredDirection.normalized;
                _hipRb.AddForce(pushDir * _config.StancePushForce, ForceMode.Force);
            }

            // Check if this foot needs to step:
            // - been in stance long enough
            // - foot is too far behind hip in the movement direction
            // - other foot is grounded (can support weight)
            if (leg.StanceTimer < _config.MinStanceTime) return;
            if (!otherFoot.IsGrounded) return;

            Vector3 hipPos = _hipRb.position;
            Vector3 footPos = footRb.position;
            Vector3 hipToFoot = footPos - hipPos;
            hipToFoot.y = 0;

            // Foot is "behind" if dot product with desired direction is negative,
            // or foot is far from hip
            float behindness = -Vector3.Dot(hipToFoot.normalized, _desiredDirection.normalized);
            float horizontalDist = hipToFoot.magnitude;

            if (behindness > 0.2f || horizontalDist > _config.StrideLength)
            {
                // Compute swing target: ahead of hip in movement direction
                Vector3 target = hipPos + _desiredDirection.normalized * _config.StrideLength;
                target.y = hipPos.y - _config.HipToGroundOffset;

                // Raycast to find actual ground
                if (Physics.Raycast(target + Vector3.up * 0.5f, Vector3.down, out var hit, 2f, _groundMask))
                {
                    target.y = hit.point.y;
                }

                leg.SwingTarget = target;
                leg.Phase = StepPhase.Swing;
                leg.StanceTimer = 0f;

                // Soften knee joint so gravity can bend it during swing.
                // At full spring (1500), the joint fights any bending.
                // At 15%, shin/foot weight (1.8kg) naturally folds the knee.
                if (kneeJointIndex >= 0 && !IsJointDebugLocked(kneeJointIndex))
                    _controller.SetJointSpringMultiplier(kneeJointIndex, KneeSwingSpringMultiplier);

                // Lift the THIGH up — with softened knee, this bends the knee naturally.
                // Gravity pulls shin/foot down, creating knee-bent posture.
                thighRb.AddForce(Vector3.up * _config.LiftForce, ForceMode.Impulse);
            }
        }

        private void UpdateSwing(ref LegState leg, FootSensor foot, Rigidbody footRb,
            Rigidbody thighRb, Rigidbody shinRb, int kneeJointIndex)
        {
            float elapsed = leg.StanceTimer;
            leg.StanceTimer += Time.fixedDeltaTime;

            // Phase 1 (first half): Lift — keep thigh up, knee is soft so it bends
            // Phase 2 (second half): Stiffen knee + extend — guide foot toward ground target
            float t = Mathf.Clamp01(elapsed / _config.SwingDuration);

            if (t < 0.5f)
            {
                // LIFT PHASE: knee stays soft (set in UpdateStance transition)
                // Continuous upward force on thigh to hold knee up
                thighRb.AddForce(Vector3.up * _config.LiftForce * 0.5f, ForceMode.Force);

                // Slight forward pull on thigh to advance the leg
                thighRb.AddForce(_desiredDirection.normalized * _config.SwingForce * 0.3f, ForceMode.Force);
            }
            else
            {
                // EXTEND PHASE: gradually stiffen knee to extend the leg forward
                float stiffenT = (t - 0.5f) * 2f; // 0→1 over second half
                float kneeSpring = Mathf.Lerp(KneeSwingSpringMultiplier, 1f, stiffenT);
                if (kneeJointIndex >= 0 && !IsJointDebugLocked(kneeJointIndex))
                    _controller.SetJointSpringMultiplier(kneeJointIndex, kneeSpring);

                // Guide foot toward ground target
                Vector3 toTarget = leg.SwingTarget - footRb.position;
                float dist = toTarget.magnitude;

                if (dist > 0.05f)
                {
                    Vector3 guideForce = toTarget.normalized * _config.SwingForce * Mathf.Min(dist, 1f);
                    footRb.AddForce(guideForce, ForceMode.Force);
                }

                // Also pull shin forward + down to extend the leg
                if (shinRb != null)
                {
                    Vector3 shinTarget = leg.SwingTarget + Vector3.up * 0.2f;
                    Vector3 toShinTarget = shinTarget - shinRb.position;
                    shinRb.AddForce(toShinTarget.normalized * _config.SwingForce * 0.5f, ForceMode.Force);
                }
            }

            // Transition back to stance when foot lands
            if (t > 0.3f && foot.IsGrounded && footRb.position.y < _hipRb.position.y - 0.3f)
            {
                if (kneeJointIndex >= 0 && !IsJointDebugLocked(kneeJointIndex))
                    _controller.SetJointSpringMultiplier(kneeJointIndex, 1f);
                leg.Phase = StepPhase.Stance;
                leg.StanceTimer = 0f;
            }

            // Timeout: if swing takes too long, force back to stance
            if (leg.StanceTimer > 0.8f)
            {
                if (kneeJointIndex >= 0 && !IsJointDebugLocked(kneeJointIndex))
                    _controller.SetJointSpringMultiplier(kneeJointIndex, 1f);
                leg.Phase = StepPhase.Stance;
                leg.StanceTimer = 0f;
            }
        }
    }
}
