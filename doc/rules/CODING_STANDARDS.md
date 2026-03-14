# Coding Standards — Ragdoll Realms

> **NON-NEGOTIABLE.** Every script follows these patterns. Violations get refactored before merge.

---

## 1. SSOT — Single Source of Truth

**CRITICAL: Before writing ANY new utility, helper, service, or feature, search the codebase for existing implementations first.**

- **Search first, code second.** One canonical location for every piece of data, logic, and configuration
- If a utility exists, **use it**. If it needs extension, **extend it** — NEVER create a parallel version
- Repositories = SSOT for domain data. SOs = SSOT for content. EventBus = SSOT for event contracts. ServiceLocator = SSOT for service discovery
- NEVER cache repository data in Controllers/Views, hardcode SO values, create duplicate events, or create secondary lookup mechanisms

---

## 2. Architecture: MVC (Manager → Repository + Controller → View)

| Layer | Responsibility | Allowed Dependencies |
|-------|---------------|---------------------|
| **Manager** | Orchestrates high-level game flow | Repositories, Controllers, EventBus |
| **Repository** | Owns/exposes data (Model layer) | ScriptableObjects, save data |
| **Controller** | Executes logic on world objects | Repositories, EventBus, Factories |
| **View** | Presentation only (UI, VFX, SFX) | EventBus (subscribe only) |

- Managers NEVER touch Views. Controllers NEVER reference Views. Views NEVER modify game state
- **Communication between layers is ALWAYS through EventBus.** No direct cross-references

---

## 3. Singleton — ONE Only: ServiceLocator

**There is exactly ONE singleton: `ServiceLocator`.** All managers, repositories, and services register via interfaces.

```csharp
ServiceLocator.Instance.Register<IWaveManager>(this);  // in Awake()
var waves = ServiceLocator.Instance.Get<IWaveManager>(); // to access
```

- NEVER use `public static Instance` on any other class
- NEVER use `GameObject.Find()` / `FindObjectOfType()` for service lookup
- NEVER use direct `GetComponent()` cross-references between unrelated systems

---

## 4. Spawning: Factory + Object Pool

**All object creation goes through Factories. No `Instantiate()` or `Destroy()` calls anywhere else.**

- All spawnable objects implement `ISpawnable` (Initialize/Reset)
- Factories read prefab references from ScriptableObjects, never hardcoded
- Factories integrate with `IPoolManager` from ServiceLocator

---

## 5. Communication: Event Bus

Decoupled publish/subscribe via `IEventBus`. Systems NEVER reference each other directly.

```csharp
public interface IEventBus
{
    void Subscribe<T>(Action<T> handler);
    void Unsubscribe<T>(Action<T> handler);
    void Publish<T>(T eventData);
}
```

- Event structs are immutable data — no methods, no MonoBehaviour references
- ALWAYS `Unsubscribe` in `OnDestroy()`
- Views subscribe. Controllers/Managers publish

---

## 6. State Machine: Game States & AI

Use `IState` (Enter/Update/Exit) + `StateMachine` for systems with clear state transitions: PhaseManager, Enemy AI, Player states, Boss phases.

---

## 7. Strategy Pattern: Pluggable Behaviors via SOs

Runtime-swappable behaviors as ScriptableObjects implementing interfaces (`ITargetingStrategy`, `IEnemyBehavior`, `ISpellEffect`, `IComedyFeature`, `IBossPhase`).

- Adding a new behavior = create SO + drop in folder. No code changes to existing systems

---

## 8. Command Pattern: Building Only

`ICommand` (Execute/Undo) with `CommandHistory` for building placement undo/redo. NOT used for combat, movement, or real-time systems.

---

## 9. MVP for UI

Presenter mediates between Model (data) and View (pure UI). Views have zero logic.

---

## 10. DOTween & MMFeedbacks — View Layer ONLY

- DOTween calls exist **exclusively** in Views and UI Presenters — NEVER in Controllers or Managers
- MMFeedbacks are cosmetic reactions only — NEVER put game logic inside them
- Kill tweens in `OnDestroy()`

---

## 11. Namespace Convention

```
RagdollRealms.Core              — ServiceLocator, EventBus, interfaces
RagdollRealms.Core.Events       — All event structs
RagdollRealms.Core.Data         — ContentDefinition base, shared SO types
RagdollRealms.Systems.{Name}    — Each system (Combat, Waves, Building, Towers, etc.)
RagdollRealms.Factories         — All factories
RagdollRealms.Repositories      — All repositories
RagdollRealms.Content           — SO definitions
RagdollRealms.Content.Strategies— Strategy SO implementations
RagdollRealms.UI.{Area}         — Presenters/Views (HUD, Menus, Feedback)
RagdollRealms.Networking        — Network manager, sync, RPCs
RagdollRealms.Tests             — Unit and integration tests
```

---

## 12. Assembly Definitions — Dependency Direction

```
UI → Core ← Systems → Repositories → Content
                ↓
            Factories → Content
                ↓
           Networking → Core, Systems
```

**Views NEVER import Systems. Systems NEVER import UI. Communication through EventBus in Core.**

---

## 13. ScriptableObject Architecture

All content is data-driven via SOs extending `ContentDefinition` (ID, DisplayName, Description, Icon, Tier, Tags).

**Validation rule:** Before implementing ANY system, ask: *"Can new content be added by ONLY creating a new ScriptableObject?"* If no → rework the architecture.

---

## 14. File Naming

`{System}Manager.cs`, `{Entity}Controller.cs`, `{Entity}Repository.cs`, `{Entity}Factory.cs`, `{Feature}View.cs`, `{Feature}Presenter.cs`, `I{Name}.cs`, `On{Event}.cs`, `{Type}Definition.cs`, `{Name}State.cs`, `{Action}Command.cs`

---

## 15. Forbidden Practices

| Practice | Why |
|----------|-----|
| `public static Instance` on anything except ServiceLocator | Hidden coupling, untestable |
| `GameObject.Find()` / `FindObjectOfType()` | Fragile, slow, breaks on rename |
| `Instantiate()` / `Destroy()` outside Factories | Bypasses pooling |
| Hardcoded IDs, enums, or switch statements for content | Violates extensibility |
| DOTween in Controllers/Managers | Mixing logic and presentation |
| Game logic inside MMFeedbacks | Feedbacks are cosmetic only |
| Direct references between Systems and UI | Must use EventBus |
| `Update()` polling when events exist | Subscribe to EventBus |
| Duplicating existing utility/logic | SSOT — search first, extend if needed |
