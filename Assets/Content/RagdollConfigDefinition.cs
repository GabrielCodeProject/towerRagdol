using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewRagdollConfig", menuName = "Ragdoll Realms/Content/Ragdoll Config")]
    public class RagdollConfigDefinition : ContentDefinition, IRagdollConfig
    {
        [Header("Joint Physics")]
        [SerializeField] private float _defaultSpring = 500f;
        [SerializeField] private float _defaultDamper = 50f;
        [SerializeField] private float _maxSpringForce = 1000f;

        [Header("Mass")]
        [SerializeField] private float _hipMass = 5f;
        [SerializeField] private float _torsoMass = 3f;
        [SerializeField] private float _limbMass = 1f;
        [SerializeField] private float _headMass = 1.5f;

        [Header("Recovery")]
        [SerializeField] private float _recoveryDelay = 1.5f;
        [SerializeField] private float _recoveryDuration = 1f;
        [SerializeField] private float _minVitalityToRecover = 0.1f;

        [Header("Knockback")]
        [SerializeField] private float _knockbackMultiplier = 1f;
        [SerializeField] private float _ragdollForceThreshold = 10f;
        [SerializeField] private float _armorKnockbackReduction = 0f;

        [Header("LOD")]
        [SerializeField] private float _lodFullDistance = 20f;
        [SerializeField] private float _lodReducedDistance = 40f;
        [SerializeField] private float _lodDisableDistance = 60f;

        [Header("Sensitivity")]
        [SerializeField] private float _balanceSensitivity = 1f;
        [SerializeField] private float _getUpSpringMultiplier = 2f;

        [Header("Procedural Step")]
        [SerializeField] private float _strideLength = 0.6f;
        [SerializeField] private float _stepHeight = 0.3f;
        [SerializeField] private float _swingForce = 12f;
        [SerializeField] private float _liftForce = 8f;
        [SerializeField] private float _stancePushForce = 15f;
        [SerializeField] private float _minStanceTime = 0.15f;
        [SerializeField] private float _footGroundCheckDist = 0.15f;
        [SerializeField] private float _swingDuration = 0.4f;
        [SerializeField] private float _hipToGroundOffset = 0.9f;

        [Header("Collision Response")]
        [SerializeField] private float _collisionSpringReduction = 0.95f;
        [SerializeField] private float _collisionRecoverySpeed = 2f;
        [SerializeField] private float _minCollisionForce = 1f;
        [SerializeField] private float _collisionPropagationFactor = 0.3f;

        public float DefaultSpring => _defaultSpring;
        public float DefaultDamper => _defaultDamper;
        public float MaxSpringForce => _maxSpringForce;

        public float HipMass => _hipMass;
        public float TorsoMass => _torsoMass;
        public float LimbMass => _limbMass;
        public float HeadMass => _headMass;

        public float RecoveryDelay => _recoveryDelay;
        public float RecoveryDuration => _recoveryDuration;
        public float MinVitalityToRecover => _minVitalityToRecover;

        public float KnockbackMultiplier => _knockbackMultiplier;
        public float RagdollForceThreshold => _ragdollForceThreshold;
        public float ArmorKnockbackReduction => _armorKnockbackReduction;

        public float LodFullDistance => _lodFullDistance;
        public float LodReducedDistance => _lodReducedDistance;
        public float LodDisableDistance => _lodDisableDistance;

        public float BalanceSensitivity => _balanceSensitivity;
        public float GetUpSpringMultiplier => _getUpSpringMultiplier;

        public float StrideLength => _strideLength;
        public float StepHeight => _stepHeight;
        public float SwingForce => _swingForce;
        public float LiftForce => _liftForce;
        public float StancePushForce => _stancePushForce;
        public float MinStanceTime => _minStanceTime;
        public float FootGroundCheckDist => _footGroundCheckDist;
        public float SwingDuration => _swingDuration;
        public float HipToGroundOffset => _hipToGroundOffset;

        public float CollisionSpringReduction => _collisionSpringReduction;
        public float CollisionRecoverySpeed => _collisionRecoverySpeed;
        public float MinCollisionForce => _minCollisionForce;
        public float CollisionPropagationFactor => _collisionPropagationFactor;
    }
}
