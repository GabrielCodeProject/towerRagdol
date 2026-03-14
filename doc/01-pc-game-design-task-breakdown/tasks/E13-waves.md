# Epic E13: Enemy Wave System
**Layer:** 4
**Design Doc Sections:** 13.1–13.5
**Depends On:** E03 (Game Phases), E04 (Core Base), E19 (Content Registry, Event Bus)

---

## E13-T01: Wave manager and scheduling
- **Size:** L (3-4h)
- **Depends on:** E03-T01, E19-T02
- **Description:** Implement `WaveManager`: tracks current wave number, schedules waves on phase transition to Defend. Configurable total waves (15-25, from BalanceProfile). Wave number escalates difficulty. Boss wave replaces normal wave when boss is summoned. Publishes `OnWaveStart(waveNum, composition)`, `OnWaveEnd(waveNum, stats)`. Tracks active enemy count — wave complete when all enemies dead.
- **Test:** Start session — Wave 1 on first Defend Phase. Complete wave — counter increments. Boss summon replaces wave. Wave complete when enemy count = 0.
- **Design doc ref:** Section 13.1

## E13-T02: Enemy definition SOs (9 types)
- **Size:** M (2-3h)
- **Depends on:** E19-T03, E19-T05
- **Description:** Create `EnemyDefinition` SO for all 9 launch enemy types: **Goblin** (cost:1, rush Core, low HP, ragdolls easily), **Archer** (cost:2, ranged, targets towers), **Brute** (cost:4, slow, high HP, destroys walls), **Shaman** (cost:3, buffs/heals allies), **Bomber** (cost:5, runs at Core, explodes on arrival), **Flyer** (cost:3, flies over walls), **Tunneler** (cost:4, bypasses walls), **Elite** (cost:8, random special abilities, bonus loot), **Wave Boss** (cost:15, every 5th wave, major threat).
- **Test:** All 9 enemy SOs register in EnemyRegistry. Each has correct cost, tags, behavior reference, stats.
- **Design doc ref:** Section 13.2

## E13-T03: Point-cost wave composition system
- **Size:** L (3-4h)
- **Depends on:** E13-T01, E13-T02
- **Description:** Implement wave composition via point budget. Base budget: 10 points (Wave 1), +5 per wave, +10 per additional player. Budget spent on enemies (Goblin=1, Elite=8, etc.). Composition randomized within budget using map seed. Special themed waves: "Siege" (all Brutes/Bombers), "Swarm" (all Goblins), "Aerial Assault" (all Flyers). Theme probability increases at higher waves. Wave preview UI during Prepare Phase.
- **Test:** Wave 1: ~10 points of enemies. Wave 5: ~35 points. With 2 players: +10 extra. Themed wave triggers occasionally. Preview shows enemy types.
- **Design doc ref:** Section 13.2, 13.3

## E13-T04: Enemy AI — base behavior system
- **Size:** XL (4-6h)
- **Depends on:** E13-T02, E19-T05
- **Description:** Implement enemy AI using `IEnemyBehavior` interface. Base behaviors: **Rush** (beeline to Core via NavMesh), **Ranged** (maintain distance, shoot towers/players), **Siege** (target walls/buildings), **Support** (stay near allies, buff/heal), **Suicide** (rush Core, explode on arrival), **Flying** (ignore ground NavMesh, fly directly), **Tunneling** (bypass walls). Each behavior pluggable via SO reference. Enemies attack obstacles in path.
- **Test:** Goblin rushes Core. Archer stays at range. Brute targets walls. Shaman heals nearby. Bomber explodes at Core. Flyer ignores walls.
- **Design doc ref:** Section 13.2

## E13-T05: Enemy spawning system
- **Size:** M (2-3h)
- **Depends on:** E13-T03, E22-T03
- **Description:** Implement wave spawning: spawn points on map edges (multiple directions). Spawn direction rotates/randomizes per wave. Enemies spawn in timed batches (not all at once). Object pooling for all enemies. Spawn cap: max 40 active enemies (PerformanceBudget gate). Queue excess spawns. Enemies use animation-driven movement, switch to ragdoll on hit/death.
- **Test:** Wave starts — enemies spawn from map edges in batches. Direction rotates between waves. Cap at 40 active. Killed enemies free pool slots for queued spawns.
- **Design doc ref:** Section 13.4

## E13-T06: Enemy pathfinding to Core
- **Size:** L (3-4h)
- **Depends on:** E13-T04, E07-T08
- **Description:** Implement NavMesh-based pathfinding: enemies path toward Core Base (primary target). Building pieces act as NavMesh obstacles. If path blocked by walls, enemies attack nearest wall. Path recalculation when buildings placed/destroyed. Secondary targets: players and towers (based on AI behavior). NavMesh baking at runtime when buildings change.
- **Test:** Enemy navigates around walls to Core. Wall placed in path — enemy recalculates. All paths blocked — enemy attacks wall. Path opens — enemy resumes navigation.
- **Design doc ref:** Section 13.4

## E13-T07: Wave scaling configuration
- **Size:** M (2h)
- **Depends on:** E13-T03, E19-T06
- **Description:** Wave difficulty scaling via BalanceProfile SO: base difficulty, per-wave increment, per-player scaling. Difficulty escalation notification between waves ("Wave 5 incoming — Brutes will appear!"). Wave Boss appears every 5th wave automatically (configurable). All scaling values tunable without code changes.
- **Test:** Adjust BalanceProfile values — wave difficulty changes accordingly. 5th wave includes Wave Boss. Notification shows upcoming enemy types.
- **Design doc ref:** Section 13.3

## E13-T08: Corrupted node interaction (wave difficulty reduction)
- **Size:** M (2-3h)
- **Depends on:** E13-T05
- **Description:** Implement corrupted nodes as bonus enemy spawn points during waves. If players destroy a node during Prepare Phase, that spawn direction is removed/weakened for next wave (fewer enemies from that side). Creates exploration incentive: destroy nodes = easier defense. Nodes regenerate after X waves. Node destruction publishes event for reward system.
- **Test:** Destroy corrupted node — next wave has fewer enemies from that direction. Node regenerates after configured waves. Event fires for rewards.
- **Design doc ref:** Section 13.5

## E13-T09: Enemy network synchronization
- **Size:** L (3-4h)
- **Depends on:** E13-T04, E22-T02
- **Description:** Network sync enemies: server spawns and controls all enemies. Server runs AI/pathfinding. Client receives position updates (interpolated). Death/ragdoll events synced. Enemy HP synced (for UI health bars). Spawn events synced. Object pool managed by server. Late-join receives active enemy states.
- **Test:** All clients see enemies in same positions. Enemy killed by Player 1 — all clients see death ragdoll. Late-join player sees all active enemies.
- **Design doc ref:** Section 22.1

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E13-T01 | L | E03-T01, E19-T02 |
| E13-T02 | M | E19-T03, E19-T05 |
| E13-T03 | L | E13-T01, E13-T02 |
| E13-T04 | XL | E13-T02, E19-T05 |
| E13-T05 | M | E13-T03, E22-T03 |
| E13-T06 | L | E13-T04, E07-T08 |
| E13-T07 | M | E13-T03, E19-T06 |
| E13-T08 | M | E13-T05 |
| E13-T09 | L | E13-T04, E22-T02 |

**Total: 9 tasks, ~28-36h**
