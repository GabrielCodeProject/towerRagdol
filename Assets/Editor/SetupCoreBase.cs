using UnityEngine;
using UnityEditor;
using RagdollRealms.Content;

namespace RagdollRealms.Editor
{
    public static class SetupCoreBase
    {
    public static void Execute()
    {
        // Ensure Resources/CoreConfigs folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Resources/CoreConfigs"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateFolder("Assets/Resources", "CoreConfigs");
        }

        // Create CoreDefinition SO
        var core = ScriptableObject.CreateInstance<CoreDefinition>();

        // Set base fields via SerializedObject
        var path = "Assets/Resources/CoreConfigs/DefaultCore.asset";
        AssetDatabase.CreateAsset(core, path);

        var so = new SerializedObject(core);
        so.FindProperty("_id").stringValue = "default-core";
        so.FindProperty("_displayName").stringValue = "Crystal Core";
        so.FindProperty("_description").stringValue = "The heart of your base. Protect it at all costs.";
        so.FindProperty("_tier").intValue = 1;
        so.FindProperty("_baseHp").floatValue = 1000f;
        so.FindProperty("_healingAuraRadius").floatValue = 12f;
        so.FindProperty("_healingAuraRate").floatValue = 3f;
        so.FindProperty("_alarmBaseRange").floatValue = 25f;

        // Setup 4 upgrade tiers
        var tiers = so.FindProperty("_upgradeTiers");
        tiers.arraySize = 4;

        // Tier 1: +25% HP, small shield
        SetTier(tiers.GetArrayElementAtIndex(0), 1.25f, 100f, 1.1f, 1.1f, 1.1f);
        // Tier 2: +50% HP, medium shield
        SetTier(tiers.GetArrayElementAtIndex(1), 1.5f, 200f, 1.25f, 1.25f, 1.25f);
        // Tier 3: +75% HP, large shield
        SetTier(tiers.GetArrayElementAtIndex(2), 1.75f, 350f, 1.5f, 1.5f, 1.5f);
        // Tier 4: +100% HP, max shield
        SetTier(tiers.GetArrayElementAtIndex(3), 2.0f, 500f, 2.0f, 2.0f, 2.0f);

        so.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[SetupCoreBase] Created DefaultCore.asset at " + path);
    }

    private static void SetTier(SerializedProperty tier, float hp, float shield, float auraR, float auraRate, float alarm)
    {
        tier.FindPropertyRelative("HpMultiplier").floatValue = hp;
        tier.FindPropertyRelative("ShieldCapacity").floatValue = shield;
        tier.FindPropertyRelative("AuraRadiusMultiplier").floatValue = auraR;
        tier.FindPropertyRelative("AuraRateMultiplier").floatValue = auraRate;
        tier.FindPropertyRelative("AlarmRangeMultiplier").floatValue = alarm;
    }
    }
}
