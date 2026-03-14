using RagdollRealms.Content;
using RagdollRealms.Core;
using RagdollRealms.Core.Data;
using UnityEngine;

namespace RagdollRealms.Systems
{
    public class ContentRegistryManager : MonoBehaviour
    {
        private void Awake()
        {
            RegisterAll();
        }

        private void RegisterAll()
        {
            RegisterRegistry<EnemyDefinition>("Enemies");
            RegisterRegistry<TowerDefinition>("Towers");
            RegisterRegistry<ItemDefinition>("Items");
            RegisterRegistry<SpellDefinition>("Spells");
            RegisterRegistry<RecipeDefinition>("Recipes");
            RegisterRegistry<BuildingPieceDefinition>("Buildings");
            RegisterRegistry<MapDefinition>("Maps");
            RegisterRegistry<BossDefinition>("Bosses");
            RegisterRegistry<ClassDefinition>("Classes");
            RegisterRegistry<SkillDefinition>("Skills");
            RegisterRegistry<SkillTreeDefinition>("SkillTrees");
            RegisterRegistry<LootTableDefinition>("LootTables");
            RegisterRegistry<BalanceProfileDefinition>("Balance");
            RegisterRegistry<RagdollConfigDefinition>("RagdollConfigs");
            RegisterRegistry<PhaseConfigDefinition>("PhaseConfigs");
            RegisterRegistry<PlayerConfigDefinition>("PlayerConfigs");
        }

        private void RegisterRegistry<T>(string resourcePath) where T : ContentDefinition
        {
            var registry = new ContentRegistry<T>();
            registry.LoadFromResources(resourcePath);
            ServiceLocator.Instance.Register<IContentRegistry<T>>(registry);
        }

        private void OnDestroy()
        {
            if (ServiceLocator.Instance == null) return;

            ServiceLocator.Instance.Unregister<IContentRegistry<EnemyDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<TowerDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<ItemDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<SpellDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<RecipeDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<BuildingPieceDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<MapDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<BossDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<ClassDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<SkillDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<SkillTreeDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<LootTableDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<BalanceProfileDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<RagdollConfigDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<PhaseConfigDefinition>>();
            ServiceLocator.Instance.Unregister<IContentRegistry<PlayerConfigDefinition>>();
        }
    }
}

