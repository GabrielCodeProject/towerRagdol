# Epic E09: Combat System
**Layer:** 3
**Design Doc Sections:** 9.1–9.3
**Depends On:** E02 (Ragdoll combat), E19 (Content Registry, Behavior Plugins)

---

## E09-T01: Combat style framework
- **Size:** M (2-3h)
- **Depends on:** E19-T03, E19-T05
- **Description:** Implement combat style system: players switch styles by equipping different weapons (not class-locked). Four styles: Melee Fighter, Archer/Ranger, Mage/Caster, Brawler. Each weapon `ItemDefinition` SO references a `CombatStyle` enum and associated attack animations, combos, and special moves. `CombatController` reads equipped weapon and activates corresponding attack logic.
- **Test:** Equip sword — melee attacks available. Switch to bow — ranged attacks. Switch to staff — spell attacks. Verify smooth transition.
- **Design doc ref:** Section 9.1

## E09-T02: Melee combat — combo system
- **Size:** L (3-4h)
- **Depends on:** E09-T01, E02-T05
- **Description:** Implement melee combo system: swords, hammers, axes. Light attack → heavy attack chains. Combo timing windows. Each weapon type has different swing speed, range, and force curves. Ragdoll grapple attacks for melee (grab + slam). Damage = impact velocity × weapon stat × Strength modifier. Hit detection via raycast along weapon length.
- **Test:** Swing sword — 3-hit combo chain. Hammer — slower but more knockback. Grapple attack throws enemy. Damage scales with stats.
- **Design doc ref:** Section 9.1

## E09-T03: Ranged combat — bow/crossbow system
- **Size:** M (2-3h)
- **Depends on:** E09-T01, E02-T06
- **Description:** Implement ranged combat: hold to draw bow (charge time affects damage/range), release to fire. Projectile arc affected by gravity. Crossbow: faster fire, less charge, flatter trajectory. Precision aiming reticle. Multi-shot (skill unlock). Projectiles apply knockback on hit. Ammo system (craft arrows) or infinite basic arrows (configurable).
- **Test:** Draw bow, fire — arrow arcs to target, deals damage + knockback. Crossbow fires faster, flatter. Charge time affects damage.
- **Design doc ref:** Section 9.1

## E09-T04: Spell system — full implementation
- **Size:** XL (4-6h)
- **Depends on:** E09-T01, E02-T07, E19-T05
- **Description:** Implement spell system using `ISpellEffect` interface. **Fireball:** AOE explosion, ragdoll launch, DOT. **Ice Lance:** single-target freeze, shatter on next hit. **Lightning Bolt:** chain to 3-5 enemies, stun. **Earth Wall:** summon temporary physics wall to redirect enemies. **Heal Wave:** AOE heal players + repair nearby structures. **Gravity Well:** pull enemies together (combo setup). Mana cost per spell, mana regenerates over time. Spell scrolls = one-time powerful versions.
- **Test:** Cast each spell — correct effect. Fireball ragdolls enemies. Ice freezes. Lightning chains. Earth Wall blocks pathfinding. Gravity Well pulls enemies. Mana depletes and regenerates.
- **Design doc ref:** Section 9.2

## E09-T05: Mana system
- **Size:** S (1-2h)
- **Depends on:** E09-T04
- **Description:** Implement mana pool: max mana scales with Intelligence stat. Mana regenerates over time (base rate + Intelligence bonus). Mana potions (craftable consumable) for instant restore. Mana bar UI element. Network-synced (server validates spell costs).
- **Test:** Cast spell — mana decreases. Wait — mana regenerates. Use potion — instant mana restore. Out of mana — spell blocked.
- **Design doc ref:** Section 9.2

## E09-T06: Damage calculation system
- **Size:** M (2-3h)
- **Depends on:** E09-T01, E19-T06
- **Description:** Implement unified damage formula: `FinalDamage = BaseDamage × StatMultiplier × ElementMultiplier × CritMultiplier − ArmorReduction`. Damage types: Physical, Magic, Fire, Ice, Lightning, Void. Armor reduces physical, magic resistance reduces magic. Critical hits based on Luck stat. Damage numbers float above targets (UI). All formula values from BalanceProfile SO.
- **Test:** Hit enemy with known stats — verify damage matches formula. Crit hits show increased damage. Armor reduces physical. Magic bypasses armor.
- **Design doc ref:** Section 2.2, 9.1, 19.5

## E09-T07: Combat during Defend Phase mechanics
- **Size:** M (2h)
- **Depends on:** E09-T01, E03-T04
- **Description:** Implement defend phase combat rules: players move freely across entire map. Enemies reaching Core trigger high-threat alert UI. Players near Core get defense bonus (proximity buff — increased damage/defense). Combat XP earned during defend phase (primary XP source). XP awarded per enemy kill, bonus for wave clear.
- **Test:** During Defend: fight anywhere on map. Enemies near Core — alert appears. Stand near Core — proximity buff active. Kill enemies — XP earned.
- **Design doc ref:** Section 9.3

## E09-T08: Combat network synchronization
- **Size:** L (3-4h)
- **Depends on:** E09-T01, E22-T02
- **Description:** Network sync combat: server-authoritative damage calculation. Attack inputs sent to server, server validates hit detection and applies damage. Spell effects synced (all clients see same fireball, ice, etc.). Damage numbers synced. Death/ragdoll events synced. Mana state synced. Combat state replicated with lag compensation.
- **Test:** Player 1 hits enemy — all clients see hit effect + damage number. Spell cast by Player 1 visible to Player 2. Server validates all damage.
- **Design doc ref:** Section 22.1

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E09-T01 | M | E19-T03, E19-T05 |
| E09-T02 | L | E09-T01, E02-T05 |
| E09-T03 | M | E09-T01, E02-T06 |
| E09-T04 | XL | E09-T01, E02-T07, E19-T05 |
| E09-T05 | S | E09-T04 |
| E09-T06 | M | E09-T01, E19-T06 |
| E09-T07 | M | E09-T01, E03-T04 |
| E09-T08 | L | E09-T01, E22-T02 |

**Total: 8 tasks, ~21-28h**
