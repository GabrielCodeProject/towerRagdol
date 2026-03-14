using RagdollRealms.Content;
using RagdollRealms.Core;

namespace RagdollRealms.Systems.Phases
{
    public class PhaseGate : IPhaseGate
    {
        private readonly IPhaseManager _phaseManager;
        private readonly PhaseConfigDefinition _config;

        public PhaseGate(IPhaseManager phaseManager, PhaseConfigDefinition config)
        {
            _phaseManager = phaseManager;
            _config = config;
        }

        public bool CanBuild()
        {
            return _phaseManager.IsInPhase(PhaseType.Prepare)
                || (_phaseManager.IsInPhase(PhaseType.Defend) && _config.CanBuildDuringDefend);
        }

        public bool CanCraft()
        {
            return _phaseManager.IsInPhase(PhaseType.Prepare)
                || (_phaseManager.IsInPhase(PhaseType.Defend) && _config.CanCraftDuringDefend);
        }

        public bool CanGather()
        {
            return _phaseManager.IsInPhase(PhaseType.Prepare);
        }

        public bool CanExplore()
        {
            return _phaseManager.IsInPhase(PhaseType.Prepare);
        }

        public bool CanRepair()
        {
            return _phaseManager.IsInPhase(PhaseType.Prepare)
                || _phaseManager.IsInPhase(PhaseType.Transition)
                || (_phaseManager.IsInPhase(PhaseType.Defend) && _config.CanRepairDuringDefend);
        }

        public bool CanEnhance()
        {
            return _phaseManager.IsInPhase(PhaseType.Prepare);
        }

        public bool IsActionAllowed(string actionId)
        {
            return actionId switch
            {
                ActionIds.Build => CanBuild(),
                ActionIds.Craft => CanCraft(),
                ActionIds.Gather => CanGather(),
                ActionIds.Explore => CanExplore(),
                ActionIds.Repair => CanRepair(),
                ActionIds.Enhance => CanEnhance(),
                _ => false
            };
        }
    }
}
