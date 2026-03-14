# Epic E10: RPG Features
**Layer:** 5
**Design Doc Sections:** 10.1–10.4
**Depends On:** E02 (Ragdoll — character model), E09 (Combat — stat usage)

---

## E10-T01: Character stat system
- **Size:** M (2-3h)
- **Depends on:** E19-T03
- **Description:** Implement 6 core stats: **Strength** (melee damage, carry capacity, mining speed), **Agility** (move speed, dodge recovery, attack speed), **Vitality** (HP, stamina, ragdoll recovery speed), **Intelligence** (spell damage, mana pool, crafting quality bonus), **Tower Mastery** (tower damage bonus, tower range bonus, max tower count), **Luck** (drop rates, crit chance, enhancement luck). Stats stored on `PlayerStats` component, synced over network.
- **Test:** Set Strength to 20 — melee damage increases, carry capacity increases. Set Intelligence to 20 — mana pool increases. Verify each stat affects correct systems.
- **Design doc ref:** Section 10.1

## E10-T02: Leveling and XP system
- **Size:** M (2-3h)
- **Depends on:** E10-T01, E19-T02
- **Description:** Implement XP and leveling: XP earned from combat (defend phase primary), gathering, crafting, boss kills. Level cap: 50 (soft cap), 100 (hard cap via endgame). XP curve from BalanceProfile SO. Stat points awarded per level (player-allocated via UI). Level-up notification + VFX. XP values configurable per activity. Publish `OnPlayerLevelUp` event.
- **Test:** Kill enemies — XP gained. Level up — stat points available. Allocate to Strength — melee damage increases. Reach level 50 — XP rate reduces.
- **Design doc ref:** Section 10.2

## E10-T03: Milestone unlocks
- **Size:** M (2h)
- **Depends on:** E10-T02
- **Description:** Implement milestone system: every 10 levels (10, 20, 30, 40, 50) unlock new abilities, recipes, or tower types. Milestones defined in `MilestoneDefinition` SOs: level required, unlock type (recipe, ability, tower slot), reference to unlocked content. Unlock notification UI. Milestones tracked per player.
- **Test:** Reach level 10 — milestone unlock notification, new ability/recipe available. Reach level 20 — different unlock. Verify all 5 milestones.
- **Design doc ref:** Section 10.2

## E10-T04: Skill tree system
- **Size:** XL (4-6h)
- **Depends on:** E10-T02, E19-T09
- **Description:** Implement 4 skill trees: **Warrior Path** (melee specialization — grapples, power attacks, taunt), **Ranger Path** (ranged — multi-shot, piercing arrows, traps), **Mage Path** (spell — bigger AOE, lower cooldowns, new spells), **Architect Path** (tower/building — tower buffs, faster building, extra tower slots). Each tree = `SkillTreeDefinition` SO with node connections. Skill points from leveling. Players can hybrid between paths. Each skill = `ISkillEffect` implementation + `SkillDefinition` SO.
- **Test:** Open skill tree UI, allocate points. Warrior skill "Power Attack" — melee damage boost active. Invest in both Warrior and Mage — hybrid works.
- **Design doc ref:** Section 10.3

## E10-T05: Equipment slot system
- **Size:** L (3-4h)
- **Depends on:** E10-T01, E06-T08
- **Description:** Implement 9 equipment slots: Head, Chest, Legs, Boots, Gloves, Main Hand, Off Hand, 2 Accessory slots (rings/amulets). Each equipped item modifies player stats from `ItemDefinition` SO. Equipment visually affects ragdoll character model (mesh swap/attachment). Equip/unequip via inventory UI. Stat recalculation on equip change. Network-synced visuals.
- **Test:** Equip helmet — visual changes on character, defense stat increases. Remove — reverts. All 9 slots functional. Other players see equipped gear.
- **Design doc ref:** Section 10.4

## E10-T06: Stat allocation UI
- **Size:** M (2h)
- **Depends on:** E10-T02
- **Description:** Create stat allocation UI panel: shows current stat values, available points, +/- buttons per stat. Preview showing what each point would change (e.g., "+1 Strength = +5 melee damage, +10 carry capacity"). Confirm/reset buttons. Accessible from character menu. Stat effects described with tooltips.
- **Test:** Level up, open stat UI — points available. Allocate point — preview shows effect. Confirm — stats update. Reset before confirm — points restored.
- **Design doc ref:** Section 10.1, 10.2

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E10-T01 | M | E19-T03 |
| E10-T02 | M | E10-T01, E19-T02 |
| E10-T03 | M | E10-T02 |
| E10-T04 | XL | E10-T02, E19-T09 |
| E10-T05 | L | E10-T01, E06-T08 |
| E10-T06 | M | E10-T02 |

**Total: 6 tasks, ~17-22h**
