# Epic E08: Map & Exploration
**Layer:** 6
**Design Doc Sections:** 8.1–8.4
**Depends On:** E06 (Gathering — resource nodes), E07 (Building — structures), E14 (Spawners — overworld enemies)

---

## E08-T01: Map structure — concentric zones
- **Size:** L (3-4h)
- **Depends on:** E19-T08, E22-T05
- **Description:** Implement map layout: Core Base at center, concentric zones radiating outward (safe → moderate → dangerous). Zone boundaries defined in `MapDefinition` SO. Zone affects: resource tier availability, enemy difficulty, POI rarity. Map size: medium (5-10 min walk from Core to edge). Enemy spawn points placed on map edges/corners. Zone visual differentiation (terrain, lighting, fog).
- **Test:** Spawn at Core — safe zone. Walk outward — zone transitions visible (terrain/lighting changes). Verify resource tiers match zone. Edge of map has spawn points.
- **Design doc ref:** Section 8.1

## E08-T02: Points of Interest (POI) system
- **Size:** L (3-4h)
- **Depends on:** E08-T01, E19-T01
- **Description:** Implement POI system with types: **Boss Altars** (boss summon locations), **Ancient Chests** (rare equipment, recipes, Tower Cores), **Boss Fragment Shrines** (fragments for boss summoning), **Combat Scroll Pedestals** (one-time powerful spells), **Hidden Caves** (Tier 3-4 resources, mini-boss guardians), **Corrupted Nodes** (enemy spawner sources), **NPC Camps** (merchants, quest givers between waves). POIs defined as SOs, placed procedurally from `MapDefinition` rules using map seed.
- **Test:** Load map — POIs placed procedurally. Same seed = same placement. Interact with chest — loot drops. Fragment shrine — fragment collected.
- **Design doc ref:** Section 8.2

## E08-T03: Exploration risk/reward balance
- **Size:** M (2h)
- **Depends on:** E08-T01, E14-T01
- **Description:** Implement distance-based risk: further from Core = better loot tables but more/harder overworld enemies. POIs in outer zones guarded by elite enemies (mini-boss encounters). Time spent exploring = less time building. Create tension via phase timer — must return before wave. Visual danger indicators (zone warning UI, color-coded minimap).
- **Test:** Explore outer zone — encounter harder enemies, find better loot. Inner zone — easier, worse loot. Timer pressure creates meaningful choice.
- **Design doc ref:** Section 8.3

## E08-T04: Boss fragment collection system
- **Size:** M (2-3h)
- **Depends on:** E08-T02
- **Description:** Implement boss fragment system: each boss requires X fragments (3, 5, 7, 9, 12 for bosses 1-5). Fragments found at Boss Fragment Shrines (POIs). Fragment locations change each playthrough (procedural via map seed). Collected fragments tracked in player inventory (special quest item category). Mini-map markers for discovered fragment locations. Fragments REQUIRE exploration — can't ignore.
- **Test:** Explore map — find fragment shrine, collect fragment. Inventory shows 1/3 for Boss 1. Different seed = different shrine locations.
- **Design doc ref:** Section 8.4

## E08-T05: Boss altar activation
- **Size:** M (2h)
- **Depends on:** E08-T04
- **Description:** Implement Boss Altar: specific map location where players use collected fragments. Interact with altar when enough fragments collected → triggers boss fight (replaces next wave). Altar interaction requires all co-op players to confirm (vote). Altar location marked on map after discovery. Visual indicator showing fragment progress at altar.
- **Test:** Collect 3 fragments, go to altar, activate — boss fight starts next Defend Phase. Insufficient fragments — activation blocked.
- **Design doc ref:** Section 8.4

## E08-T06: Mini-map and exploration UI
- **Size:** M (2-3h)
- **Depends on:** E08-T01
- **Description:** Implement mini-map: shows player position, Core Base marker, discovered POI markers (icon per type), zone boundaries, teammate positions, enemy threat indicators. Fog of war: unexplored areas hidden until visited. Full map overlay (toggle key) with zoom/pan. Fragment shrine markers. Corrupted node markers. Zone danger level indicators.
- **Test:** Open map — Core visible, fog of war on unvisited areas. Explore — fog clears. POIs appear as icons. Teammates visible. Zone colors indicate danger.
- **Design doc ref:** Section 8.1, 8.4

## E08-T07: NPC camps (merchants and quests)
- **Size:** M (2-3h)
- **Depends on:** E08-T02
- **Description:** Implement NPC camps at POI locations: wandering merchants sell items for gold (inventory refreshes between waves), quest givers offer optional objectives (fetch quests, kill targets) for rewards. NPCs only available during Prepare Phase. NPC definitions as SOs: merchant inventory, quest definitions, dialogue. Camp locations from MapDefinition.
- **Test:** Visit NPC camp during Prepare — merchant offers items. Accept quest — objective tracked. Complete quest — reward given. During Defend — NPCs unavailable.
- **Design doc ref:** Section 8.2

## E08-T08: Hidden areas and destructible terrain
- **Size:** M (2-3h)
- **Depends on:** E08-T01, E07-T05
- **Description:** Implement hidden areas: secret caves/rooms behind destructible terrain (breakable walls, collapsed tunnels). Destructible using combat abilities or tools. Hidden areas contain Tier 3-4 special resources and unique loot. Discovery marked on map. Some hidden areas require puzzle interaction (pressure plates, switches). Mini-boss guards inside.
- **Test:** Find destructible wall — break with hammer. Hidden cave revealed. Enter — find rare resources. Map updates with discovery marker.
- **Design doc ref:** Section 6.3, 8.2

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E08-T01 | L | E19-T08, E22-T05 |
| E08-T02 | L | E08-T01, E19-T01 |
| E08-T03 | M | E08-T01, E14-T01 |
| E08-T04 | M | E08-T02 |
| E08-T05 | M | E08-T04 |
| E08-T06 | M | E08-T01 |
| E08-T07 | M | E08-T02 |
| E08-T08 | M | E08-T01, E07-T05 |

**Total: 8 tasks, ~22-27h**
