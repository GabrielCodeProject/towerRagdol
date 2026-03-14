# Epic E07: Building System
**Layer:** 2
**Design Doc Sections:** 7.1–7.3
**Depends On:** E06 (Resources/Inventory), E19 (Content Registry, Tags)

---

## E07-T01: Grid-based placement system
- **Size:** L (3-4h)
- **Depends on:** E19-T03
- **Description:** Implement grid-based building: snap system for piece placement, 90-degree and 45-degree rotation. Ghost preview of piece before placement (green=valid, red=invalid). Placement validation: must have resources, must be on valid ground or connected to existing piece, no overlap. Server-authoritative placement (client sends request, server validates and spawns).
- **Test:** Enter build mode, ghost preview follows cursor, snaps to grid. Place piece — resources consumed, piece spawns. Invalid position — red ghost, placement blocked.
- **Design doc ref:** Section 7.1

## E07-T02: Building piece definitions
- **Size:** M (2-3h)
- **Depends on:** E07-T01, E19-T03
- **Description:** Create `BuildingPieceDefinition` SO instances for all launch pieces. **Foundations:** Floor tiles, platforms, ramps (marked as tower-placement-valid). **Walls:** Full walls, half walls, windows, doorframes, crenellations. **Roofs:** Flat, angled, peaked. **Functional:** Crafting station slots, storage, furnaces. **Defensive:** Reinforced walls, gates, barricades. **Utility:** Bridges, stairs, elevated platforms. Each SO: model ref, HP, material tier, snap rules, cost.
- **Test:** All building piece SOs register in BuildingRegistry. Each piece placeable with correct snap behavior.
- **Design doc ref:** Section 7.2

## E07-T03: Structural integrity system
- **Size:** L (3-4h)
- **Depends on:** E07-T01
- **Description:** Implement structural integrity: pieces need support for tall structures. Foundation pieces = max integrity. Each level up from foundation loses integrity. When support is removed (piece destroyed), check if above pieces still have valid support path — if not, they collapse (ragdoll physics collapse!). Material affects max integrity height (wood=3, stone=5, iron=7, crystal=10).
- **Test:** Build tower 5 high with wood — top pieces show low integrity warning. Destroy middle piece — above pieces collapse with physics.
- **Design doc ref:** Section 7.1

## E07-T04: Material-based durability
- **Size:** M (2h)
- **Depends on:** E07-T02
- **Description:** Implement HP per building piece based on material tier: Wood (100 HP) < Stone (250 HP) < Iron (500 HP) < Crystal (1000 HP). Pieces take damage from enemy attacks. Damage visual states (cracked, damaged, critical). Pieces can be repaired during Prepare Phase (costs partial materials). Destroyed pieces drop partial materials for salvage.
- **Test:** Enemy attacks wood wall — HP decreases, visual damage appears. Wall destroyed — partial materials drop. Repair restores HP and visuals.
- **Design doc ref:** Section 7.1

## E07-T05: Physics-based destruction
- **Size:** L (3-4h)
- **Depends on:** E07-T04, E02-T03
- **Description:** Implement physics destruction: when a building piece's HP reaches 0, it fractures into physics debris (pre-fractured mesh or runtime fracture). Debris ragdolls outward from impact direction. Brute enemies smash through walls (dramatic destruction). Chain reaction: structural integrity collapse triggers cascade of physics debris. Debris despawns after timeout.
- **Test:** Brute hits wall — wall explodes into physics fragments flying in hit direction. Cascade collapse creates dramatic debris shower.
- **Design doc ref:** Section 7.1

## E07-T06: Ragdoll building interactions
- **Size:** S (1-2h)
- **Depends on:** E07-T01, E02-T02
- **Description:** Add ragdoll physics to building: character wobbles while placing heavy pieces (force applied based on piece weight). Placing large pieces = visible effort (character staggers). Pieces placed on character can knock them over (comedy). Light pieces = smooth placement.
- **Test:** Place heavy iron wall — character wobbles. Place light wood floor — smooth placement. Drop piece on player — ragdoll reaction.
- **Design doc ref:** Section 7.1

## E07-T07: Phase-gated building
- **Size:** S (1h)
- **Depends on:** E07-T01, E03-T09
- **Description:** Integrate with PhaseGate: full building allowed during Prepare Phase. During Defend Phase: no new building OR limited quick-repairs only (configurable via BalanceProfile). Build mode UI disabled/hidden during Defend. Quick-repair: target damaged piece, hold interact, restore HP at cost.
- **Test:** Prepare Phase: build freely. Defend Phase: build mode blocked. Quick-repair works on damaged pieces during Defend.
- **Design doc ref:** Section 7.1

## E07-T08: Strategic building — enemy pathfinding interaction
- **Size:** L (3-4h)
- **Depends on:** E07-T01
- **Description:** Implement building-aware pathfinding: walls act as NavMesh obstacles. Enemies path around intact walls to reach Core. If no valid path exists, enemies attack nearest wall to create path. Walls funnel enemies into chokepoints (player-created kill zones). Elevated platforms affect tower range calculations. Building layout directly determines defense effectiveness.
- **Test:** Build wall maze — enemies navigate around walls toward Core. Block all paths — enemies attack nearest wall. Create chokepoint — enemies funnel through.
- **Design doc ref:** Section 7.3

## E07-T09: Building network synchronization
- **Size:** M (2-3h)
- **Depends on:** E07-T01, E22-T02
- **Description:** Network sync for building: server validates and spawns all placed pieces. Building state (position, rotation, HP, material) synced to all clients. Late-joining players receive full building state snapshot. Destruction events synced (all clients see same debris physics seed). Repair state synced.
- **Test:** Player 1 builds wall — Player 2 sees it. Player 1 repairs — Player 2 sees HP restore. New player joins — sees all existing buildings.
- **Design doc ref:** Section 7.1, 22.1

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E07-T01 | L | E19-T03 |
| E07-T02 | M | E07-T01, E19-T03 |
| E07-T03 | L | E07-T01 |
| E07-T04 | M | E07-T02 |
| E07-T05 | L | E07-T04, E02-T03 |
| E07-T06 | S | E07-T01, E02-T02 |
| E07-T07 | S | E07-T01, E03-T09 |
| E07-T08 | L | E07-T01 |
| E07-T09 | M | E07-T01, E22-T02 |

**Total: 9 tasks, ~22-28h**
