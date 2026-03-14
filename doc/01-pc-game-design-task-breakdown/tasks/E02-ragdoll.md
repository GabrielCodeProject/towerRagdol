# Epic E02: Ragdoll System
**Layer:** 1
**Design Doc Sections:** 2.1–2.4
**Depends On:** E19 (Content Registry, Behavior Plugins), E22 (Networking, Object Pooling, Performance Budget)

---

## E02-T01: Active ragdoll character controller — skeleton setup
- **Size:** L (4h)
- **Depends on:** E22-T01
- **Description:** Create a humanoid ragdoll skeleton using Unity ConfigurableJoints. Set up hip (root), spine, chest, head, upper/lower arms, upper/lower legs, hands, feet. Configure joint limits for realistic human range of motion. Character uses `Rigidbody` on each bone. Implement `RagdollController` MonoBehaviour that holds references to all joints.
- **Test:** Import a humanoid model, apply ragdoll setup, drop character — limbs should flop naturally within joint limits.
- **Design doc ref:** Section 2.1

## E02-T02: Animation-driven target rotation (active ragdoll)
- **Size:** XL (4-6h)
- **Depends on:** E02-T01
- **Description:** Implement Target Rotation system: `AnimationFollower` reads animation clip bone rotations each frame and sets ConfigurableJoint `targetRotation` to match. Spring/damper values control how tightly ragdoll follows animation. Result: character moves via animation but remains fully physics-driven. Tune spring strength for "wobbly but functional" feel.
- **Test:** Play walk/idle animations, character follows animation while remaining physics-interactive. Push character — it wobbles but recovers.
- **Design doc ref:** Section 2.1

## E02-T03: External force response system
- **Size:** M (2-3h)
- **Depends on:** E02-T02
- **Description:** Create `RagdollForceReceiver` that applies external forces (hits, explosions, falls) to ragdoll bodies. When force exceeds threshold, temporarily reduce joint spring strength (animation override → ragdoll takeover). Implement force types: `Impact` (single hit), `Explosion` (radial), `Sustained` (wind, gravity well). Ragdoll sensitivity tuning via SO config.
- **Test:** Apply impact force to character — character ragdolls realistically. Apply explosion — character launches. Verify sensitivity config changes behavior.
- **Design doc ref:** Section 2.1

## E02-T04: Ragdoll recovery mechanic
- **Size:** M (2-3h)
- **Depends on:** E02-T03
- **Description:** Implement recovery system: after knockdown, character enters "downed" state (full ragdoll, reduced joint springs). After configurable recovery time, joints gradually re-engage (springs ramp up), character scrambles back to feet via recovery animation blended with physics. Vulnerability window during recovery (can be re-knocked). Recovery speed tied to Vitality stat.
- **Test:** Knock character down, wait recovery time, character gets up. Hit during recovery — restarts recovery timer.
- **Design doc ref:** Section 2.1

## E02-T05: Ragdoll combat — melee physics
- **Size:** L (3-4h)
- **Depends on:** E02-T02
- **Description:** Implement physics-driven melee: weapon colliders on hand/weapon bones, raycast along weapon length for hit detection. On hit: calculate damage from impact velocity + weapon stats. Apply knockback force based on hit angle and weapon type. Damage scales with velocity (faster swing = more damage). Armor/weight affects ragdoll response (heavy armor = less knockback).
- **Test:** Swing sword at enemy ragdoll — enemy reacts with physics knockback. Verify damage scales with swing speed. Heavy armor reduces knockback.
- **Design doc ref:** Section 2.2

## E02-T06: Ragdoll combat — ranged projectiles
- **Size:** M (2-3h)
- **Depends on:** E02-T03
- **Description:** Implement projectile system: `Projectile` MonoBehaviour with Rigidbody, travels via physics. On collision: apply knockback force at impact point, deal damage. Projectile types defined by SO (arrow, bolt, fireball shell). Use object pooling for projectiles. Knockback direction based on projectile velocity at impact.
- **Test:** Fire arrow at ragdoll — target staggers/ragdolls backward from impact point. Verify pooling recycles projectiles.
- **Design doc ref:** Section 2.2

## E02-T07: Ragdoll combat — magic/spell forces
- **Size:** M (2-3h)
- **Depends on:** E02-T03
- **Description:** Implement spell force effects: Fireball (AOE explosion + radial ragdoll launch), Ice (freeze joint springs to max → shatter releases), Lightning (chain stun — lock joints briefly). Each spell effect implements `ISpellEffect` and defines force type, magnitude, radius, duration. Spells cost mana (basic mana pool).
- **Test:** Cast fireball near group — all ragdolls launch outward. Cast ice — target freezes then shatters. Chain lightning stuns 3 targets.
- **Design doc ref:** Section 2.2

## E02-T08: Ragdoll combat — hand-to-hand grapple system
- **Size:** L (3-4h)
- **Depends on:** E02-T05
- **Description:** Implement grab/throw/punch system for Brawler style. `GrappleController`: detect grab target (proximity + input), create temporary FixedJoint between player hand and enemy body part. Throw: release joint + apply massive force in aim direction. Punch: short-range impact with exaggerated ragdoll response. Ragdoll wrestling = competing joint forces.
- **Test:** Grab enemy, throw across map — enemy ragdolls through air. Punch enemy — satisfying ragdoll reaction. Two characters grapple — physics tug-of-war.
- **Design doc ref:** Section 2.2

## E02-T09: Hip-only network synchronization
- **Size:** L (3-4h)
- **Depends on:** E02-T02, E22-T02
- **Description:** Implement ragdoll networking: sync only hip (pelvis) `position` + `rotation` over network. Remote clients simulate all other limbs locally using animation + physics. Delta compression: only send when hip changes beyond threshold. Ragdoll "sleep" state: stop sending updates for idle characters. Interpolation buffer for smooth remote display.
- **Test:** Two players see each other's ragdoll characters. Movement appears smooth. Bandwidth monitor shows only hip data transmitting. Idle characters stop sending.
- **Design doc ref:** Section 2.3

## E02-T10: Physics LOD system
- **Size:** M (2-3h)
- **Depends on:** E02-T02, E22-T04
- **Description:** Implement distance-based physics LOD: Full ragdoll physics for entities within close range (configurable, ~30m). Beyond that: switch to animation-driven (disable joint physics, use pure animation). Dead entities: full ragdoll for 5 seconds then fade/despawn. Integrate with `PerformanceBudget` — respect 32 active ragdoll limit.
- **Test:** Spawn 50 characters. Verify only nearest 32 use ragdoll physics. Walk toward distant character — transitions to ragdoll. Kill entity — ragdolls 5s then fades.
- **Design doc ref:** Section 2.4

## E02-T11: Ragdoll configuration ScriptableObject
- **Size:** S (1-2h)
- **Depends on:** E02-T02, E19-T03
- **Description:** Create `RagdollConfig` SO that defines: joint spring/damper values, mass per bone, recovery time, knockback multiplier, sensitivity tuning, LOD distances, sleep thresholds. Referenced by `RagdollController`. Different configs for players vs enemies (players more stable, enemies more floppy for comedy).
- **Test:** Create "floppy" and "stiff" configs, apply to two characters, verify distinctly different physics behavior.
- **Design doc ref:** Section 2.1, 2.4

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E02-T01 | L | E22-T01 |
| E02-T02 | XL | E02-T01 |
| E02-T03 | M | E02-T02 |
| E02-T04 | M | E02-T03 |
| E02-T05 | L | E02-T02 |
| E02-T06 | M | E02-T03 |
| E02-T07 | M | E02-T03 |
| E02-T08 | L | E02-T05 |
| E02-T09 | L | E02-T02, E22-T02 |
| E02-T10 | M | E02-T02, E22-T04 |
| E02-T11 | S | E02-T02, E19-T03 |

**Total: 11 tasks, ~32-39h**
