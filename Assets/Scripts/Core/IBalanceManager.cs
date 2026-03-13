namespace RagdollRealms.Core
{
    public interface IBalanceManager
    {
        float GetValue(string key, float defaultValue = 0f);
        float EvaluateCurve(string key, float t, float defaultValue = 0f);
        void SetActiveProfile(string profileId);
        string ActiveProfileId { get; }
    }
}
