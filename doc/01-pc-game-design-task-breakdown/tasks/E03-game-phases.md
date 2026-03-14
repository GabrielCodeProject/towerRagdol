# Epic E03: Game Phases
**Layer:** 1
**Design Doc Sections:** 3.1–3.4
**Depends On:** E19 (Event Bus, Balance Framework)

---

## E03-T01: Phase state machine
- **Size:** L (3-4h)
- **Depends on:** E19-T02
- **Description:** Implement `PhaseManager` state machine with states: `Prepare`, `Defend`, `Transition`, `BossWave`, `GameOver`, `Victory`. Transitions driven by conditions (timer expiry, wave clear, Core death, boss defeat). Publish phase change events via Event Bus (`OnPhaseChanged` with old/new phase). Server-authoritative phase control synced to clients.
- **Test:** Start in Prepare, trigger wave start → transitions to Defend. Clear wave → transitions to Transition → Prepare. Core dies → GameOver.
- **Design doc ref:** Section 3.1–3.4

## E03-T02: Prepare Phase mechanics
- **Size:** M (2-3h)
- **Depends on:** E03-T01
- **Description:** Implement Prepare Phase rules: configurable timer (default 3-5 min), countdown visible in UI (warnings at 30s, 10s). During Prepare: building allowed, crafting allowed, gathering allowed, exploration allowed, repair allowed, enhancement allowed. Timer driven by `BalanceProfile` SO. Visual/audio warnings at thresholds.
- **Test:** Enter Prepare Phase, timer counts down, warnings fire at 30s and 10s. Timer expires → transitions to Defend.
- **Design doc ref:** Section 3.1

## E03-T03: Ready-up voting system
- **Size:** M (2h)
- **Depends on:** E03-T02, E22-T02
- **Description:** Implement "ready up" system during Prepare Phase. Players press ready button, UI shows who is ready. When all players ready OR majority vote, skip remaining prep time. Network-synced: server tracks ready states, broadcasts to all clients. Ready state resets each phase.
- **Test:** 2 of 2 players ready up → phase skips to Defend immediately. 1 of 2 ready → timer continues.
- **Design doc ref:** Section 3.1

## E03-T04: Defend Phase mechanics
- **Size:** M (2-3h)
- **Depends on:** E03-T01
- **Description:** Implement Defend Phase rules: wave announcement with enemy composition preview (UI popup showing enemy types/counts). Building/crafting disabled (or limited to quick-repairs). Players fight freely. Wave complete when all enemies killed (track active enemy count). Integrate with Event Bus: `OnWaveStart`, `OnWaveEnd`, `OnEnemyKilled`.
- **Test:** Enter Defend Phase, wave announcement displays. Building blocked. Kill all enemies → wave complete triggers.
- **Design doc ref:** Section 3.2

## E03-T05: Player down and revive system
- **Size:** L (3-4h)
- **Depends on:** E03-T04, E02-T04
- **Description:** Implement downed state: when player HP reaches 0, enter ragdoll crawl state (reduced movement, can crawl toward teammates). Teammates revive by standing near downed player and holding interact (progress bar). If all players downed simultaneously, towers fight alone until respawn timer expires or Core dies. Ragdoll crawl animation drives physics.
- **Test:** Player takes lethal damage → enters crawl state. Teammate stands near → revive bar fills → player recovers. All players down → respawn timer starts.
- **Design doc ref:** Section 3.2

## E03-T06: Between-wave transition
- **Size:** M (2h)
- **Depends on:** E03-T04
- **Description:** Implement wave-survived transition: trigger ragdoll victory poses (random from pool), collect loot drops from wave, display damage report UI (structures damaged, enemies killed, player MVP stats). Auto-transition to Prepare Phase after delay. Show difficulty escalation notification for next wave.
- **Test:** Survive wave → victory poses play, damage report shows, transitions to Prepare. Next wave difficulty notification appears.
- **Design doc ref:** Section 3.3

## E03-T07: Game Over — defeat condition
- **Size:** M (2h)
- **Depends on:** E03-T01
- **Description:** Implement defeat: when Core HP reaches 0, trigger Core explosion VFX/SFX, transition to GameOver state, display game over screen with session stats (waves survived, enemies killed, bosses defeated, time played). Disable player input. Option to return to menu or start new session.
- **Test:** Reduce Core HP to 0 → explosion plays, game over screen appears with correct stats.
- **Design doc ref:** Section 3.4

## E03-T08: Game Over — victory condition
- **Size:** M (2h)
- **Depends on:** E03-T01
- **Description:** Implement victory: when final boss (Chaos Entity) is defeated, trigger victory celebration (ragdoll party poses, fireworks VFX), display victory screen with full session stats and final rewards. Unlock Endless Mode flag. Option to continue to Endless Mode or return to menu.
- **Test:** Defeat final boss → victory celebration plays, Endless Mode unlocked, stats display correctly.
- **Design doc ref:** Section 3.4

## E03-T09: Phase-aware system gating
- **Size:** M (2h)
- **Depends on:** E03-T01
- **Description:** Implement `PhaseGate` utility that other systems query to check what's allowed in current phase. Building system checks `PhaseGate.CanBuild()`, crafting checks `PhaseGate.CanCraft()`, etc. Configurable per phase via SO. Provides clean API for all systems to respect phase rules without coupling to PhaseManager directly.
- **Test:** During Prepare: CanBuild()=true, CanCraft()=true. During Defend: CanBuild()=false (or limited), CanCraft()=false.
- **Design doc ref:** Section 3.1, 3.2

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E03-T01 | L | E19-T02 |
| E03-T02 | M | E03-T01 |
| E03-T03 | M | E03-T02, E22-T02 |
| E03-T04 | M | E03-T01 |
| E03-T05 | L | E03-T04, E02-T04 |
| E03-T06 | M | E03-T04 |
| E03-T07 | M | E03-T01 |
| E03-T08 | M | E03-T01 |
| E03-T09 | M | E03-T01 |

**Total: 9 tasks, ~20-25h**
