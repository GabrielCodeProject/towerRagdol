namespace RagdollRealms.Core
{
    public interface IConfigurableProjectile
    {
        void Configure(float damage, float speed, float knockback, ForceType forceType);
    }
}
