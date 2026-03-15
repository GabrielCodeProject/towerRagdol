using NUnit.Framework;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Tests
{
    [TestFixture]
    public class RagdollEventsTests
    {
        [Test]
        public void OnRagdollActivated_Constructor_StoresValues()
        {
            var position = new Vector3(1f, 2f, 3f);
            var evt = new OnRagdollActivated(42, position);

            Assert.AreEqual(42, evt.EntityId);
            Assert.AreEqual(position, evt.Position);
        }

        [Test]
        public void OnRagdollDeactivated_Constructor_StoresEntityId()
        {
            var evt = new OnRagdollDeactivated(99);

            Assert.AreEqual(99, evt.EntityId);
        }

        [Test]
        public void OnRagdollForceApplied_Constructor_StoresValues()
        {
            var force = new Vector3(5f, 0f, 10f);
            var evt = new OnRagdollForceApplied(7, force, ForceType.Explosion);

            Assert.AreEqual(7, evt.EntityId);
            Assert.AreEqual(force, evt.Force);
            Assert.AreEqual(ForceType.Explosion, evt.ForceType);
        }

        [Test]
        public void OnRagdollRecoveryStarted_Constructor_StoresEntityId()
        {
            var evt = new OnRagdollRecoveryStarted(15);

            Assert.AreEqual(15, evt.EntityId);
        }

        [Test]
        public void OnRagdollRecovered_Constructor_StoresEntityId()
        {
            var evt = new OnRagdollRecovered(33);

            Assert.AreEqual(33, evt.EntityId);
        }

        [Test]
        public void OnMeleeHit_Constructor_StoresValues()
        {
            var hitPoint = new Vector3(0f, 1f, 0f);
            var force = new Vector3(10f, 0f, 0f);
            var evt = new OnMeleeHit(1, 2, 25f, hitPoint, force);

            Assert.AreEqual(1, evt.AttackerId);
            Assert.AreEqual(2, evt.TargetId);
            Assert.AreEqual(25f, evt.Damage);
            Assert.AreEqual(hitPoint, evt.HitPoint);
            Assert.AreEqual(force, evt.Force);
        }

        [Test]
        public void OnProjectileHit_Constructor_StoresValues()
        {
            var hitPoint = new Vector3(3f, 0f, 2f);
            var force = new Vector3(0f, 5f, 0f);
            var evt = new OnProjectileHit(10, 20, 50f, hitPoint, force);

            Assert.AreEqual(10, evt.ProjectileId);
            Assert.AreEqual(20, evt.TargetId);
            Assert.AreEqual(50f, evt.Damage);
            Assert.AreEqual(hitPoint, evt.HitPoint);
            Assert.AreEqual(force, evt.Force);
        }

        [Test]
        public void OnGrappleStarted_Constructor_StoresValues()
        {
            var evt = new OnGrappleStarted(5, 8);

            Assert.AreEqual(5, evt.GrabberId);
            Assert.AreEqual(8, evt.TargetId);
        }

        [Test]
        public void OnGrappleReleased_Constructor_StoresValues()
        {
            var throwForce = new Vector3(0f, 10f, 20f);
            var evt = new OnGrappleReleased(5, 8, throwForce);

            Assert.AreEqual(5, evt.GrabberId);
            Assert.AreEqual(8, evt.TargetId);
            Assert.AreEqual(throwForce, evt.ThrowForce);
        }

        [Test]
        public void OnBoneCollisionImpact_Constructor_StoresValues()
        {
            var force = new Vector3(3f, -1f, 0f);
            var contactPoint = new Vector3(0f, 1.5f, 2f);
            var evt = new OnBoneCollisionImpact(42, 3, force, contactPoint, 5.5f);

            Assert.AreEqual(42, evt.EntityId);
            Assert.AreEqual(3, evt.JointIndex);
            Assert.AreEqual(force, evt.Force);
            Assert.AreEqual(contactPoint, evt.ContactPoint);
            Assert.AreEqual(5.5f, evt.ForceMagnitude, 0.0001f);
        }
    }
}
