using NUnit.Framework;
using System;
using RagdollRealms.Core;

namespace RagdollRealms.Tests
{
    [TestFixture]
    public class ForceTypeTests
    {
        [Test]
        public void ForceType_HasExactlyThreeValues()
        {
            var values = Enum.GetValues(typeof(ForceType));
            Assert.AreEqual(3, values.Length);
        }

        [Test]
        public void ForceType_Impact_HasValueZero()
        {
            Assert.AreEqual(0, (int)ForceType.Impact);
        }

        [Test]
        public void ForceType_Explosion_HasValueOne()
        {
            Assert.AreEqual(1, (int)ForceType.Explosion);
        }

        [Test]
        public void ForceType_Sustained_HasValueTwo()
        {
            Assert.AreEqual(2, (int)ForceType.Sustained);
        }

        [Test]
        public void ForceType_ContainsImpact()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(ForceType), "Impact"));
        }

        [Test]
        public void ForceType_ContainsExplosion()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(ForceType), "Explosion"));
        }

        [Test]
        public void ForceType_ContainsSustained()
        {
            Assert.IsTrue(Enum.IsDefined(typeof(ForceType), "Sustained"));
        }
    }
}
