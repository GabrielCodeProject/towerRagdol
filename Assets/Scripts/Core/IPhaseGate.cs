namespace RagdollRealms.Core
{
    public static class ActionIds
    {
        public const string Build = "build";
        public const string Craft = "craft";
        public const string Gather = "gather";
        public const string Explore = "explore";
        public const string Repair = "repair";
        public const string Enhance = "enhance";
    }

    public interface IPhaseGate
    {
        bool CanBuild();
        bool CanCraft();
        bool CanGather();
        bool CanExplore();
        bool CanRepair();
        bool CanEnhance();
        bool IsActionAllowed(string actionId);
    }
}
