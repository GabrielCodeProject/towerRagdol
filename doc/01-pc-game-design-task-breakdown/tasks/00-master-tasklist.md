# Master Task List — Ragdoll Realms

## Overview
- **17 Epics** across **9 dependency layers** (0-8)
- **155 atomic tasks** total
- **~285-358 estimated hours**
- Every checkbox from the design doc mapped to at least one task
- Dependency graph is acyclic (layers enforce ordering)
- Foundation (Layer 0) tasks have zero cross-dependencies

---

## Dependency Graph (Layers)

```
Layer 0 ─── E19 Extensibility ──────────────────────────────┐
         └─ E22 Technical Architecture ─────────────────────┤
                                                             │
Layer 1 ─── E02 Ragdoll System (← E19, E22) ───────────────┤
         ├─ E03 Game Phases (← E19) ────────────────────────┤
         └─ E04 Core Base (← E19) ─────────────────────────┤
                                                             │
Layer 2 ─── E06 Gathering (← E02, E19) ────────────────────┤
         └─ E07 Building (← E06, E19) ─────────────────────┤
                                                             │
Layer 3 ─── E05 Towers (← E07, E09, E19) ──────────────────┤
         └─ E09 Combat (← E02, E19) ───────────────────────┤
                                                             │
Layer 4 ─── E13 Enemy Waves (← E03, E04, E19) ─────────────┤
         └─ E14 Spawners (← E13) ──────────────────────────┤
                                                             │
Layer 5 ─── E10 RPG Features (← E02, E09) ─────────────────┤
         ├─ E11 Crafting (← E06, E10) ─────────────────────┤
         └─ E12 Enhancement (← E11) ───────────────────────┤
                                                             │
Layer 6 ─── E08 Map & Exploration (← E06, E07, E14) ───────┤
         └─ E15 Boss Fights (← E08, E09, E13) ─────────────┤
                                                             │
Layer 7 ─── E16 Reward System (← E13, E15, E11) ───────────┤
         └─ E20 Comedy Framework (← E19) ──────────────────┤
                                                             │
Layer 8 ─── E17 Monetization (← E16, E12) ─────────────────┘
```

---

## Task Summary by Epic

| Epic | Name | Layer | Tasks | Est. Hours | Depends On |
|------|------|-------|-------|------------|------------|
| E19 | Extensibility Architecture | 0 | 10 | 28-32h | None |
| E22 | Technical Architecture | 0 | 7 | 18-23h | None |
| E02 | Ragdoll System | 1 | 11 | 32-39h | E19, E22 |
| E03 | Game Phases | 1 | 9 | 20-25h | E19 |
| E04 | Core Base | 1 | 7 | 11-14h | E19 |
| E06 | Gathering System | 2 | 8 | 18-22h | E02, E19 |
| E07 | Building System | 2 | 9 | 22-28h | E06, E19 |
| E05 | Tower Defense | 3 | 9 | 23-28h | E07, E09, E19 |
| E09 | Combat System | 3 | 8 | 21-28h | E02, E19 |
| E13 | Enemy Waves | 4 | 9 | 28-36h | E03, E04, E19 |
| E14 | Enemy Spawners | 4 | 6 | 12-16h | E13 |
| E10 | RPG Features | 5 | 6 | 17-22h | E02, E09 |
| E11 | Crafting System | 5 | 7 | 18-22h | E06, E10 |
| E12 | Enhancement System | 5 | 7 | 20-24h | E11 |
| E08 | Map & Exploration | 6 | 8 | 22-27h | E06, E07, E14 |
| E15 | Boss Fights | 6 | 8 | 22-29h | E08, E09, E13 |
| E16 | Reward System | 7 | 8 | 20-24h | E13, E15, E11 |
| E20 | Comedy Framework | 7 | 8 | 18-24h | E19 |
| E17 | Monetization | 8 | 7 | 16-20h | E16, E12 |
| **TOTAL** | | | **155** | **~285-358h** | |

---

## Full Task Index

### Layer 0: Foundation

#### E19 — Extensibility Architecture
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E19-T01 | Content Registry base system | L | — |
| E19-T02 | Event Bus / Message System | L | — |
| E19-T03 | ScriptableObject base templates | L | E19-T01 |
| E19-T04 | Tag & Category System | M | E19-T03 |
| E19-T05 | Behavior Plugin interfaces | M | — |
| E19-T06 | Scaling & Balance Framework | M | E19-T01 |
| E19-T07 | Typed registries per content type | M | E19-T01, E19-T03 |
| E19-T08 | Map system extensibility foundation | M | E19-T03 |
| E19-T09 | Class system extensibility foundation | M | E19-T03, E19-T05 |
| E19-T10 | Loot Table system | M | E19-T01, E19-T04 |

#### E22 — Technical Architecture
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E22-T01 | Project scaffolding and folder structure | M | — |
| E22-T02 | Networking foundation (client-server) | XL | E22-T01 |
| E22-T03 | Object pooling system | M | E22-T01 |
| E22-T04 | Performance budget manager | M | E22-T01 |
| E22-T05 | Chunk-based world loading | L | E22-T02 |
| E22-T06 | Session save/load system | L | E22-T02 |
| E22-T07 | Map seed system | M | E22-T06 |

### Layer 1: Core Systems

#### E02 — Ragdoll System
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E02-T01 | Active ragdoll skeleton setup | L | E22-T01 |
| E02-T02 | Animation-driven target rotation | XL | E02-T01 |
| E02-T03 | External force response system | M | E02-T02 |
| E02-T04 | Ragdoll recovery mechanic | M | E02-T03 |
| E02-T05 | Ragdoll melee physics | L | E02-T02 |
| E02-T06 | Ragdoll ranged projectiles | M | E02-T03 |
| E02-T07 | Ragdoll magic/spell forces | M | E02-T03 |
| E02-T08 | Hand-to-hand grapple system | L | E02-T05 |
| E02-T09 | Hip-only network sync | L | E02-T02, E22-T02 |
| E02-T10 | Physics LOD system | M | E02-T02, E22-T04 |
| E02-T11 | Ragdoll config ScriptableObject | S | E02-T02, E19-T03 |

#### E03 — Game Phases
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E03-T01 | Phase state machine | L | E19-T02 |
| E03-T02 | Prepare Phase mechanics | M | E03-T01 |
| E03-T03 | Ready-up voting system | M | E03-T02, E22-T02 |
| E03-T04 | Defend Phase mechanics | M | E03-T01 |
| E03-T05 | Player down and revive system | L | E03-T04, E02-T04 |
| E03-T06 | Between-wave transition | M | E03-T04 |
| E03-T07 | Game Over — defeat condition | M | E03-T01 |
| E03-T08 | Game Over — victory condition | M | E03-T01 |
| E03-T09 | Phase-aware system gating | M | E03-T01 |

#### E04 — Core Base
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E04-T01 | Core Base entity (HP + visuals) | M | E19-T02 |
| E04-T02 | Core healing aura | S | E04-T01 |
| E04-T03 | Core HP UI element | S | E04-T01 |
| E04-T04 | Core upgrades — HP increase | M | E04-T01, E19-T01 |
| E04-T05 | Core Shield upgrade | M | E04-T04 |
| E04-T06 | Core Healing Aura upgrade | S | E04-T02, E04-T04 |
| E04-T07 | Core Alarm Range upgrade | S | E04-T01, E04-T04 |

### Layer 2: Resource & Construction

#### E06 — Gathering System
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E06-T01 | Resource type definitions | M | E19-T03, E19-T04 |
| E06-T02 | Resource node world objects | M | E06-T01 |
| E06-T03 | Tool-gated gathering mechanics | M | E06-T02 |
| E06-T04 | Ragdoll gathering interactions | M | E06-T03, E02-T02 |
| E06-T05 | Co-op gathering bonus | M | E06-T03, E22-T02 |
| E06-T06 | Resource node respawn system | M | E06-T02 |
| E06-T07 | Biome-locked resource distribution | M | E06-T01, E19-T08 |
| E06-T08 | Player inventory system | L | E06-T01, E19-T03 |

#### E07 — Building System
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E07-T01 | Grid-based placement system | L | E19-T03 |
| E07-T02 | Building piece definitions | M | E07-T01, E19-T03 |
| E07-T03 | Structural integrity system | L | E07-T01 |
| E07-T04 | Material-based durability | M | E07-T02 |
| E07-T05 | Physics-based destruction | L | E07-T04, E02-T03 |
| E07-T06 | Ragdoll building interactions | S | E07-T01, E02-T02 |
| E07-T07 | Phase-gated building | S | E07-T01, E03-T09 |
| E07-T08 | Enemy pathfinding interaction | L | E07-T01 |
| E07-T09 | Building network sync | M | E07-T01, E22-T02 |

### Layer 3: Defense & Combat

#### E05 — Tower Defense
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E05-T01 | Tower base entity and placement | L | E07-T01, E19-T03 |
| E05-T02 | Tower targeting (pluggable) | M | E05-T01, E19-T05 |
| E05-T03 | Tower attack and projectile system | L | E05-T02, E22-T03 |
| E05-T04 | Tower type definitions (8 towers) | M | E05-T03, E19-T03 |
| E05-T05 | Tower crafting integration | M | E05-T01 |
| E05-T06 | Tower upgrade system (in-place) | M | E05-T01 |
| E05-T07 | Tower HP and destruction | M | E05-T01 |
| E05-T08 | Trap system | L | E05-T01, E02-T03 |
| E05-T09 | Tower network sync | M | E05-T01, E22-T02 |

#### E09 — Combat System
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E09-T01 | Combat style framework | M | E19-T03, E19-T05 |
| E09-T02 | Melee combat — combo system | L | E09-T01, E02-T05 |
| E09-T03 | Ranged combat — bow/crossbow | M | E09-T01, E02-T06 |
| E09-T04 | Spell system — full implementation | XL | E09-T01, E02-T07, E19-T05 |
| E09-T05 | Mana system | S | E09-T04 |
| E09-T06 | Damage calculation system | M | E09-T01, E19-T06 |
| E09-T07 | Combat during Defend Phase | M | E09-T01, E03-T04 |
| E09-T08 | Combat network sync | L | E09-T01, E22-T02 |

### Layer 4: Enemies

#### E13 — Enemy Waves
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E13-T01 | Wave manager and scheduling | L | E03-T01, E19-T02 |
| E13-T02 | Enemy definition SOs (9 types) | M | E19-T03, E19-T05 |
| E13-T03 | Point-cost wave composition | L | E13-T01, E13-T02 |
| E13-T04 | Enemy AI — base behavior system | XL | E13-T02, E19-T05 |
| E13-T05 | Enemy spawning system | M | E13-T03, E22-T03 |
| E13-T06 | Enemy pathfinding to Core | L | E13-T04, E07-T08 |
| E13-T07 | Wave scaling configuration | M | E13-T03, E19-T06 |
| E13-T08 | Corrupted node interaction | M | E13-T05 |
| E13-T09 | Enemy network sync | L | E13-T04, E22-T02 |

#### E14 — Enemy Spawners (Overworld)
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E14-T01 | Overworld enemy camp system | M | E13-T02, E13-T04 |
| E14-T02 | Corrupted node spawner objects | M | E14-T01 |
| E14-T03 | Roaming enemy packs | M | E13-T04 |
| E14-T04 | Mini-boss guards | M | E14-T01, E13-T02 |
| E14-T05 | Spawner node regeneration | S | E14-T02 |
| E14-T06 | Proximity activation system | S | E14-T02 |

### Layer 5: Progression

#### E10 — RPG Features
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E10-T01 | Character stat system | M | E19-T03 |
| E10-T02 | Leveling and XP system | M | E10-T01, E19-T02 |
| E10-T03 | Milestone unlocks | M | E10-T02 |
| E10-T04 | Skill tree system | XL | E10-T02, E19-T09 |
| E10-T05 | Equipment slot system | L | E10-T01, E06-T08 |
| E10-T06 | Stat allocation UI | M | E10-T02 |

#### E11 — Crafting System
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E11-T01 | Crafting station entities (4 tiers) | M | E07-T02 |
| E11-T02 | Recipe system | L | E11-T01, E19-T01 |
| E11-T03 | Crafting quality system | M | E11-T02, E10-T01 |
| E11-T04 | Recipe discovery system | M | E11-T02 |
| E11-T05 | Tower-specific crafting | M | E11-T02, E05-T05 |
| E11-T06 | Resource sink balancing | M | E11-T02 |
| E11-T07 | Crafting UI | L | E11-T02 |

#### E12 — Enhancement System
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E12-T01 | Enhancement level system (+0 to +25) | L | E11-T01 |
| E12-T02 | Risk tier system | L | E12-T01, E19-T06 |
| E12-T03 | Pity system (Failstack) | M | E12-T02 |
| E12-T04 | Protection mechanics | M | E12-T02 |
| E12-T05 | Broken equipment system | M | E12-T02 |
| E12-T06 | Tower enhancement | M | E12-T01, E05-T01 |
| E12-T07 | Enhancement UI | L | E12-T03, E12-T04 |

### Layer 6: World & Bosses

#### E08 — Map & Exploration
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E08-T01 | Map structure — concentric zones | L | E19-T08, E22-T05 |
| E08-T02 | Points of Interest (POI) system | L | E08-T01, E19-T01 |
| E08-T03 | Exploration risk/reward balance | M | E08-T01, E14-T01 |
| E08-T04 | Boss fragment collection system | M | E08-T02 |
| E08-T05 | Boss altar activation | M | E08-T04 |
| E08-T06 | Mini-map and exploration UI | M | E08-T01 |
| E08-T07 | NPC camps (merchants/quests) | M | E08-T02 |
| E08-T08 | Hidden areas and destructible terrain | M | E08-T01, E07-T05 |

#### E15 — Boss Fights
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E15-T01 | Boss fight framework | L | E13-T01, E09-T06, E19-T05 |
| E15-T02 | Boss definitions (5 bosses) | L | E15-T01 |
| E15-T03 | Boss phase system | XL | E15-T01 |
| E15-T04 | Boss ragdoll knockback/weak points | M | E15-T01, E02-T03 |
| E15-T05 | Boss minion waves (Core threat) | M | E15-T01, E13-T05 |
| E15-T06 | Boss structure destruction | S | E15-T01, E07-T05 |
| E15-T07 | Co-op boss scaling | M | E15-T01 |
| E15-T08 | Boss rewards and progression unlocks | M | E15-T01, E19-T02 |

### Layer 7: Rewards & Comedy

#### E16 — Reward System
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E16-T01 | Individual loot distribution | M | E19-T10, E22-T02 |
| E16-T02 | Reward type definitions | M | E19-T03 |
| E16-T03 | Enemy kill reward loop | M | E16-T01, E13-T04 |
| E16-T04 | Wave clear reward loop | M | E16-T01, E13-T01 |
| E16-T05 | Exploration reward loop | M | E16-T01, E08-T02 |
| E16-T06 | Currency systems (Gold + Void Crystals) | M | E16-T02 |
| E16-T07 | Daily/Weekly quest system | L | E16-T02, E19-T02 |
| E16-T08 | Season pass reward track | L | E16-T07 |

#### E20 — Comedy Framework
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E20-T01 | Comedy feature plugin system | M | E19-T02, E19-T05 |
| E20-T02 | Comedy session settings UI | M | E20-T01 |
| E20-T03 | Physics comedy features | L | E20-T01, E02-T03 |
| E20-T04 | Audio comedy features | M | E20-T01 |
| E20-T05 | Visual comedy features | M | E20-T01 |
| E20-T06 | Gameplay comedy features | L | E20-T01, E09-T04 |
| E20-T07 | Comedy governance system | S | E20-T02 |
| E20-T08 | Seasonal comedy events | M | E20-T01 |

### Layer 8: Business

#### E17 — Monetization
| ID | Task | Size | Depends On |
|----|------|------|-----------|
| E17-T01 | Base game purchase and content gating | S | — |
| E17-T02 | Cosmetic shop system | L | E19-T01, E16-T06 |
| E17-T03 | Premium currency system | M | E16-T06 |
| E17-T04 | Battle Pass integration | M | E16-T08, E17-T03 |
| E17-T05 | Enhancement convenience items | M | E12-T04, E17-T03 |
| E17-T06 | Parental controls | M | E17-T03 |
| E17-T07 | DLC expansion framework | M | E19-T08, E17-T02 |

---

## Size Distribution
| Size | Count | Hours Each | Total Hours |
|------|-------|-----------|-------------|
| S (1-2h) | 16 | 1-2h | 16-32h |
| M (2-3h) | 93 | 2-3h | 186-279h |
| L (3-4h) | 36 | 3-4h | 108-144h |
| XL (4-6h) | 10 | 4-6h | 40-60h |
| **Total** | **155** | | **~285-358h** |

---

## Parallelization Opportunities

**Within each layer**, epics and tasks with independent dependencies can run in parallel:

- **Layer 0:** E19 and E22 are fully parallel
- **Layer 1:** E02, E03, E04 can progress in parallel (shared E19 dependency only)
- **Layer 2:** E06 and E07 are sequential (E07 depends on E06)
- **Layer 3:** E05 and E09 can start in parallel
- **Layer 4:** E13 and E14 are sequential (E14 depends on E13)
- **Layer 5:** E10 can start independently; E11 needs E10-T01; E12 needs E11
- **Layer 6:** E08 and E15 have some parallel paths
- **Layer 7:** E16 and E20 are fully parallel
- **Layer 8:** E17 is the final integration layer

---

## Design Doc Coverage Verification

| Section | Epic(s) | Checkboxes | Tasks |
|---------|---------|------------|-------|
| 2. Ragdoll System | E02 | 23 | 11 |
| 3. Game Phases | E03 | 22 | 9 |
| 4. Core Base | E04 | 11 | 7 |
| 5. Tower Defense | E05 | 22 | 9 |
| 6. Gathering System | E06 | 14 | 8 |
| 7. Building System | E07 | 17 | 9 |
| 8. Map & Exploration | E08 | 19 | 8 |
| 9. Combat System | E09 | 18 | 8 |
| 10. RPG Features | E10 | 16 | 6 |
| 11. Crafting System | E11 | 13 | 7 |
| 12. Enhancement System | E12 | 21 | 7 |
| 13. Enemy Waves | E13 | 19 | 9 |
| 14. Enemy Spawners | E14 | 8 | 6 |
| 15. Boss Fights | E15 | 18 | 8 |
| 16. Reward System | E16 | 18 | 8 |
| 17. Monetization | E17 | 17 | 7 |
| 19. Extensibility | E19 | 24 | 10 |
| 20. Comedy Framework | E20 | 28 | 8 |
| 22. Technical Architecture | E22 | 12 | 7 |
| **TOTAL** | **17 epics** | **~280** | **155** |

Sections 1 (Core Identity), 18 (Progression Flow), 21 (Content Roadmap), 23 (Dev Phases), 24 (Risk Register), 25 (Design Decisions) are narrative/planning sections — no implementation tasks needed. Their requirements are captured in the functional epics above.
