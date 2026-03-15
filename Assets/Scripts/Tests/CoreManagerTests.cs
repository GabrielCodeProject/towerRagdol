using NUnit.Framework;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Tests
{
    [TestFixture]
    public class CoreManagerTests
    {
        [Test]
        public void OnCoreHealthChanged_ContainsCorrectFields()
        {
            var evt = new OnCoreHealthChanged(750f, 1000f, 100f, 200f);

            Assert.AreEqual(750f, evt.CurrentHp);
            Assert.AreEqual(1000f, evt.MaxHp);
            Assert.AreEqual(100f, evt.ShieldHp);
            Assert.AreEqual(200f, evt.MaxShieldHp);
        }

        [Test]
        public void OnCoreDestroyed_IsEmptyStruct()
        {
            var evt = new OnCoreDestroyed();

            Assert.IsNotNull(evt);
        }

        [Test]
        public void OnCoreUpgraded_ContainsCorrectFields()
        {
            var evt = new OnCoreUpgraded(2, "core");

            Assert.AreEqual(2, evt.NewTier);
            Assert.AreEqual("core", evt.UpgradeType);
        }

        [Test]
        public void OnCoreShieldChanged_ContainsCorrectFields()
        {
            var evt = new OnCoreShieldChanged(150f, 200f);

            Assert.AreEqual(150f, evt.CurrentShield);
            Assert.AreEqual(200f, evt.MaxShield);
        }

        [Test]
        public void OnCoreAlarmTriggered_ContainsCorrectFields()
        {
            var direction = new UnityEngine.Vector3(1f, 0f, 0f);
            var evt = new OnCoreAlarmTriggered(direction, 15f, true);

            Assert.AreEqual(direction, evt.EnemyDirection);
            Assert.AreEqual(15f, evt.EnemyDistance);
            Assert.IsTrue(evt.IsPriority);
        }

        [Test]
        public void OnCoreAlarmTriggered_NonPriority()
        {
            var direction = new UnityEngine.Vector3(0f, 0f, -1f);
            var evt = new OnCoreAlarmTriggered(direction, 25f, false);

            Assert.IsFalse(evt.IsPriority);
            Assert.AreEqual(25f, evt.EnemyDistance);
        }

        [Test]
        public void OnCoreHealthChanged_ZeroHp_IsValid()
        {
            var evt = new OnCoreHealthChanged(0f, 1000f, 0f, 0f);

            Assert.AreEqual(0f, evt.CurrentHp);
            Assert.AreEqual(1000f, evt.MaxHp);
        }

        [Test]
        public void OnCoreShieldChanged_FullShield()
        {
            var evt = new OnCoreShieldChanged(200f, 200f);

            Assert.AreEqual(evt.CurrentShield, evt.MaxShield);
        }

        [Test]
        public void OnCoreShieldChanged_DepletedShield()
        {
            var evt = new OnCoreShieldChanged(0f, 200f);

            Assert.AreEqual(0f, evt.CurrentShield);
        }
    }
}
