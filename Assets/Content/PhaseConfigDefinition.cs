using System.Collections.Generic;
using UnityEngine;
using RagdollRealms.Core.Data;

namespace RagdollRealms.Content
{
    [CreateAssetMenu(fileName = "NewPhaseConfig", menuName = "Ragdoll Realms/Content/Phase Config")]
    public class PhaseConfigDefinition : ContentDefinition
    {
        [Header("Prepare Phase")]
        [SerializeField] private float _prepareTimerDuration = 180f;
        [SerializeField] private float[] _prepareWarningThresholds = { 30f, 10f };

        [Header("Transition Phase")]
        [SerializeField] private float _transitionDuration = 5f;

        [Header("Boss Wave")]
        [SerializeField] private int _bossWaveInterval = 5;

        [Header("Defend Phase Permissions")]
        [SerializeField] private bool _canBuildDuringDefend = false;
        [SerializeField] private bool _canCraftDuringDefend = false;
        [SerializeField] private bool _canRepairDuringDefend = true;

        [Header("Ready-Up")]
        [SerializeField, Range(0.5f, 1f)] private float _readyUpMajorityThreshold = 0.5f;

        [Header("Player Down / Revive")]
        [SerializeField] private float _reviveDuration = 3f;
        [SerializeField] private float _respawnTimerDuration = 15f;
        [SerializeField] private float _downedMoveSpeedMultiplier = 0.3f;

        [Header("Transition Report")]
        [SerializeField] private float _transitionReportDuration = 3f;

        [Header("Victory")]
        [Tooltip("Total waves to complete for victory. 0 = endless (no victory).")]
        [SerializeField] private int _maxWaves = 10;
        [SerializeField] private int _endlessModeWaveThreshold = 0;

        public float PrepareTimerDuration => _prepareTimerDuration;
        public IReadOnlyList<float> PrepareWarningThresholds => _prepareWarningThresholds;
        public float TransitionDuration => _transitionDuration;
        public int BossWaveInterval => _bossWaveInterval;
        public bool CanBuildDuringDefend => _canBuildDuringDefend;
        public bool CanCraftDuringDefend => _canCraftDuringDefend;
        public bool CanRepairDuringDefend => _canRepairDuringDefend;
        public float ReadyUpMajorityThreshold => _readyUpMajorityThreshold;
        public float ReviveDuration => _reviveDuration;
        public float RespawnTimerDuration => _respawnTimerDuration;
        public float DownedMoveSpeedMultiplier => _downedMoveSpeedMultiplier;
        public float TransitionReportDuration => _transitionReportDuration;
        public int MaxWaves => _maxWaves;
        public int EndlessModeWaveThreshold => _endlessModeWaveThreshold;
    }
}
