# Epic E16: Reward System
**Layer:** 7
**Design Doc Sections:** 16.1–16.3
**Depends On:** E13 (Waves — wave clear rewards), E15 (Bosses — boss rewards), E11 (Crafting — item types)

---

## E16-T01: Individual loot distribution system
- **Size:** M (2-3h)
- **Depends on:** E19-T10, E22-T02
- **Description:** Implement per-player loot: each player receives their own drops (no stealing). Loot rolls are individual — server generates separate loot table results per player. Dropped items visible only to the owning player (or shared view with "yours" indicator). Exploration POIs: first opener gets primary loot, others get secondary. Network-synced loot visibility.
- **Test:** Enemy dies — each nearby player gets individual drops. Player 1 sees different loot than Player 2. Chest opened — opener gets primary, others get secondary.
- **Design doc ref:** Section 16.1

## E16-T02: Reward type definitions
- **Size:** M (2h)
- **Depends on:** E19-T03
- **Description:** Define all reward categories as taggable SO types: **Resources** (raw materials, processed components), **Equipment** (weapons, armor, accessories — tiered), **Tower Cores** (required for tower crafting), **Recipes** (crafting blueprints), **Enhancement Materials** (failstacks, protection items, upgrade stones), **Boss Fragments** (summon items), **Combat Scrolls** (one-use powerful spells), **Cosmetics** (skins, emotes, building styles), **Currency** (Gold for trading, Void Crystals for endgame vendor). Each category integrated with inventory system.
- **Test:** Verify all reward types can be generated, stored in inventory, and used by their respective systems.
- **Design doc ref:** Section 16.2

## E16-T03: Enemy kill reward loop
- **Size:** M (2h)
- **Depends on:** E16-T01, E13-T04
- **Description:** Implement per-kill rewards: each enemy kill drops resources, XP, chance of Tower Cores. Drop table per enemy type (from `EnemyDefinition` SO loot table reference). Elite enemies = bonus loot. Instant gratification: drops appear as physics objects near corpse, auto-collect within radius or manual pickup. Kill streak bonuses (optional).
- **Test:** Kill Goblin — small resource drop + XP. Kill Elite — better loot. Drops appear near corpse, auto-collect when nearby.
- **Design doc ref:** Section 16.3

## E16-T04: Wave clear reward loop
- **Size:** M (2h)
- **Depends on:** E16-T01, E13-T01
- **Description:** Implement wave-clear rewards: shared resource reward for all players + individual random equipment/material drops. Reward quality scales with wave number. Reward summary UI after wave clear (shows what each player received). Wave clear rewards distributed during transition phase.
- **Test:** Clear Wave 1 — basic rewards. Clear Wave 10 — better rewards. All players see their individual rewards in summary UI.
- **Design doc ref:** Section 16.3

## E16-T05: Exploration reward loop
- **Size:** M (2h)
- **Depends on:** E16-T01, E08-T02
- **Description:** Implement exploration rewards: POI-specific loot tables. Ancient Chests = rare equipment, recipes, Tower Cores. Fragment Shrines = boss fragments. Scroll Pedestals = combat scrolls. Hidden Caves = Tier 3-4 resources. Corrupted Nodes = reduced wave difficulty + loot. Each POI type has configurable loot table SO.
- **Test:** Open Ancient Chest — get equipment/recipe. Visit Fragment Shrine — get boss fragment. Destroy Corrupted Node — wave difficulty reduced + loot.
- **Design doc ref:** Section 16.3

## E16-T06: Currency systems (Gold + Void Crystals)
- **Size:** M (2h)
- **Depends on:** E16-T02
- **Description:** Implement dual currency: **Gold** — earned from enemy kills, wave clears, quests. Used for NPC merchant purchases and trading. **Void Crystals** — endgame currency from bosses and Endless Mode. Used at endgame vendor for top-tier items. Currency UI display (persistent HUD element). Currency earned notifications.
- **Test:** Kill enemies — earn Gold. Kill boss — earn Void Crystals. Buy from merchant — Gold spent. Buy from endgame vendor — Void Crystals spent.
- **Design doc ref:** Section 16.2

## E16-T07: Daily/Weekly quest system
- **Size:** L (3-4h)
- **Depends on:** E16-T02, E19-T02
- **Description:** Implement repeating quests: **Daily Quests** (currency + enhancement materials) — "Kill 50 enemies", "Craft 5 items", "Survive 3 waves". **Weekly Challenges** (cosmetics + rare materials) — "Defeat Boss X", "Enhance to +15", "Clear Wave 15". Quest definitions as SOs. Quest tracker UI. Auto-generated from pool. Reset timers (daily/weekly). Publish quest completion events.
- **Test:** Login — daily quests available. Complete "Kill 50 enemies" — reward given. Next day — new quests. Weekly challenge tracks across sessions.
- **Design doc ref:** Section 16.3

## E16-T08: Season pass reward track
- **Size:** L (3-4h)
- **Depends on:** E16-T07
- **Description:** Implement battle pass: free track (resources, basic cosmetics) and premium track ($9.99 — exclusive cosmetics, building styles, emotes). 60-day seasons. XP earned from all activities fills season pass bar. Tier rewards claimed at milestones. No gameplay advantages in premium track. Season pass UI with tier preview, progress bar, reward claiming.
- **Test:** Earn season XP — progress bar fills. Reach tier — claim free reward. Premium tier shows lock/unlock correctly. Season resets after 60 days.
- **Design doc ref:** Section 16.3

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E16-T01 | M | E19-T10, E22-T02 |
| E16-T02 | M | E19-T03 |
| E16-T03 | M | E16-T01, E13-T04 |
| E16-T04 | M | E16-T01, E13-T01 |
| E16-T05 | M | E16-T01, E08-T02 |
| E16-T06 | M | E16-T02 |
| E16-T07 | L | E16-T02, E19-T02 |
| E16-T08 | L | E16-T07 |

**Total: 8 tasks, ~20-24h**
