using NUnit.Framework;
using UnityEngine;
using RagdollRealms.Core;
using RagdollRealms.Content;

namespace RagdollRealms.Tests
{
    [TestFixture]
    public class RagdollConfigTests
    {
        private RagdollConfigDefinition _config;

        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<RagdollConfigDefinition>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_config);
        }

        [Test]
        public void RagdollConfigDefinition_CanBeCreated_ViaCreateInstance()
        {
            Assert.IsNotNull(_config);
        }

        [Test]
        public void RagdollConfigDefinition_ImplementsIRagdollConfig()
        {
            Assert.IsInstanceOf<IRagdollConfig>(_config);
        }

        [Test]
        public void RagdollConfigDefinition_DefaultSpring_IsCorrect()
        {
            Assert.AreEqual(500f, _config.DefaultSpring);
        }

        [Test]
        public void RagdollConfigDefinition_DefaultDamper_IsCorrect()
        {
            Assert.AreEqual(50f, _config.DefaultDamper);
        }

        [Test]
        public void RagdollConfigDefinition_MaxSpringForce_IsCorrect()
        {
            Assert.AreEqual(1000f, _config.MaxSpringForce);
        }

        [Test]
        public void RagdollConfigDefinition_HipMass_IsCorrect()
        {
            Assert.AreEqual(5f, _config.HipMass);
        }

        [Test]
        public void RagdollConfigDefinition_TorsoMass_IsCorrect()
        {
            Assert.AreEqual(3f, _config.TorsoMass);
        }

        [Test]
        public void RagdollConfigDefinition_LimbMass_IsCorrect()
        {
            Assert.AreEqual(1f, _config.LimbMass);
        }

        [Test]
        public void RagdollConfigDefinition_HeadMass_IsCorrect()
        {
            Assert.AreEqual(1.5f, _config.HeadMass);
        }

        [Test]
        public void RagdollConfigDefinition_RecoveryDelay_IsCorrect()
        {
            Assert.AreEqual(1.5f, _config.RecoveryDelay);
        }

        [Test]
        public void RagdollConfigDefinition_RecoveryDuration_IsCorrect()
        {
            Assert.AreEqual(1f, _config.RecoveryDuration);
        }

        [Test]
        public void RagdollConfigDefinition_MinVitalityToRecover_IsCorrect()
        {
            Assert.AreEqual(0.1f, _config.MinVitalityToRecover, 0.0001f);
        }

        [Test]
        public void RagdollConfigDefinition_KnockbackMultiplier_IsCorrect()
        {
            Assert.AreEqual(1f, _config.KnockbackMultiplier);
        }

        [Test]
        public void RagdollConfigDefinition_RagdollForceThreshold_IsCorrect()
        {
            Assert.AreEqual(10f, _config.RagdollForceThreshold);
        }

        [Test]
        public void RagdollConfigDefinition_ArmorKnockbackReduction_IsCorrect()
        {
            Assert.AreEqual(0f, _config.ArmorKnockbackReduction);
        }

        [Test]
        public void RagdollConfigDefinition_LodFullDistance_IsCorrect()
        {
            Assert.AreEqual(20f, _config.LodFullDistance);
        }

        [Test]
        public void RagdollConfigDefinition_LodReducedDistance_IsCorrect()
        {
            Assert.AreEqual(40f, _config.LodReducedDistance);
        }

        [Test]
        public void RagdollConfigDefinition_LodDisableDistance_IsCorrect()
        {
            Assert.AreEqual(60f, _config.LodDisableDistance);
        }

        [Test]
        public void RagdollConfigDefinition_BalanceSensitivity_IsCorrect()
        {
            Assert.AreEqual(1f, _config.BalanceSensitivity);
        }

        [Test]
        public void RagdollConfigDefinition_GetUpSpringMultiplier_IsCorrect()
        {
            Assert.AreEqual(2f, _config.GetUpSpringMultiplier);
        }

        [Test]
        public void TestRagdollConfig_ImplementsIRagdollConfig()
        {
            var testConfig = new TestRagdollConfig();
            Assert.IsInstanceOf<IRagdollConfig>(testConfig);
        }

        [Test]
        public void TestRagdollConfig_DefaultValues_MatchExpectedDefaults()
        {
            var testConfig = new TestRagdollConfig();

            Assert.AreEqual(500f, testConfig.DefaultSpring);
            Assert.AreEqual(50f, testConfig.DefaultDamper);
            Assert.AreEqual(1000f, testConfig.MaxSpringForce);
            Assert.AreEqual(5f, testConfig.HipMass);
            Assert.AreEqual(1f, testConfig.KnockbackMultiplier);
        }

        [Test]
        public void TestRagdollConfig_SetValues_AreReflected()
        {
            var testConfig = new TestRagdollConfig
            {
                DefaultSpring = 250f,
                KnockbackMultiplier = 2f
            };

            Assert.AreEqual(250f, testConfig.DefaultSpring);
            Assert.AreEqual(2f, testConfig.KnockbackMultiplier);
        }
    }
}
