namespace RagdollRealms.Core
{
    public interface ICoreManager
    {
        float CurrentHp { get; }
        float MaxHp { get; }
        float ShieldHp { get; }
        float MaxShieldHp { get; }
        int CurrentTier { get; }
        bool IsDestroyed { get; }
        float HealingAuraRadius { get; }
        float HealingAuraRate { get; }
        float AlarmRange { get; }
        void TakeDamage(float damage, int attackerId);
        void Heal(float amount);
        bool UpgradeCore();
        void RechargeShield();
    }
}
