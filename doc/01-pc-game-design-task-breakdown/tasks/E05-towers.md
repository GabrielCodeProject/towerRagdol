# Epic E05: Tower Defense System
**Layer:** 3
**Design Doc Sections:** 5.1–5.5
**Depends On:** E07 (Building — placement surfaces), E09 (Combat — damage system), E19 (Content Registry, Behavior Plugins)

---

## E05-T01: Tower base entity and placement
- **Size:** L (3-4h)
- **Depends on:** E07-T01, E19-T03
- **Description:** Create `Tower` MonoBehaviour: references `TowerDefinition` SO, has HP, range, fire rate, damage. Towers MUST be placed on player-built foundations/platforms (check `BuildingPiece.allowsTowerPlacement`). Placement radius enforced (minimum spacing). Tower facing/rotation matters for directional towers. Towers can be picked up and repositioned during Prepare Phase only.
- **Test:** Place tower on foundation — works. Place on ground — blocked. Place too close to another tower — blocked. Pick up during Prepare — works. Pick up during Defend — blocked.
- **Design doc ref:** Section 5.2

## E05-T02: Tower targeting system (pluggable)
- **Size:** M (2-3h)
- **Depends on:** E05-T01, E19-T05
- **Description:** Implement tower targeting using `ITargetingStrategy` interface. Default strategies: `NearestTarget`, `StrongestTarget`, `LowestHPTarget`, `FirstInPath`. Tower detects enemies in range via overlap sphere. Strategy selectable per tower (configurable in TowerDefinition SO or player toggle). Server-authoritative targeting calculation.
- **Test:** Place tower near enemies. Switch targeting to "nearest" — targets closest. Switch to "strongest" — targets highest HP. Verify server controls target selection.
- **Design doc ref:** Section 5.2, 19.2

## E05-T03: Tower attack and projectile system
- **Size:** L (3-4h)
- **Depends on:** E05-T02, E22-T03
- **Description:** Implement tower attacks: tower aims at target, fires projectile at fire rate interval. Projectile types from `TowerDefinition`: single-target (arrow), AOE splash (cannon), chain (lightning), pierce (ballista), DOT (fire), slow zone (frost). Projectiles use object pooling. On hit: apply damage + knockback force + special effects. Tower rotation animation toward target.
- **Test:** Arrow tower fires at enemy — single target hit. Cannon fires — splash damages multiple. Lightning chains to 3 enemies. Verify projectile pooling.
- **Design doc ref:** Section 5.1

## E05-T04: Tower type definitions (8 launch towers)
- **Size:** M (2-3h)
- **Depends on:** E05-T03, E19-T03
- **Description:** Create `TowerDefinition` SO for all 8 launch towers: **Tier 1:** Arrow Tower (physical/single, fast fire, low damage), Cannon Tower (physical/AOE, slow fire, splash, ragdolls enemies). **Tier 2:** Magic Bolt (magic/single, ground+air), Frost Tower (magic/AOE, slow, no damage). **Tier 3:** Fire Tower (magic/AOE, DOT), Lightning Tower (magic/chain, ground+air), Ballista (physical/pierce). **Tier 4:** Void Tower (void/AOE, massive damage, slow attack).
- **Test:** All 8 towers register in TowerRegistry. Each tower fires with correct behavior (splash, chain, pierce, DOT, slow).
- **Design doc ref:** Section 5.1

## E05-T05: Tower crafting integration
- **Size:** M (2h)
- **Depends on:** E05-T01
- **Description:** Implement tower crafting: towers crafted at Tower Workbench (matches crafting station tier). Each tower requires resources + Tower Core item (dropped by enemies/exploration). Tower recipes defined as `RecipeDefinition` SOs. Crafted tower appears in inventory as placeable item. Max tower count per player (scales with Tower Mastery stat + level).
- **Test:** Craft Arrow Tower at Workbench — requires wood + stone + Tower Core. Appears in inventory. Place in world. Hit max tower count — crafting blocked.
- **Design doc ref:** Section 5.3

## E05-T06: Tower upgrade system (in-place)
- **Size:** M (2-3h)
- **Depends on:** E05-T01
- **Description:** Implement tower upgrading: Level 1 → 2 → 3. Done in-place during Prepare Phase. Each level costs scaling resources. Upgrades increase damage, range, fire rate per `TowerDefinition` upgrade curves. Visual change per level (larger, more elaborate model variant). Interact with placed tower → upgrade UI showing cost and stat preview.
- **Test:** Place Level 1 tower, upgrade to Level 2 — stats increase, visual changes. Upgrade to Level 3 — further improvement. Upgrade during Defend — blocked.
- **Design doc ref:** Section 5.3

## E05-T07: Tower HP and destruction
- **Size:** M (2h)
- **Depends on:** E05-T01
- **Description:** Towers have HP (from TowerDefinition SO). Enemies can attack and destroy towers. Damage visual states on tower (cracks, sparks). Destroyed tower drops partial materials for rebuilding. Destruction triggers physics debris (tower crumbles). Tower destruction publishes event for scoring/UI.
- **Test:** Enemy attacks tower — HP decreases, visuals degrade. Tower destroyed — debris physics, partial materials drop, event fires.
- **Design doc ref:** Section 5.2

## E05-T08: Trap system
- **Size:** L (3-4h)
- **Depends on:** E05-T01, E02-T03
- **Description:** Implement trap system: **Spike Trap** (damage + ragdoll trip), **Tar Pit** (slow zone), **Explosive Mine** (one-shot AOE explosion + ragdoll launch), **Spring Trap** (launches enemies backward — catapult ragdoll). Traps placed on ground (don't require built floors). Traps have charges — consumed on trigger, must be refilled between waves with resources. Trap definitions as SOs.
- **Test:** Place spike trap — enemy walks over, takes damage, trips. Place mine — enemy triggers, massive AOE ragdoll explosion. Refill traps between waves.
- **Design doc ref:** Section 5.5

## E05-T09: Tower network synchronization
- **Size:** M (2-3h)
- **Depends on:** E05-T01, E22-T02
- **Description:** Network sync towers: server-authoritative placement, targeting, and attack. Clients see tower aiming, firing, projectiles. Tower HP synced. Upgrade state synced. Placement/pickup synced. Late-join players receive all tower states. Trap trigger events synced.
- **Test:** Player 1 places tower — Player 2 sees it. Tower fires — all clients see projectile. Tower destroyed — all clients see debris.
- **Design doc ref:** Section 22.1

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E05-T01 | L | E07-T01, E19-T03 |
| E05-T02 | M | E05-T01, E19-T05 |
| E05-T03 | L | E05-T02, E22-T03 |
| E05-T04 | M | E05-T03, E19-T03 |
| E05-T05 | M | E05-T01 |
| E05-T06 | M | E05-T01 |
| E05-T07 | M | E05-T01 |
| E05-T08 | L | E05-T01, E02-T03 |
| E05-T09 | M | E05-T01, E22-T02 |

**Total: 9 tasks, ~23-28h**
