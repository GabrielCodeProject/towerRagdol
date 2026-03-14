# Epic E06: Gathering System
**Layer:** 2
**Design Doc Sections:** 6.1–6.3
**Depends On:** E02 (Ragdoll interactions), E19 (Content Registry, Tags)

---

## E06-T01: Resource type definitions
- **Size:** M (2h)
- **Depends on:** E19-T03, E19-T04
- **Description:** Create `ResourceDefinition` SO template: ID, display name, icon, description, tier (1-4), tags, stack size, gather tool requirement (tool tier needed), gather time, gather VFX/SFX. Create all launch resources: **Tier 1:** Wood, Stone, Fiber, Clay. **Tier 2:** Iron Ore, Copper, Thick Leather, Hardwood. **Tier 3:** Crystal, Darksteel, Enchanted Fiber, Dragon Bone. **Tier 4:** Void Essence, Celestial Ore, Primordial Shard.
- **Test:** All 15 resource SOs load into ContentRegistry, queryable by tier and tags.
- **Design doc ref:** Section 6.1

## E06-T02: Resource node world objects
- **Size:** M (2-3h)
- **Depends on:** E06-T01
- **Description:** Create `ResourceNode` MonoBehaviour: references a `ResourceDefinition`, has HP (gather progress), yields resources when depleted. Visual states: full → partially gathered → depleted → respawning. Nodes placed in world via MapDefinition rules or manual placement. Network-synced state (server tracks HP, all clients see same node state).
- **Test:** Approach node, gather — HP decreases, resources awarded on deplete. Remote clients see node state changes.
- **Design doc ref:** Section 6.2

## E06-T03: Tool-gated gathering mechanics
- **Size:** M (2-3h)
- **Depends on:** E06-T02
- **Description:** Implement tool tier gating: player must have equipped tool matching or exceeding resource tier. Stone pickaxe (tier 1) can't mine Iron (tier 2). Gathering interaction: hold interact button, progress bar fills based on tool tier + Strength stat. Wrong tool shows "requires Tier X tool" feedback. Tools defined as `ItemDefinition` SOs with `toolTier` field.
- **Test:** Equip tier 1 pickaxe, try tier 2 node — blocked with message. Equip tier 2 — gathering works. Higher tier tool = faster gathering.
- **Design doc ref:** Section 6.2

## E06-T04: Ragdoll gathering interactions
- **Size:** M (2h)
- **Depends on:** E06-T03, E02-T02
- **Description:** Add ragdoll physics to gathering: character stumbles/wobbles when mining (apply small forces to arms during swing animation). Trees fall with ragdoll physics when chopped (tree becomes ragdoll on deplete, falls in random direction). Mining rocks crack and fragments scatter via physics. Comedy factor: character can be knocked over by falling tree.
- **Test:** Mine a rock — character wobbles. Chop a tree — tree falls with physics, can hit player causing ragdoll.
- **Design doc ref:** Section 6.2

## E06-T05: Co-op gathering bonus
- **Size:** M (2h)
- **Depends on:** E06-T03, E22-T02
- **Description:** Implement multi-player gathering: when 2+ players gather from same node simultaneously, gather speed increases + bonus resources on deplete. Server tracks how many players are gathering each node. Bonus scales: 2 players = 1.5x speed + 20% bonus, 3 players = 2x speed + 40% bonus, 4 players = 2.5x speed + 60% bonus. Visual indicator when co-op bonus active.
- **Test:** Two players gather same node — speed visibly faster, bonus resources in loot. Single player gathers — normal speed.
- **Design doc ref:** Section 6.2

## E06-T06: Resource node respawn system
- **Size:** M (2h)
- **Depends on:** E06-T02
- **Description:** Implement respawn: depleted nodes enter cooldown timer, respawn between waves (partial respawn — not all nodes). Full respawn triggered on boss kill. Respawn timer configurable per resource tier (higher tier = longer respawn). Visual: node fades in when respawning. Server controls respawn timing, synced to clients.
- **Test:** Deplete node, survive wave — node partially respawns. Kill boss — all nodes fully respawn.
- **Design doc ref:** Section 6.2

## E06-T07: Biome-locked resource distribution
- **Size:** M (2-3h)
- **Depends on:** E06-T01, E19-T08
- **Description:** Implement zone-based resource placement rules: Starting Zone (near Core) = Tier 1 only. Mid-range = Tier 1-2. Outer Zone (near enemy spawners) = Tier 2-3. Boss Zones (unlocked after boss kill) = Tier 3-4. Hidden Areas (behind puzzles/destructible terrain) = special resources. Rules defined in `MapDefinition` SO zone configs. Procedural placement within zones using map seed.
- **Test:** Inspect resource distribution — Tier 1 only near Core, Tier 3-4 only in boss/outer zones. Regenerate with same seed — identical placement.
- **Design doc ref:** Section 6.3

## E06-T08: Player inventory system
- **Size:** L (3-4h)
- **Depends on:** E06-T01, E19-T03
- **Description:** Implement player inventory: grid-based slots, items stack by type (stack size from SO), weight system tied to Strength stat (carry capacity). Inventory UI with drag/drop, split stacks, sort by type/tier. Quick-access hotbar (8 slots). Network-synced (server validates, client displays). Drop items into world as physics pickups.
- **Test:** Gather resources — appear in inventory. Stack items. Exceed capacity — blocked. Drop item — spawns in world as pickup.
- **Design doc ref:** Section 6 (implied), Section 10.1 (carry capacity)

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E06-T01 | M | E19-T03, E19-T04 |
| E06-T02 | M | E06-T01 |
| E06-T03 | M | E06-T02 |
| E06-T04 | M | E06-T03, E02-T02 |
| E06-T05 | M | E06-T03, E22-T02 |
| E06-T06 | M | E06-T02 |
| E06-T07 | M | E06-T01, E19-T08 |
| E06-T08 | L | E06-T01, E19-T03 |

**Total: 8 tasks, ~18-22h**
