# Epic E19: Extensibility Architecture
**Layer:** 0 (Foundation — no dependencies)
**Design Doc Sections:** 19.1–19.7
**Depends On:** None

---

## E19-T01: Create Content Registry base system
- **Size:** L (3-4h)
- **Depends on:** None
- **Description:** Implement a generic `ContentRegistry<T>` base class that auto-discovers ScriptableObjects from designated asset folders at startup using `Resources.LoadAll` or Addressables scanning. Expose `GetByID()`, `GetAll()`, `GetByTier()`, `GetByTag()` query methods. Support runtime hot-reload for development.
- **Test:** Create a test SO type, place in content folder, verify auto-registration and retrieval by ID/tier/tag.
- **Design doc ref:** Section 19.2

## E19-T02: Implement Event Bus / Message System
- **Size:** L (3-4h)
- **Depends on:** None
- **Description:** Build a decoupled Event Bus system using a static `EventBus` class with `Subscribe<T>()`, `Unsubscribe<T>()`, `Publish<T>()` methods. Define core event types: `OnEnemyKilled`, `OnWaveStart`, `OnWaveEnd`, `OnBossDefeated`, `OnCoreHit`, `OnItemCrafted`, `OnEnhancementAttempt`, `OnPlayerDowned`, `OnPhaseChanged`. Events carry typed payloads.
- **Test:** Subscribe to test event, publish it, verify handler fires with correct payload. Test multiple subscribers, unsubscribe, and event ordering.
- **Design doc ref:** Section 19.2

## E19-T03: Create ScriptableObject base templates
- **Size:** L (4h)
- **Depends on:** E19-T01
- **Description:** Create base SO templates for all content types: `EnemyDefinition`, `TowerDefinition`, `ItemDefinition`, `SpellDefinition`, `RecipeDefinition`, `BuildingPieceDefinition`, `MapDefinition`, `BossDefinition`, `ClassDefinition`. Each SO has: unique ID, display name, description, icon, tier, tags list. Use a common `ContentDefinition` base class.
- **Test:** Create one instance of each SO type, verify all fields serialize correctly and auto-register in ContentRegistry.
- **Design doc ref:** Section 19.1

## E19-T04: Implement Tag & Category System
- **Size:** M (2-3h)
- **Depends on:** E19-T03
- **Description:** Build a string-based tag system on `ContentDefinition` base. Tags stored as `List<string>`. Implement `TagQuery` utility with methods: `HasTag()`, `HasAllTags()`, `HasAnyTag()`, `GetByTags()`. Tags used for filtering, wave composition, loot tables, and UI. No enums — pure string tags addable without code changes.
- **Test:** Tag test SOs with `[weapon, melee, fire, tier3]`, query "all fire items", "all tier3 melee weapons", verify correct results.
- **Design doc ref:** Section 19.4

## E19-T05: Create Behavior Plugin interfaces
- **Size:** M (2-3h)
- **Depends on:** None
- **Description:** Define core plugin interfaces: `IEnemyBehavior` (Think, Act, OnHit, OnDeath), `ITargetingStrategy` (SelectTarget from list), `ISpellEffect` (Cast, OnHit, OnExpire), `IComedyFeature` (OnActivate, OnDeactivate, GetTags), `IBossPhase` (Enter, Update, Exit, IsComplete), `ISkillEffect` (Activate, Deactivate, GetDescription). Each interface designed for strategy pattern swapping.
- **Test:** Create mock implementations of each interface, verify they can be assigned to SO references and invoked polymorphically.
- **Design doc ref:** Section 19.2, 19.3

## E19-T06: Implement Scaling & Balance Framework
- **Size:** M (2-3h)
- **Depends on:** E19-T01
- **Description:** Create `BalanceProfile` SO that holds all balance configuration: damage formula curves, wave difficulty curves, enhancement success rate tables, boss scaling multipliers, performance budgets. Implement `BalanceManager` that loads active profile and exposes `GetValue(key)`. Support runtime profile swapping ("Normal", "Hard", "Endless", "Party Mode"). Values reloadable without rebuild.
- **Test:** Create two balance profiles with different values, swap at runtime, verify game systems read updated values.
- **Design doc ref:** Section 19.5

## E19-T07: Create typed registries for each content type
- **Size:** M (2-3h)
- **Depends on:** E19-T01, E19-T03
- **Description:** Create concrete registries inheriting from `ContentRegistry<T>`: `EnemyRegistry`, `TowerRegistry`, `ItemRegistry`, `SpellRegistry`, `RecipeRegistry`, `BuildingRegistry`, `MapRegistry`, `BossRegistry`, `ClassRegistry`, `LootTableRegistry`. Each registry lives as a singleton MonoBehaviour on a persistent GameManager object.
- **Test:** Populate each registry with 2-3 test SOs, verify type-safe queries work (e.g., `TowerRegistry.GetByTier(2)` returns only tier 2 towers).
- **Design doc ref:** Section 19.2

## E19-T08: Map system extensibility foundation
- **Size:** M (2-3h)
- **Depends on:** E19-T03
- **Description:** Flesh out `MapDefinition` SO to contain: Core position, spawn point slot list, biome type, resource distribution rules (tier zones), POI slot definitions, boss altar locations, ambient settings. Maps reference enemy sets and tower types available. Support procedural POI/resource placement from rules. Map selection UI data support.
- **Test:** Create a test MapDefinition SO with all fields populated, verify it can be loaded and all fields accessed programmatically.
- **Design doc ref:** Section 19.6

## E19-T09: Class system extensibility foundation
- **Size:** M (2-3h)
- **Depends on:** E19-T03, E19-T05
- **Description:** Flesh out `ClassDefinition` SO: base stats, stat growth curves, passive ability references, starting equipment references. Create `SkillTreeDefinition` SO: tree layout, node connections, skill SO references. Each skill = `ISkillEffect` script + `SkillDefinition` SO. No class-locked content — classes provide passive bonuses favoring certain playstyles.
- **Test:** Create a test Warrior ClassDefinition + simple SkillTreeDefinition, verify data loads and skills can be queried.
- **Design doc ref:** Section 19.7

## E19-T10: Loot Table system
- **Size:** M (2-3h)
- **Depends on:** E19-T01, E19-T04
- **Description:** Create `LootTableDefinition` SO with weighted drop entries. Each entry references an `ItemDefinition` (or resource type) with: weight, min/max quantity, tag-based conditions (e.g., only drop if enemy has tag "elite"), tier restrictions. Implement `LootTableResolver` that rolls drops using weighted random. Support nested loot tables (table references another table).
- **Test:** Create a loot table with 5 entries of varying weights, roll 1000 times, verify distribution matches weights within statistical tolerance.
- **Design doc ref:** Section 19.3

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E19-T01 | L | None |
| E19-T02 | L | None |
| E19-T03 | L | E19-T01 |
| E19-T04 | M | E19-T03 |
| E19-T05 | M | None |
| E19-T06 | M | E19-T01 |
| E19-T07 | M | E19-T01, E19-T03 |
| E19-T08 | M | E19-T03 |
| E19-T09 | M | E19-T03, E19-T05 |
| E19-T10 | M | E19-T01, E19-T04 |

**Total: 10 tasks, ~28-32h**
