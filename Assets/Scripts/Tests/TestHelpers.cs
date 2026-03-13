using UnityEngine;
using RagdollRealms.Core;

namespace RagdollRealms.Tests
{
    public class TestRagdollConfig : IRagdollConfig
    {
        public float DefaultSpring { get; set; } = 500f;
        public float DefaultDamper { get; set; } = 50f;
        public float MaxSpringForce { get; set; } = 1000f;

        public float HipMass { get; set; } = 5f;
        public float TorsoMass { get; set; } = 3f;
        public float LimbMass { get; set; } = 1f;
        public float HeadMass { get; set; } = 1.5f;

        public float RecoveryDelay { get; set; } = 1.5f;
        public float RecoveryDuration { get; set; } = 1f;
        public float MinVitalityToRecover { get; set; } = 0.1f;

        public float KnockbackMultiplier { get; set; } = 1f;
        public float RagdollForceThreshold { get; set; } = 10f;
        public float ArmorKnockbackReduction { get; set; } = 0f;

        public float LodFullDistance { get; set; } = 20f;
        public float LodReducedDistance { get; set; } = 40f;
        public float LodDisableDistance { get; set; } = 60f;

        public float BalanceSensitivity { get; set; } = 1f;
        public float GetUpSpringMultiplier { get; set; } = 2f;
    }

    public static class TestServiceHelpers
    {
        public static ServiceLocator SetUpServiceLocator()
        {
            var go = new GameObject("ServiceLocator");
            var locator = go.AddComponent<ServiceLocator>();

            var field = typeof(ServiceLocator).GetField("Instance",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (field != null)
                field.SetValue(null, locator);

            var instanceProp = typeof(ServiceLocator).GetProperty("Instance",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            return locator;
        }

        public static void TearDownServiceLocator()
        {
            if (ServiceLocator.Instance != null)
                Object.DestroyImmediate(ServiceLocator.Instance.gameObject);
        }

        public static void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
    }
}
