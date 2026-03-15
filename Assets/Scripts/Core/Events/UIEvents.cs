namespace RagdollRealms.Core.Events
{
    public readonly struct OnPlayerHealthChanged
    {
        public readonly int PlayerId;
        public readonly float Current;
        public readonly float Max;

        public OnPlayerHealthChanged(int playerId, float current, float max)
        {
            PlayerId = playerId;
            Current = current;
            Max = max;
        }
    }

    public readonly struct OnPlayerManaChanged
    {
        public readonly int PlayerId;
        public readonly float Current;
        public readonly float Max;

        public OnPlayerManaChanged(int playerId, float current, float max)
        {
            PlayerId = playerId;
            Current = current;
            Max = max;
        }
    }

    public readonly struct OnPlayerXpChanged
    {
        public readonly int PlayerId;
        public readonly int Level;
        public readonly float Xp;
        public readonly float XpToNextLevel;

        public OnPlayerXpChanged(int playerId, int level, float xp, float xpToNextLevel)
        {
            PlayerId = playerId;
            Level = level;
            Xp = xp;
            XpToNextLevel = xpToNextLevel;
        }
    }

    public readonly struct OnSpellSlotUpdated
    {
        public readonly int PlayerId;
        public readonly int SlotIndex;
        public readonly string SpellName;
        public readonly float CooldownRemaining;
        public readonly float CooldownTotal;
        public readonly float ManaCost;
        public readonly bool IsReady;

        public OnSpellSlotUpdated(int playerId, int slotIndex, string spellName,
            float cooldownRemaining, float cooldownTotal, float manaCost, bool isReady)
        {
            PlayerId = playerId;
            SlotIndex = slotIndex;
            SpellName = spellName;
            CooldownRemaining = cooldownRemaining;
            CooldownTotal = cooldownTotal;
            ManaCost = manaCost;
            IsReady = isReady;
        }
    }
}
