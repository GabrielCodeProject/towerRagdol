using NUnit.Framework;
using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Tests
{
    [TestFixture]
    public class PerformanceBudgetTests
    {
        private GameObject _go;
        private PerformanceBudgetManager _budget;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("PerformanceBudget");
            _budget = _go.AddComponent<PerformanceBudgetManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_go);
        }

        [Test]
        public void CanSpawnRagdoll_WhenUnderLimit_ReturnsTrue()
        {
            Assert.IsTrue(_budget.CanSpawnRagdoll());
        }

        [Test]
        public void CanSpawnEnemy_WhenUnderLimit_ReturnsTrue()
        {
            Assert.IsTrue(_budget.CanSpawnEnemy());
        }

        [Test]
        public void MaxRagdolls_DefaultValue_IsThirtyTwo()
        {
            Assert.AreEqual(32, _budget.MaxRagdolls);
        }

        [Test]
        public void MaxEnemies_DefaultValue_IsForty()
        {
            Assert.AreEqual(40, _budget.MaxEnemies);
        }

        [Test]
        public void ActiveRagdolls_InitialValue_IsZero()
        {
            Assert.AreEqual(0, _budget.ActiveRagdolls);
        }

        [Test]
        public void ActiveEnemies_InitialValue_IsZero()
        {
            Assert.AreEqual(0, _budget.ActiveEnemies);
        }

        [Test]
        public void TrackRagdoll_PositiveDelta_IncrementsActiveCount()
        {
            _budget.TrackRagdoll(1);
            Assert.AreEqual(1, _budget.ActiveRagdolls);
        }

        [Test]
        public void TrackRagdoll_NegativeDelta_DecrementsActiveCount()
        {
            _budget.TrackRagdoll(5);
            _budget.TrackRagdoll(-2);
            Assert.AreEqual(3, _budget.ActiveRagdolls);
        }

        [Test]
        public void TrackRagdoll_BelowZero_ClampsAtZero()
        {
            _budget.TrackRagdoll(-10);
            Assert.AreEqual(0, _budget.ActiveRagdolls);
        }

        [Test]
        public void TrackEnemy_PositiveDelta_IncrementsActiveCount()
        {
            _budget.TrackEnemy(3);
            Assert.AreEqual(3, _budget.ActiveEnemies);
        }

        [Test]
        public void TrackEnemy_NegativeDelta_DecrementsActiveCount()
        {
            _budget.TrackEnemy(5);
            _budget.TrackEnemy(-3);
            Assert.AreEqual(2, _budget.ActiveEnemies);
        }

        [Test]
        public void TrackEnemy_BelowZero_ClampsAtZero()
        {
            _budget.TrackEnemy(-5);
            Assert.AreEqual(0, _budget.ActiveEnemies);
        }

        [Test]
        public void CanSpawnRagdoll_WhenAtMaxCapacity_ReturnsFalse()
        {
            _budget.TrackRagdoll(_budget.MaxRagdolls);
            Assert.IsFalse(_budget.CanSpawnRagdoll());
        }

        [Test]
        public void CanSpawnEnemy_WhenAtMaxCapacity_ReturnsFalse()
        {
            _budget.TrackEnemy(_budget.MaxEnemies);
            Assert.IsFalse(_budget.CanSpawnEnemy());
        }

        [Test]
        public void CanSpawnRagdoll_OneUnderMax_ReturnsTrue()
        {
            _budget.TrackRagdoll(_budget.MaxRagdolls - 1);
            Assert.IsTrue(_budget.CanSpawnRagdoll());
        }

        [Test]
        public void TrackRagdoll_MultipleIncrements_AccumulatesCorrectly()
        {
            _budget.TrackRagdoll(1);
            _budget.TrackRagdoll(1);
            _budget.TrackRagdoll(1);
            Assert.AreEqual(3, _budget.ActiveRagdolls);
        }

        [Test]
        public void TrackRagdoll_IncrementThenFullDecrement_ReturnsToZero()
        {
            _budget.TrackRagdoll(5);
            _budget.TrackRagdoll(-5);
            Assert.AreEqual(0, _budget.ActiveRagdolls);
        }
    }
}
