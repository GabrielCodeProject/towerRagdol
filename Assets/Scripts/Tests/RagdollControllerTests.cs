using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using RagdollRealms.Core;
using RagdollRealms.Content;
using RagdollRealms.Systems.Ragdoll;

namespace RagdollRealms.Tests
{
    [TestFixture]
    public class RagdollControllerTests
    {
        private GameObject _root;
        private GameObject _child1;
        private GameObject _child2;
        private Rigidbody _rb1;
        private Rigidbody _rb2;
        private ConfigurableJoint _joint1;
        private ConfigurableJoint _joint2;
        private RagdollController _controller;
        private RagdollConfigDefinition _config;

        [SetUp]
        public void SetUp()
        {
            _root = new GameObject("RagdollRoot");

            _child1 = new GameObject("Child1");
            _child1.transform.parent = _root.transform;
            _rb1 = _child1.AddComponent<Rigidbody>();
            _joint1 = _child1.AddComponent<ConfigurableJoint>();

            _child2 = new GameObject("Child2");
            _child2.transform.parent = _root.transform;
            _rb2 = _child2.AddComponent<Rigidbody>();
            _joint2 = _child2.AddComponent<ConfigurableJoint>();

            _controller = _root.AddComponent<RagdollController>();
            _config = ScriptableObject.CreateInstance<RagdollConfigDefinition>();

            SetPrivateField(_controller, "_config", _config);
            SetPrivateField(_controller, "_hipRigidbody", _rb1);

            // Manually populate the arrays that Awake() would gather, since Awake doesn't
            // run automatically in EditMode
            var joints = _root.GetComponentsInChildren<ConfigurableJoint>();
            var bodies = _root.GetComponentsInChildren<Rigidbody>();
            SetPrivateField(_controller, "_allJoints", joints);
            SetPrivateField(_controller, "_allBodies", bodies);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
            Object.DestroyImmediate(_config);
        }

        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, value);
        }

        [Test]
        public void SetKinematic_True_SetsAllBodiesKinematic()
        {
            _controller.SetKinematic(true);

            Assert.IsTrue(_rb1.isKinematic);
            Assert.IsTrue(_rb2.isKinematic);
        }

        [Test]
        public void SetKinematic_False_ClearsKinematicOnAllBodies()
        {
            _rb1.isKinematic = true;
            _rb2.isKinematic = true;

            _controller.SetKinematic(false);

            Assert.IsFalse(_rb1.isKinematic);
            Assert.IsFalse(_rb2.isKinematic);
        }

        [Test]
        public void SetJointSpringMultiplier_One_AppliesDefaultSpring()
        {
            _controller.SetJointSpringMultiplier(1f);

            Assert.AreEqual(_config.DefaultSpring * 1f, _joint1.angularXDrive.positionSpring, 0.001f);
            Assert.AreEqual(_config.DefaultSpring * 1f, _joint2.angularXDrive.positionSpring, 0.001f);
        }

        [Test]
        public void SetJointSpringMultiplier_Half_AppliesHalfSpring()
        {
            _controller.SetJointSpringMultiplier(0.5f);

            Assert.AreEqual(_config.DefaultSpring * 0.5f, _joint1.angularXDrive.positionSpring, 0.001f);
            Assert.AreEqual(_config.DefaultSpring * 0.5f, _joint2.angularXDrive.positionSpring, 0.001f);
        }

        [Test]
        public void SetJointSpringMultiplier_SetsAngularYZDrive()
        {
            _controller.SetJointSpringMultiplier(1f);

            Assert.AreEqual(_config.DefaultSpring, _joint1.angularYZDrive.positionSpring, 0.001f);
            Assert.AreEqual(_config.DefaultSpring, _joint2.angularYZDrive.positionSpring, 0.001f);
        }

        [Test]
        public void SetJointSpringMultiplier_SetsDamperFromConfig()
        {
            _controller.SetJointSpringMultiplier(1f);

            Assert.AreEqual(_config.DefaultDamper, _joint1.angularXDrive.positionDamper, 0.001f);
        }

        [Test]
        public void SetJointSpringMultiplier_SetsMaxForceFromConfig()
        {
            _controller.SetJointSpringMultiplier(1f);

            Assert.AreEqual(_config.MaxSpringForce, _joint1.angularXDrive.maximumForce, 0.001f);
        }

        [Test]
        public void Initialize_SetsIsRagdollingFalse()
        {
            _controller.IsRagdolling = true;
            _controller.Initialize();

            Assert.IsFalse(_controller.IsRagdolling);
        }

        [Test]
        public void Initialize_SetsAllBodiesNonKinematic()
        {
            _rb1.isKinematic = true;
            _rb2.isKinematic = true;

            _controller.Initialize();

            Assert.IsFalse(_rb1.isKinematic);
            Assert.IsFalse(_rb2.isKinematic);
        }

        [Test]
        public void Initialize_SetsJointSpringToDefault()
        {
            _controller.Initialize();

            Assert.AreEqual(_config.DefaultSpring, _joint1.angularXDrive.positionSpring, 0.001f);
        }

        [Test]
        public void Reset_SetsAllBodiesKinematic()
        {
            _controller.Reset();

            Assert.IsTrue(_rb1.isKinematic);
            Assert.IsTrue(_rb2.isKinematic);
        }

        [Test]
        public void Reset_ZerosLinearVelocityOnAllBodies()
        {
            _rb1.isKinematic = false;
            _rb2.isKinematic = false;
            _rb1.linearVelocity = new Vector3(5f, 10f, 0f);
            _rb2.linearVelocity = new Vector3(0f, 3f, 2f);

            _controller.Reset();

            Assert.AreEqual(Vector3.zero, _rb1.linearVelocity);
            Assert.AreEqual(Vector3.zero, _rb2.linearVelocity);
        }

        [Test]
        public void Reset_ZerosAngularVelocityOnAllBodies()
        {
            _rb1.isKinematic = false;
            _rb2.isKinematic = false;
            _rb1.angularVelocity = new Vector3(1f, 2f, 3f);
            _rb2.angularVelocity = new Vector3(4f, 5f, 6f);

            _controller.Reset();

            Assert.AreEqual(Vector3.zero, _rb1.angularVelocity);
            Assert.AreEqual(Vector3.zero, _rb2.angularVelocity);
        }

        [Test]
        public void HipRigidbody_ReturnsConfiguredBody()
        {
            Assert.AreEqual(_rb1, _controller.HipRigidbody);
        }

        [Test]
        public void Config_ReturnsConfiguredInstance()
        {
            Assert.AreEqual(_config, _controller.Config);
        }

        [Test]
        public void IsRagdolling_DefaultValue_IsFalse()
        {
            Assert.IsFalse(_controller.IsRagdolling);
        }

        [Test]
        public void IsRagdolling_CanBeSetToTrue()
        {
            _controller.IsRagdolling = true;
            Assert.IsTrue(_controller.IsRagdolling);
        }

        [Test]
        public void AllBodies_ContainsBothChildBodies()
        {
            Assert.AreEqual(2, _controller.AllBodies.Count);
        }

        [Test]
        public void AllJoints_ContainsBothChildJoints()
        {
            Assert.AreEqual(2, _controller.AllJoints.Count);
        }
    }
}
