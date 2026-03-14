# Epic E12: Enhancement System
**Layer:** 5
**Design Doc Sections:** 12.1–12.7
**Depends On:** E11 (Crafting — enhancement materials and stations)

---

## E12-T01: Enhancement level system (+0 to +25)
- **Size:** L (3-4h)
- **Depends on:** E11-T01
- **Description:** Implement enhancement: equipment AND towers can be enhanced from +0 to +25. Each level increases stats (damage, defense, speed, etc.) per curves defined in `EnhancementCurve` SO. Enhancement performed at crafting stations (gear at appropriate tier station, towers at Tower Workbench). Enhancement consumes materials (Enhancement Stones + resources). Server-authoritative results.
- **Test:** Enhance weapon from +0 to +1 — stats increase. Materials consumed. Verify stat curve matches SO definition. Server validates result.
- **Design doc ref:** Section 12.1

## E12-T02: Risk tier system (success rates and failure penalties)
- **Size:** L (3-4h)
- **Depends on:** E12-T01, E19-T06
- **Description:** Implement tiered risk: **+0 to +5:** 100% success. **+6 to +9:** 70-50%, stay same on fail. **+10 to +14:** 50-30%, -1 level on fail. **+15 to +19:** 30-15%, -1 to -3 levels on fail. **+20 to +25:** 15-5%, -3 levels OR item breaks on fail. All rates from BalanceProfile SO. Failure penalty calculated server-side. Animation/VFX for success and failure.
- **Test:** Enhance at +3 — always succeeds. Enhance at +12 — sometimes fails, item drops 1 level. Enhance at +22 — high failure, item can break.
- **Design doc ref:** Section 12.2

## E12-T03: Pity system (Failstack)
- **Size:** M (2-3h)
- **Depends on:** E12-T02
- **Description:** Implement failstack: each failure grants +1 Failstack. Each Failstack adds +0.5% to next success rate. Failstacks consumed on success. **Hard pity:** +10-14 guaranteed after 10 failures, +15-19 after 20, +20-25 after 40. Pity progress bar in UI. Failstack count persisted per player. Server tracks and validates.
- **Test:** Fail 5 times — Failstack = 5, next attempt +2.5% bonus. Succeed — Failstacks consumed. Hit hard pity threshold — guaranteed success.
- **Design doc ref:** Section 12.3

## E12-T04: Protection mechanics
- **Size:** M (2-3h)
- **Depends on:** E12-T02
- **Description:** Implement protection items: **Enhancement Shield** (crafted) — prevents level regression on failure, consumed. **Restoration Scroll** (crafted) — restores broken equipment to +10, consumed. **Lucky Charm** (rare drop) — +10% success rate for one attempt, consumed. **Premium Protection Stone** — prevents regression AND breakage, also obtainable via weekly quest and boss drop (not cash-shop exclusive). All items as `ItemDefinition` SOs.
- **Test:** Use Enhancement Shield, fail — level stays same, shield consumed. Use Lucky Charm — success rate shows +10%. Break item, use Restoration Scroll — item restored to +10.
- **Design doc ref:** Section 12.4

## E12-T05: Broken equipment system
- **Size:** M (2h)
- **Depends on:** E12-T02
- **Description:** Implement broken state: broken items retain appearance but lose ALL stats. Cannot be traded. Visual indicator: cracked/damaged appearance on character model. Restoration at Celestial Anvil using Restoration materials. Restored item returns to +10 (not original level). Broken status persisted in item data.
- **Test:** Item breaks at +22 — stats zeroed, cracked visual. Take to Celestial Anvil — restore to +10. Verify appearance and stats restored.
- **Design doc ref:** Section 12.6

## E12-T06: Tower enhancement
- **Size:** M (2h)
- **Depends on:** E12-T01, E05-T01
- **Description:** Tower-specific enhancement using Tower Stones (separate from gear enhancement materials). Towers enhanced +0 to +15 (lower cap than gear). On failure: towers lose levels but are NEVER destroyed (softer penalty than gear). Enhanced towers visually change (glow, larger, more elaborate). Strategic choice: enhance gear or invest in towers?
- **Test:** Enhance tower from +0 to +5 — visual changes, damage increases. Fail at +10 — tower drops level, not destroyed. Tower Stones consumed.
- **Design doc ref:** Section 12.7

## E12-T07: Enhancement UI
- **Size:** L (3-4h)
- **Depends on:** E12-T03, E12-T04
- **Description:** Full enhancement UI: item slot (drag item to enhance), clear success rate percentage, Failstack counter, pity progress bar, cost forecast ("Expected attempts to guarantee: X"), protection item slots (optional shield/charm), celebratory failure messaging ("Failstack +3! Getting closer!"), history log of recent attempts, cost forecast showing materials needed. Success/failure animations.
- **Test:** Open enhancement UI — all elements display correctly. Enhance — success/failure animation. Failstack updates. Pity bar progresses. History log records attempt.
- **Design doc ref:** Section 12.5

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E12-T01 | L | E11-T01 |
| E12-T02 | L | E12-T01, E19-T06 |
| E12-T03 | M | E12-T02 |
| E12-T04 | M | E12-T02 |
| E12-T05 | M | E12-T02 |
| E12-T06 | M | E12-T01, E05-T01 |
| E12-T07 | L | E12-T03, E12-T04 |

**Total: 7 tasks, ~20-24h**
