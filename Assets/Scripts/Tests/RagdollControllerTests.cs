using NUnit.Framework;
using UnityEngine;
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

            TestServiceHelpers.SetPrivateField(_controller, "_config", _config);
            TestServiceHelpers.SetPrivateField(_controller, "_hipRigidbody", _rb1);

            // Manually populate the arrays that Awake() would gather, since Awake doesn't
            // run automatically in EditMode
            var joints = _root.GetComponentsInChildren<ConfigurableJoint>();
            var bodies = _root.GetComponentsInChildren<Rigidbody>();
            TestServiceHelpers.SetPrivateField(_controller, "_allJoints", joints);
            TestServiceHelpers.SetPrivateField(_controller, "_allBodies", bodies);

            // Initialize per-joint tracking (added for collision wobble support)
            var perJointMultipliers = new float[joints.Length];
            var jointIndexMap = new System.Collections.Generic.Dictionary<ConfigurableJoint, int>(joints.Length);
            for (int i = 0; i < joints.Length; i++)
            {
                perJointMultipliers[i] = 1f;
                jointIndexMap[joints[i]] = i;
            }
            TestServiceHelpers.SetPrivateField(_controller, "_perJointMultipliers", perJointMultipliers);
            TestServiceHelpers.SetPrivateField(_controller, "_jointIndexMap", jointIndexMap);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_root);
            Object.DestroyImmediate(_config);
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

        [Test]
        public void JointCount_ReturnsTotalJoints()
        {
            Assert.AreEqual(2, _controller.JointCount);
        }

        [Test]
        public void SetJointSpringMultiplier_PerJoint_AppliesOnlyToTargetJoint()
        {
            _controller.SetJointSpringMultiplier(1f); // global = 1
            _controller.SetJointSpringMultiplier(0, 0.5f); // joint 0 = 0.5

            Assert.AreEqual(_config.DefaultSpring * 0.5f, _joint1.angularXDrive.positionSpring, 0.001f);
            Assert.AreEqual(_config.DefaultSpring * 1f, _joint2.angularXDrive.positionSpring, 0.001f);
        }

        [Test]
        public void SetJointSpringMultiplier_PerJoint_CombinesWithGlobal()
        {
            _controller.SetJointSpringMultiplier(0.5f); // global = 0.5
            _controller.SetJointSpringMultiplier(1, 0.5f); // joint 1 per-joint = 0.5

            // Joint 1: global(0.5) * perJoint(0.5) = 0.25
            Assert.AreEqual(_config.DefaultSpring * 0.25f, _joint2.angularXDrive.positionSpring, 0.001f);
            // Joint 0: global(0.5) * perJoint(1.0) = 0.5
            Assert.AreEqual(_config.DefaultSpring * 0.5f, _joint1.angularXDrive.positionSpring, 0.001f);
        }

        [Test]
        public void GetJointSpringMultiplier_ReturnsSetValue()
        {
            _controller.SetJointSpringMultiplier(0, 0.3f);
            Assert.AreEqual(0.3f, _controller.GetJointSpringMultiplier(0), 0.0001f);
        }

        [Test]
        public void GetJointSpringMultiplier_InvalidIndex_ReturnsZero()
        {
            Assert.AreEqual(0f, _controller.GetJointSpringMultiplier(-1));
            Assert.AreEqual(0f, _controller.GetJointSpringMultiplier(99));
        }

        [Test]
        public void GetJointIndex_ReturnsCorrectIndex()
        {
            Assert.AreEqual(0, _controller.GetJointIndex(_joint1));
            Assert.AreEqual(1, _controller.GetJointIndex(_joint2));
        }

        [Test]
        public void GetJointIndex_UnknownJoint_ReturnsNegativeOne()
        {
            var otherGo = new GameObject("Other");
            var otherJoint = otherGo.AddComponent<ConfigurableJoint>();
            Assert.AreEqual(-1, _controller.GetJointIndex(otherJoint));
            Object.DestroyImmediate(otherGo);
        }

        [Test]
        public void Initialize_ResetsPerJointMultipliers()
        {
            _controller.SetJointSpringMultiplier(0, 0.3f);
            _controller.Initialize();

            Assert.AreEqual(_config.DefaultSpring, _joint1.angularXDrive.positionSpring, 0.001f);
        }

        [Test]
        public void Reset_ResetsPerJointMultipliers()
        {
            _controller.SetJointSpringMultiplier(0, 0.3f);
            _controller.Reset();

            Assert.AreEqual(1f, _controller.GetJointSpringMultiplier(0), 0.0001f);
        }
    }
}
