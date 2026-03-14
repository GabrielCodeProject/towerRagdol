# Epic E14: Enemy Spawner System (Overworld)
**Layer:** 4
**Design Doc Sections:** 14.1–14.2
**Depends On:** E13 (Enemy Wave System — enemy definitions and AI)

---

## E14-T01: Overworld enemy camp system
- **Size:** M (2-3h)
- **Depends on:** E13-T02, E13-T04
- **Description:** Implement enemy camps: static overworld encounters guarding POIs and resources. Camps contain preset enemy groups (defined in SO). Hostile on player proximity (aggro radius). Camp enemies use same `IEnemyBehavior` AI as wave enemies but with "guard" behavior (patrol area, return if player retreats). Camps reward loot on clear.
- **Test:** Approach camp — enemies aggro. Kill all camp enemies — camp cleared, loot drops. Retreat beyond aggro range — enemies return to camp.
- **Design doc ref:** Section 14.1

## E14-T02: Corrupted node spawner objects
- **Size:** M (2-3h)
- **Depends on:** E14-T01
- **Description:** Implement corrupted nodes as destructible spawner objects in overworld. Nodes have HP, spawn enemies on proximity (timer + max active per node). Difficulty scales with distance from Core. Destroying node: area cleared, reduces next wave spawn points (integrates with E13-T08). Nodes visually corrupted (dark particles, glowing cracks).
- **Test:** Approach node — enemies spawn periodically. Destroy node — spawning stops, area cleared. Further nodes spawn harder enemies.
- **Design doc ref:** Section 14.1, 14.2

## E14-T03: Roaming enemy packs
- **Size:** M (2h)
- **Depends on:** E13-T04
- **Description:** Implement roaming packs: groups of 2-5 enemies that patrol between nodes. Follow waypoint paths. Can stumble into player base area (creates surprise encounters during Prepare Phase). Roaming packs use "patrol" AI behavior. Pack composition from EnemyRegistry by zone tier.
- **Test:** Roaming pack follows patrol path. Pack enters base area — attacks structures. Kill pack — loot drops.
- **Design doc ref:** Section 14.1

## E14-T04: Mini-boss guards
- **Size:** M (2-3h)
- **Depends on:** E14-T01, E13-T02
- **Description:** Implement mini-boss guards: elite enemies protecting boss fragment shrines and hidden caves. Mini-boss uses "Elite" enemy definition with enhanced stats and special abilities. Each mini-boss guard defined per POI in MapDefinition SO. Defeating mini-boss = access to guarded content. Mini-boss has visible health bar and unique name.
- **Test:** Approach guarded shrine — mini-boss aggros. Defeat mini-boss — shrine accessible. Mini-boss has health bar and special attacks.
- **Design doc ref:** Section 14.1

## E14-T05: Spawner node regeneration
- **Size:** S (1-2h)
- **Depends on:** E14-T02
- **Description:** Implement node regeneration: destroyed corrupted nodes regenerate after X waves (configurable per node tier). Regeneration visible (node rebuilds over several waves). Players can't permanently clear all nodes — must re-clear periodically. Regeneration timer tracked by server, synced to clients.
- **Test:** Destroy node, survive X waves — node regenerates. Re-destroy — timer resets. Verify regeneration timeline.
- **Design doc ref:** Section 14.2

## E14-T06: Proximity activation system
- **Size:** S (1-2h)
- **Depends on:** E14-T02
- **Description:** Implement proximity-based activation for spawner nodes: nodes only spawn enemies when players are nearby (configurable radius). Inactive nodes consume no performance. Activation state synced across network. Visual indicator when node is active (glowing, particles intensify).
- **Test:** Node inactive when no players near — no enemies spawn. Player approaches — node activates, enemies begin spawning. Player leaves — node deactivates.
- **Design doc ref:** Section 14.2

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E14-T01 | M | E13-T02, E13-T04 |
| E14-T02 | M | E14-T01 |
| E14-T03 | M | E13-T04 |
| E14-T04 | M | E14-T01, E13-T02 |
| E14-T05 | S | E14-T02 |
| E14-T06 | S | E14-T02 |

**Total: 6 tasks, ~12-16h**
