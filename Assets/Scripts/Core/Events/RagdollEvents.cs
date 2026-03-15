using UnityEngine;

namespace RagdollRealms.Core.Events
{
    public readonly struct OnRagdollActivated
    {
        public readonly int EntityId;
        public readonly Vector3 Position;

        public OnRagdollActivated(int entityId, Vector3 position)
        {
            EntityId = entityId;
            Position = position;
        }
    }

    public readonly struct OnRagdollDeactivated
    {
        public readonly int EntityId;

        public OnRagdollDeactivated(int entityId)
        {
            EntityId = entityId;
        }
    }

    public readonly struct OnRagdollForceApplied
    {
        public readonly int EntityId;
        public readonly Vector3 Force;
        public readonly ForceType ForceType;

        public OnRagdollForceApplied(int entityId, Vector3 force, ForceType forceType)
        {
            EntityId = entityId;
            Force = force;
            ForceType = forceType;
        }
    }

    public readonly struct OnRagdollRecoveryStarted
    {
        public readonly int EntityId;

        public OnRagdollRecoveryStarted(int entityId)
        {
            EntityId = entityId;
        }
    }

    public readonly struct OnRagdollRecovered
    {
        public readonly int EntityId;

        public OnRagdollRecovered(int entityId)
        {
            EntityId = entityId;
        }
    }

    public readonly struct OnMeleeHit
    {
        public readonly int AttackerId;
        public readonly int TargetId;
        public readonly float Damage;
        public readonly Vector3 HitPoint;
        public readonly Vector3 Force;

        public OnMeleeHit(int attackerId, int targetId, float damage, Vector3 hitPoint, Vector3 force)
        {
            AttackerId = attackerId;
            TargetId = targetId;
            Damage = damage;
            HitPoint = hitPoint;
            Force = force;
        }
    }

    public readonly struct OnProjectileHit
    {
        public readonly int ProjectileId;
        public readonly int TargetId;
        public readonly float Damage;
        public readonly Vector3 HitPoint;
        public readonly Vector3 Force;

        public OnProjectileHit(int projectileId, int targetId, float damage, Vector3 hitPoint, Vector3 force)
        {
            ProjectileId = projectileId;
            TargetId = targetId;
            Damage = damage;
            HitPoint = hitPoint;
            Force = force;
        }
    }

    public readonly struct OnGrappleStarted
    {
        public readonly int GrabberId;
        public readonly int TargetId;

        public OnGrappleStarted(int grabberId, int targetId)
        {
            GrabberId = grabberId;
            TargetId = targetId;
        }
    }

    public readonly struct OnGrappleReleased
    {
        public readonly int GrabberId;
        public readonly int TargetId;
        public readonly Vector3 ThrowForce;

        public OnGrappleReleased(int grabberId, int targetId, Vector3 throwForce)
        {
            GrabberId = grabberId;
            TargetId = targetId;
            ThrowForce = throwForce;
        }
    }

    public readonly struct OnBoneCollisionImpact
    {
        public readonly int EntityId;
        public readonly int JointIndex;
        public readonly Vector3 Force;
        public readonly Vector3 ContactPoint;
        public readonly float ForceMagnitude;

        public OnBoneCollisionImpact(int entityId, int jointIndex, Vector3 force, Vector3 contactPoint, float forceMagnitude)
        {
            EntityId = entityId;
            JointIndex = jointIndex;
            Force = force;
            ContactPoint = contactPoint;
            ForceMagnitude = forceMagnitude;
        }
    }
}
