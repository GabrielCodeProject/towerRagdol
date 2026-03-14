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

        public float PrepareTimerDuration => _prepareTimerDuration;
        public IReadOnlyList<float> PrepareWarningThresholds => _prepareWarningThresholds;
        public float TransitionDuration => _transitionDuration;
        public int BossWaveInterval => _bossWaveInterval;
        public bool CanBuildDuringDefend => _canBuildDuringDefend;
        public bool CanCraftDuringDefend => _canCraftDuringDefend;
        public bool CanRepairDuringDefend => _canRepairDuringDefend;
    }
}
