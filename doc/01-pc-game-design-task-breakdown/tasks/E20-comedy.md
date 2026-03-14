# Epic E20: Comedy Framework
**Layer:** 7
**Design Doc Sections:** 20.1–20.5
**Depends On:** E19 (Event Bus, Content Registry, IComedyFeature interface)

---

## E20-T01: Comedy feature plugin system
- **Size:** M (2-3h)
- **Depends on:** E19-T02, E19-T05
- **Description:** Implement comedy feature plugin architecture: each comedy feature = standalone MonoBehaviour/SO implementing `IComedyFeature` (OnActivate, OnDeactivate, GetTags). Features subscribe to game events via Event Bus — no core code changes. `ComedyFeatureDefinition` SO: name, description, icon, tags (`[visual, audio, physics, gameplay, cosmetic]`), toggle, intensity slider. Features auto-registered from Content/ComedyFeatures/ folder.
- **Test:** Create test comedy feature (confetti on kill), drop SO in folder — auto-registers. Toggle on — confetti spawns on enemy death. Toggle off — no confetti. No core code modified.
- **Design doc ref:** Section 20.1

## E20-T02: Comedy session settings UI
- **Size:** M (2h)
- **Depends on:** E20-T01
- **Description:** Implement host-controlled comedy settings: list all registered comedy features with toggle switches. Filter by category tags. Intensity sliders where applicable. Players can vote to enable/disable features. Settings synced to all clients. Preset modes: "Normal" (visual/audio only), "Party Mode" (all comedy), "Competitive" (comedy disabled).
- **Test:** Host opens settings — all comedy features listed. Toggle confetti on — all clients see confetti. Switch to Competitive — all gameplay comedy disabled.
- **Design doc ref:** Section 20.1, 20.4

## E20-T03: Physics comedy features
- **Size:** L (3-4h)
- **Depends on:** E20-T01, E02-T03
- **Description:** Implement physics comedy features as SO plugins: **Exaggerated ragdoll** (1000x knockback mode), **Ragdoll magnet** (dead enemies stick to player), **Trampoline floors** (bounce zones), **Slippery ice zones** (everyone slides), **Giant head mode** (big heads + tiny bodies), **Low gravity zones**, **Rubber weapons** (bend on hit). Each as standalone IComedyFeature implementation subscribing to relevant events.
- **Test:** Enable "1000x knockback" — enemies launch into orbit. Enable "Giant head mode" — all characters get oversized heads. Each feature independently toggleable.
- **Design doc ref:** Section 20.2

## E20-T04: Audio comedy features
- **Size:** M (2h)
- **Depends on:** E20-T01
- **Description:** Implement audio comedy features: **Fart sound on ragdoll**, **Cartoon bonk on melee**, **Squeaky toy on enemy death**, **Dramatic opera on Core damage**, **Announcer voice pack system**. Each subscribes to relevant event (OnEnemyKilled → squeaky, OnMeleeHit → bonk). Sound packs as purchasable cosmetics. Volume tied to comedy feature intensity slider.
- **Test:** Enable "fart sounds" — every ragdoll event plays fart. Enable "bonk sounds" — melee hits play bonk. Disable — normal sounds return.
- **Design doc ref:** Section 20.2

## E20-T05: Visual comedy features
- **Size:** M (2-3h)
- **Depends on:** E20-T01
- **Description:** Implement visual comedy features: **Confetti on kill**, **Silly hats on enemies** (random per wave), **Screen shake exaggeration**, **Comic book hit effects** (POW!, BAAM!, SPLAT!), **Googly eyes on all characters**, **Trail effects on ragdolling bodies** (rainbow, fire, sparkles). Each as IComedyFeature subscribing to visual events. VFX prefabs referenced in SO.
- **Test:** Enable "confetti" — every kill spawns confetti VFX. Enable "silly hats" — enemies spawn with random hat models. Each toggleable independently.
- **Design doc ref:** Section 20.2

## E20-T06: Gameplay comedy features
- **Size:** L (3-4h)
- **Depends on:** E20-T01, E09-T04
- **Description:** Implement gameplay comedy features (Party Mode only): **Chicken launcher weapon**, **Banana peel trap** (slip chain-reaction), **ACME anvil spell** (drops anvil from sky), **Friendly fire ragdoll mode**, **Enemy costume waves** (tutus, rubber ducks), **Catapult tower** (throws enemies), **Shrink ray spell**, **Growth potion** (giant player ragdolls everything). These affect gameplay — disabled in Endless Mode leaderboards.
- **Test:** Enable "banana peel trap" — enemies slip in chain reaction. Enable "friendly fire ragdoll" — player attacks knock each other around. Verify disabled in competitive mode.
- **Design doc ref:** Section 20.2

## E20-T07: Comedy governance system
- **Size:** S (1-2h)
- **Depends on:** E20-T02
- **Description:** Implement comedy governance: visual/audio comedy always allowed (cosmetic only). Gameplay comedy = "Party Mode" toggle (separate from ranked/competitive). Endless Mode leaderboards disable gameplay comedy features. Comedy feature tags enforce categorization. Community vote integration placeholder (for Steam Workshop potential).
- **Test:** In Endless Mode with leaderboard — gameplay comedy auto-disabled. Visual comedy still works. Party Mode toggle enables/disables gameplay comedy.
- **Design doc ref:** Section 20.4

## E20-T08: Seasonal comedy events
- **Size:** M (2h)
- **Depends on:** E20-T01
- **Description:** Implement seasonal comedy framework: time-limited comedy features activated by date range. Holiday features: Halloween (skeleton enemies), Christmas (snowball weapons), April Fools (everything upside down). Seasonal `ComedyFeatureDefinition` SOs with active date range. Community challenge events with comedy constraints ("Win wave 10 with only banana peel traps"). Auto-activation system.
- **Test:** Set date to Halloween range — skeleton enemy skins auto-activate. Outside range — deactivated. Community challenge tracks comedy-constrained objectives.
- **Design doc ref:** Section 20.5

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E20-T01 | M | E19-T02, E19-T05 |
| E20-T02 | M | E20-T01 |
| E20-T03 | L | E20-T01, E02-T03 |
| E20-T04 | M | E20-T01 |
| E20-T05 | M | E20-T01 |
| E20-T06 | L | E20-T01, E09-T04 |
| E20-T07 | S | E20-T02 |
| E20-T08 | M | E20-T01 |

**Total: 8 tasks, ~18-24h**
