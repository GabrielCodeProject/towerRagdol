# Epic E04: Core Base
**Layer:** 1
**Design Doc Sections:** 4.1–4.2
**Depends On:** E19 (Content Registry, Event Bus)

---

## E04-T01: Core Base entity — HP and visuals
- **Size:** M (2-3h)
- **Depends on:** E19-T02
- **Description:** Create Core Base entity: glowing crystal/artifact at map center. Has `CoreHP` component with configurable HP pool. Cannot be moved. Emits light + particle effects (visual beacon). When damaged: visual cracks appear progressively, alarm sound triggers. When HP reaches 0: publish `OnCoreDestroyed` event. Network-synced HP (server-authoritative).
- **Test:** Spawn Core, deal damage — cracks appear progressively. HP reaches 0 — destruction event fires. Remote clients see same HP/visual state.
- **Design doc ref:** Section 4.1

## E04-T02: Core healing aura
- **Size:** S (1-2h)
- **Depends on:** E04-T01
- **Description:** Implement healing aura around Core: players within radius slowly regenerate HP. Aura radius and heal rate configurable via SO. Visual indicator (ground circle, particle field) showing aura range. Aura visible to all players.
- **Test:** Stand near Core — HP regenerates. Move away — regeneration stops. Verify visual matches actual radius.
- **Design doc ref:** Section 4.1

## E04-T03: Core HP UI element
- **Size:** S (1-2h)
- **Depends on:** E04-T01
- **Description:** Create persistent UI element showing Core Base HP bar, visible to all players at all times. Shows current/max HP, percentage. Color changes as HP drops (green → yellow → red). Pulse animation when taking damage. Mini-map marker for Core position.
- **Test:** Damage Core — HP bar updates in real-time. Color transitions at correct thresholds. All clients see same values.
- **Design doc ref:** Section 3.2 (UI reference), Section 4.1

## E04-T04: Core upgrades — HP increase
- **Size:** M (2h)
- **Depends on:** E04-T01, E19-T01
- **Description:** Implement Core HP upgrade: 4 tiers, each granting +25% HP. Upgrade requires rare materials (defined in recipe SO). Upgrade interaction at Core (hold interact + confirm). Each tier visually changes Core appearance (larger glow, more elaborate model). Upgrade state persisted in session save.
- **Test:** Upgrade Core from Tier 0 to Tier 1 — HP increases by 25%, visual changes. Materials consumed.
- **Design doc ref:** Section 4.2

## E04-T05: Core Shield upgrade
- **Size:** M (2h)
- **Depends on:** E04-T04
- **Description:** Implement Core Shield: temporary damage absorption barrier that recharges between waves. Shield has separate HP pool (absorbs damage before Core HP). Visual bubble/barrier effect when active. Depleted shield shows cracked barrier. Recharges to full during Prepare Phase transition. Tier-based shield capacity.
- **Test:** Activate shield, take damage — shield absorbs first. Shield depletes — Core HP takes damage. Enter Prepare Phase — shield recharges.
- **Design doc ref:** Section 4.2

## E04-T06: Core Healing Aura upgrade
- **Size:** S (1h)
- **Depends on:** E04-T02, E04-T04
- **Description:** Implement upgradeable healing aura: increased radius and healing rate per upgrade tier. Visual aura grows with upgrade level. Uses same upgrade interaction flow as other Core upgrades.
- **Test:** Upgrade aura — radius visibly increases, heal rate increases. Verify at each tier.
- **Design doc ref:** Section 4.2

## E04-T07: Core Alarm Range upgrade
- **Size:** S (1h)
- **Depends on:** E04-T01, E04-T04
- **Description:** Implement Core Alarm: when enemies enter inner perimeter, alert all players with directional UI indicator and alarm sound. Alarm range upgradeable (larger perimeter detection). Visual pulse on mini-map showing enemy direction. Priority alert for enemies directly attacking Core.
- **Test:** Enemy enters perimeter — alarm triggers for all players with direction indicator. Upgrade range — detection radius increases.
- **Design doc ref:** Section 4.2

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E04-T01 | M | E19-T02 |
| E04-T02 | S | E04-T01 |
| E04-T03 | S | E04-T01 |
| E04-T04 | M | E04-T01, E19-T01 |
| E04-T05 | M | E04-T04 |
| E04-T06 | S | E04-T02, E04-T04 |
| E04-T07 | S | E04-T01, E04-T04 |

**Total: 7 tasks, ~11-14h**
