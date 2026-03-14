# Epic E15: Boss Fights
**Layer:** 6
**Design Doc Sections:** 15.1–15.5
**Depends On:** E08 (Map — altars and fragments), E09 (Combat — damage system), E13 (Waves — wave replacement)

---

## E15-T01: Boss fight framework
- **Size:** L (3-4h)
- **Depends on:** E13-T01, E09-T06, E19-T05
- **Description:** Implement `BossManager` that handles boss encounter lifecycle: altar activation → phase transition → boss spawn → phase-based combat → defeat → rewards. Boss defined by `BossDefinition` SO: phases (2-3 per boss), attacks, fragment count required, rewards, HP scaling curve, minion wave config. Bosses use `IBossPhase` interface for phase transitions. During boss fight, Core is STILL vulnerable (boss sends minion waves).
- **Test:** Activate altar → boss spawns. Boss transitions through phases. Minion waves attack Core during fight. Boss defeated → rewards.
- **Design doc ref:** Section 15.1, 15.3

## E15-T02: Boss definitions (5 bosses)
- **Size:** L (3-4h)
- **Depends on:** E15-T01
- **Description:** Create `BossDefinition` SOs for all 5 bosses: **Mushroom Golem** (3 fragments, tutorial boss, unlocks Tier 2 + new zone), **Ancient Treant** (5 fragments, unlocks Tier 3 + advanced towers), **Swamp Hydra** (7 fragments, unlocks Tier 4 + spell upgrades), **Fire Dragon** (9 fragments, unlocks endgame zone + Void towers), **Chaos Entity** (12 fragments, final boss, unlocks Endless Mode). Each with unique phases, attacks, and arena hazards.
- **Test:** All 5 boss SOs register in BossRegistry. Each has correct fragment count, unlock rewards, phase definitions.
- **Design doc ref:** Section 15.2

## E15-T03: Boss phase system
- **Size:** XL (4-6h)
- **Depends on:** E15-T01
- **Description:** Implement phase-based boss combat: each boss has 2-3 phases triggered by HP thresholds. Phase transition: dramatic animation/VFX, brief invulnerability, new attack patterns. `IBossPhase` implementations: `Enter()` (setup new attacks), `Update()` (run phase logic), `Exit()` (cleanup), `IsComplete()` (HP threshold check). Phase-specific environmental hazards (arena-specific: fire zones, poison pools, falling rocks).
- **Test:** Fight boss — at 66% HP, phase 2 triggers (new attacks). At 33%, phase 3 triggers. Each phase has distinct behavior and arena hazard.
- **Design doc ref:** Section 15.3

## E15-T04: Boss ragdoll knockback and weak points
- **Size:** M (2-3h)
- **Depends on:** E15-T01, E02-T03
- **Description:** Boss attacks cause massive ragdoll knockback to players (comedy + danger). Bosses have weak points that require player positioning (e.g., back of head, exposed core during attack animation). Hitting weak point = bonus damage + special ragdoll reaction from boss. Weak point highlighted with VFX during vulnerability window.
- **Test:** Boss attack knocks player flying (ragdoll). Hit weak point during window — bonus damage, boss staggers. Miss weak point — normal damage.
- **Design doc ref:** Section 15.3

## E15-T05: Boss minion waves (Core threat)
- **Size:** M (2-3h)
- **Depends on:** E15-T01, E13-T05
- **Description:** During boss fights, boss periodically spawns minion waves that target the Core. Forces split attention: some players fight boss, others defend Core. Minion waves use same spawn system as regular waves but with boss-specific composition. Minion count and frequency from `BossDefinition` SO. Creates strategic co-op decisions.
- **Test:** During boss fight — minion wave spawns, targets Core. Core takes damage if undefended. Minion waves increase as boss phases progress.
- **Design doc ref:** Section 15.3

## E15-T06: Boss structure destruction
- **Size:** S (1-2h)
- **Depends on:** E15-T01, E07-T05
- **Description:** Bosses can destroy player structures in the boss arena area. Boss attacks that hit buildings deal massive damage. Creates arena cleanup — players can't just build walls around boss. Structure destruction uses same physics system as regular building destruction but with amplified forces.
- **Test:** Build walls near boss — boss attack destroys walls with dramatic physics debris. Verify only arena-area structures affected.
- **Design doc ref:** Section 15.3

## E15-T07: Co-op boss scaling
- **Size:** M (2h)
- **Depends on:** E15-T01
- **Description:** Implement boss scaling per player count: **HP:** +30% per additional player. **Minions:** +1 additional minion wave per extra player. **Damage:** +4% per additional player. Minimum difficulty floor (can't be trivialized even solo). Scaling values from `BossDefinition` SO. UI shows boss HP scaled for current player count.
- **Test:** Solo: base HP. 2 players: +30% HP. 4 players: +90% HP. More minion waves with more players. Verify minimum floor active.
- **Design doc ref:** Section 15.4

## E15-T08: Boss rewards and progression unlocks
- **Size:** M (2-3h)
- **Depends on:** E15-T01, E19-T02
- **Description:** Implement boss rewards: guaranteed progression unlock on first kill (new tier, zone, recipes, tower types). Unique boss material drop (needed for top-tier crafting). Enhancement materials. First-kill bonus: unique cosmetic. Rewards per player (individual loot). Re-farmable: fragments respawn after boss defeat for re-farming materials. Publish `OnBossDefeated` event for progression system.
- **Test:** Kill boss first time — unlock notification (Tier 2 crafting + new zone). Unique cosmetic awarded. Fragments respawn. Kill again — materials drop, no re-unlock.
- **Design doc ref:** Section 15.5

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E15-T01 | L | E13-T01, E09-T06, E19-T05 |
| E15-T02 | L | E15-T01 |
| E15-T03 | XL | E15-T01 |
| E15-T04 | M | E15-T01, E02-T03 |
| E15-T05 | M | E15-T01, E13-T05 |
| E15-T06 | S | E15-T01, E07-T05 |
| E15-T07 | M | E15-T01 |
| E15-T08 | M | E15-T01, E19-T02 |

**Total: 8 tasks, ~22-29h**
