namespace RagdollRealms.Core
{
    public interface IRagdollConfig
    {
        float DefaultSpring { get; }
        float DefaultDamper { get; }
        float MaxSpringForce { get; }

        float HipMass { get; }
        float TorsoMass { get; }
        float LimbMass { get; }
        float HeadMass { get; }

        float RecoveryDelay { get; }
        float RecoveryDuration { get; }
        float MinVitalityToRecover { get; }

        float KnockbackMultiplier { get; }
        float RagdollForceThreshold { get; }
        float ArmorKnockbackReduction { get; }

        float LodFullDistance { get; }
        float LodReducedDistance { get; }
        float LodDisableDistance { get; }

        float BalanceSensitivity { get; }
        float GetUpSpringMultiplier { get; }

        float CollisionSpringReduction { get; }
        float CollisionRecoverySpeed { get; }
        float MinCollisionForce { get; }
        float CollisionPropagationFactor { get; }
    }
}
