using NUnit.Framework;
using RagdollRealms.Core.Events;

namespace RagdollRealms.Tests
{
    [TestFixture]
    public class UIEventsTests
    {
        [Test]
        public void OnPlayerHealthChanged_Constructor_StoresValues()
        {
            var evt = new OnPlayerHealthChanged(1, 75f, 100f);

            Assert.AreEqual(1, evt.PlayerId);
            Assert.AreEqual(75f, evt.Current);
            Assert.AreEqual(100f, evt.Max);
        }

        [Test]
        public void OnPlayerManaChanged_Constructor_StoresValues()
        {
            var evt = new OnPlayerManaChanged(2, 30f, 50f);

            Assert.AreEqual(2, evt.PlayerId);
            Assert.AreEqual(30f, evt.Current);
            Assert.AreEqual(50f, evt.Max);
        }

        [Test]
        public void OnPlayerXpChanged_Constructor_StoresValues()
        {
            var evt = new OnPlayerXpChanged(1, 5, 250f, 500f);

            Assert.AreEqual(1, evt.PlayerId);
            Assert.AreEqual(5, evt.Level);
            Assert.AreEqual(250f, evt.Xp);
            Assert.AreEqual(500f, evt.XpToNextLevel);
        }

        [Test]
        public void OnSpellSlotUpdated_Constructor_StoresValues()
        {
            var evt = new OnSpellSlotUpdated(1, 2, "Fireball", 1.5f, 3f, 25f, false);

            Assert.AreEqual(1, evt.PlayerId);
            Assert.AreEqual(2, evt.SlotIndex);
            Assert.AreEqual("Fireball", evt.SpellName);
            Assert.AreEqual(1.5f, evt.CooldownRemaining);
            Assert.AreEqual(3f, evt.CooldownTotal);
            Assert.AreEqual(25f, evt.ManaCost);
            Assert.IsFalse(evt.IsReady);
        }

        [Test]
        public void OnSpellSlotUpdated_IsReady_WhenCooldownZero()
        {
            var evt = new OnSpellSlotUpdated(1, 0, "Heal", 0f, 5f, 10f, true);

            Assert.IsTrue(evt.IsReady);
            Assert.AreEqual(0f, evt.CooldownRemaining);
        }
    }
}
