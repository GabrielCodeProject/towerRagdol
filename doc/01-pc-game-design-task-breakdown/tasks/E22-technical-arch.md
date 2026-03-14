# Epic E22: Technical Architecture
**Layer:** 0 (Foundation — no dependencies)
**Design Doc Sections:** 22.1–22.3
**Depends On:** None

---

## E22-T01: Project scaffolding and folder structure
- **Size:** M (2h)
- **Depends on:** None
- **Description:** Set up Unity project with organized folder structure: `Content/` (SO assets by type), `Scripts/Core/`, `Scripts/Systems/`, `Scripts/Interfaces/`, `Scripts/UI/`, `Scripts/Networking/`, `Prefabs/`, `Scenes/`. Configure assembly definitions for compile-time modularity. Set up `.gitignore`, project settings for PC build target.
- **Test:** Project opens cleanly, assembly definitions compile, folder structure matches architecture plan.
- **Design doc ref:** Section 22

## E22-T02: Networking foundation — client-server model
- **Size:** XL (4-6h)
- **Depends on:** E22-T01
- **Description:** Set up networking layer using Mirror (or Photon Fusion 2). Implement: host/join session flow, player connection/disconnection handling, network manager with scene management, basic RPCs for testing. Configure server-authoritative model: server owns game state (damage, loot, Core HP), clients own visuals (ragdoll, particles, UI).
- **Test:** Two clients can connect to a host session, sync a test object's position, disconnect cleanly.
- **Design doc ref:** Section 22.1

## E22-T03: Object pooling system
- **Size:** M (2-3h)
- **Depends on:** E22-T01
- **Description:** Implement generic `ObjectPool<T>` for spawned entities: enemies, projectiles, VFX, loot drops. Pool pre-warms configurable count, auto-grows when exhausted, recycles on despawn. Integrate with `NetworkManager` for networked object spawning. Pool manager tracks all pools with debug stats.
- **Test:** Pool 20 test objects, spawn/despawn 100 times, verify no GC allocations, verify pool size stays within bounds.
- **Design doc ref:** Section 22.2

## E22-T04: Performance budget manager
- **Size:** M (2h)
- **Depends on:** E22-T01
- **Description:** Create `PerformanceBudget` singleton that enforces hard limits: max 32 active ragdoll bodies, max 40 active enemies, physics solver at 12 iterations. Expose `CanSpawnRagdoll()`, `CanSpawnEnemy()` gate methods. Track active counts and provide debug overlay showing current usage vs budget.
- **Test:** Attempt to exceed ragdoll limit, verify gate blocks spawning. Despawn one, verify new spawn allowed.
- **Design doc ref:** Section 22.2

## E22-T05: Chunk-based world loading
- **Size:** L (3-4h)
- **Depends on:** E22-T02
- **Description:** Implement chunk-based map loading: divide map into grid chunks, load/unload based on player proximity. Use additive scene loading or addressables. Grid-based building zones around Core always loaded. Sync chunk state across network (which chunks each player has loaded).
- **Test:** Move player across chunk boundaries, verify seamless load/unload, verify remote players in different chunks still function.
- **Design doc ref:** Section 22.3

## E22-T06: Session save/load system
- **Size:** L (3-4h)
- **Depends on:** E22-T02
- **Description:** Implement persistent session saves: serialize game state (wave number, Core HP, built structures, player inventories, tower placements, explored POIs, boss progress) to JSON. Support save/resume mid-session. Map seed stored for procedural element consistency. Host controls saves; clients receive state on join.
- **Test:** Play 3 waves, save, reload, verify all state restored correctly (structures, inventories, wave number).
- **Design doc ref:** Section 22.3

## E22-T07: Map seed system for procedural elements
- **Size:** M (2h)
- **Depends on:** E22-T06
- **Description:** Implement seeded random for all procedural placement: boss fragment locations, POI positions, resource node distribution, wave composition randomization. Seed stored in session save. `SeededRandom` wrapper ensures deterministic results across clients given same seed.
- **Test:** Generate procedural placements with seed X, restart with same seed, verify identical results.
- **Design doc ref:** Section 22.3

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E22-T01 | M | None |
| E22-T02 | XL | E22-T01 |
| E22-T03 | M | E22-T01 |
| E22-T04 | M | E22-T01 |
| E22-T05 | L | E22-T02 |
| E22-T06 | L | E22-T02 |
| E22-T07 | M | E22-T06 |

**Total: 7 tasks, ~18-23h**
