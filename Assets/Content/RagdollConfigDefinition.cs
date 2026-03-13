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
    }
}
