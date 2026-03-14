# PC Game Design Document

## Game Title: [TBD - Working Title: "Ragdoll Realms"]

**Genre:** Co-op 3D Ragdoll Tower Defense / Action RPG / Builder
**Platform:** PC (Steam)
**Engine:** Unity
**Players:** 1-4 Co-op (online + local)
**Tone:** Comedy-action (ragdoll humor meets strategic tower defense)
**Core Inspiration:** Dungeon Defenders (tower defense + action combat hybrid) + Ragdoll Physics + Minecraft Gathering/Building
**Monetization:** Cosmetics + Battle Pass + Premium DLC + Enhancement Convenience Items

---

## 1. CORE IDENTITY

### 1.1 Elevator Pitch
A co-op 3D ragdoll tower defense game where wobbly characters gather resources, build defensive towers and walls around a Core Base, then fight alongside their defenses against escalating enemy waves — using spells, arrows, magic, and melee combat. Explore the map between waves to find boss summoning items and powerful loot. Game ends when the Core is destroyed or the final boss falls.

### 1.2 Core Game Loop
```
[SPAWN] Player spawns near Core Base
    |
    v
[PREPARE PHASE] (X in-game days between waves)
    |-- Gather resources (Minecraft-style mining, chopping, harvesting)
    |-- Build/repair walls, floors, structures around Core
    |-- Craft defense towers (arrow, magic, cannon, slow, etc.)
    |-- Place towers strategically on built structures
    |-- Explore the map for special items (boss summon fragments, combat scrolls)
    |-- Craft/enhance personal gear (weapons, armor, spells)
    |-- Repair damaged defenses from last wave
    |
    v
[DEFEND PHASE] (Enemy wave attacks Core Base)
    |-- Enemies spawn from map edges / spawner nodes, path toward Core
    |-- Towers auto-attack enemies in range
    |-- Players freely move the map: cast spells, shoot arrows, melee combat (Dungeon Defenders style)
    |-- Enemies attack towers, walls, AND Core Base
    |-- Core Base has HP — if it reaches 0, GAME OVER
    |-- Wave cleared = all enemies dead
    |
    v
[WAVE SURVIVED] --> Back to PREPARE PHASE (next wave harder)
    |
    v
[BOSS SUMMONING] (Optional during Prepare Phase)
    |-- Players collect boss summoning fragments from map exploration
    |-- Activate boss altar to trigger boss fight (replaces next wave)
    |-- Defeating boss = progression unlock (new tiers, areas, recipes)
    |
    v
[WIN CONDITION] Final boss defeated
[LOSE CONDITION] Core Base HP reaches 0
[FUTURE] Infinite Wave Mode (endless escalation, leaderboards)
```

### 1.3 Core Pillars
- **Defend the Core** — Everything revolves around keeping the Core Base alive
- **Ragdoll Comedy** — Every hit, explosion, and death is physics-driven hilarity
- **Build & Fortify** — Walls, towers, traps create YOUR unique defense layout
- **Fight Alongside Your Towers** — You're not just building; you're in the action (Dungeon Defenders hybrid)
- **Explore Between Waves** — The map has secrets, boss fragments, and powerful items
- **Risk vs Reward** — Enhancement system where pushing for power means risking what you have

### 1.4 Unique Selling Points
- Ragdoll physics on EVERYTHING (players ragdoll when hit, enemies ragdoll when killed, towers wobble)
- Dungeon Defenders-style hybrid: place towers AND fight in third-person action combat
- Physics-destructible buildings (enemies smash through your walls)
- Explore the open map between waves for boss fragments and special loot
- Punitive enhancement system creates tension and streamer moments

---

## 2. RAGDOLL SYSTEM

### 2.1 Character Physics
- [ ] Active ragdoll system using Unity ConfigurableJoints
- [ ] Characters use Target Rotation to match animations while remaining physics-driven
- [ ] External forces (hits, explosions, falls) override animation control
- [ ] Ragdoll sensitivity tuning (not too floppy, not too stiff)
- [ ] Ragdoll "recovery" mechanic (characters scramble back to feet after knockdown)
- [ ] Players knocked down during defend phase = vulnerable seconds (comedy + stakes)

### 2.2 Ragdoll Combat (Dungeon Defenders-style + Physics)
- [ ] **Melee:** Physics-driven weapon swings — swords, hammers, fists with force-based damage
- [ ] **Ranged:** Bows, crossbows — projectiles apply knockback forces on impact
- [ ] **Magic/Spells:** Fireballs (AOE explosion + ragdoll launch), Ice (freeze + shatter), Lightning (chain stun)
- [ ] **Hand-to-hand:** Grab, throw, punch enemies — ragdoll grapple system
- [ ] Hit detection: raycast-based along weapon length for precision
- [ ] Damage scales with impact velocity + weapon stats + spell power
- [ ] Knockback direction based on hit angle (enemies fly when hit by hammer)
- [ ] Armor/weight affects ragdoll response (heavy armor = less knockback but slower)

### 2.3 Ragdoll Networking
- [ ] Hip-only synchronization (position + rotation of pelvis)
- [ ] Client-side physics simulation for all other limbs
- [ ] Delta compression (only send changes)
- [ ] Ragdoll "sleep" state for idle characters (reduce bandwidth)
- [ ] Physics LOD: full ragdoll for nearby, animated for distant

### 2.4 Performance Budget
- [ ] Maximum active ragdoll entities: 32 (players + nearby enemies)
- [ ] Distant entities: animation-driven (no physics simulation)
- [ ] Dead entities: ragdoll for 5 seconds, then fade/despawn
- [ ] Physics solver: 12+ iterations for stability
- [ ] Target: 60 FPS with 4 players + 20 enemies on mid-range PC

---

## 3. GAME PHASES

### 3.1 Prepare Phase
- [ ] Timer: X in-game days between waves (configurable, default 3-5 minutes real-time)
- [ ] Players can freely roam the entire map
- [ ] All crafting stations accessible
- [ ] Building/placing towers allowed
- [ ] Resource nodes available for gathering
- [ ] Map exploration for special items and boss fragments
- [ ] Repair damaged structures from previous wave
- [ ] Enhancement and gear management
- [ ] Visual countdown timer to next wave (warning at 30s, 10s)
- [ ] Players can vote to "ready up" and skip remaining prep time

### 3.2 Defend Phase
- [ ] Wave announcement with enemy composition preview
- [ ] Enemies spawn from designated spawn points on map edges
- [ ] Enemy pathfinding targets Core Base (primary) and players/towers (secondary)
- [ ] Towers auto-fire at enemies in range
- [ ] Players fight freely: move anywhere, use any combat style
- [ ] Core Base HP bar visible to all players (UI element)
- [ ] Wave complete when all enemies killed
- [ ] NO building/crafting during defend phase (or limited quick-repairs only)
- [ ] Downed players can be revived by teammates (ragdoll crawl to teammate)
- [ ] If all players downed, towers fight alone until players respawn or Core dies

### 3.3 Between-Wave Transition
- [ ] Wave survived celebration (ragdoll victory poses)
- [ ] Loot drops from wave collected
- [ ] Damage report: structures damaged, enemies killed, MVP stats
- [ ] Automatic transition to Prepare Phase
- [ ] Difficulty escalation notification ("Wave 5 incoming — Brutes will appear!")

### 3.4 Game Over Conditions
- [ ] **LOSE:** Core Base HP reaches 0 — Core explodes, game over screen
- [ ] **WIN:** Final boss defeated — victory celebration, final rewards
- [ ] **Endless Mode (Future):** No win condition, waves escalate infinitely, leaderboard score

---

## 4. CORE BASE

### 4.1 The Core
- [ ] Central object on the map — glowing crystal/artifact/heart
- [ ] Has HP pool (scales with upgrades)
- [ ] Cannot be moved
- [ ] Emits light/particles (visual beacon for players)
- [ ] Healing aura: players near Core slowly regenerate HP
- [ ] If damaged, visual cracks appear + alarm sound

### 4.2 Core Upgrades (Crafted)
- [ ] **Core HP Increase:** +25% HP per tier (Tier 1-4)
- [ ] **Core Shield:** Temporary damage absorption barrier (recharges between waves)
- [ ] **Core Healing Aura:** Increased radius and healing rate
- [ ] **Core Alarm Range:** Alerts players when enemies reach inner perimeter
- [ ] Upgrades require rare materials from exploration and boss kills

---

## 5. TOWER DEFENSE SYSTEM (Dungeon Defenders-inspired)

### 5.1 Tower Types

| Tower | Tier | Damage Type | Target | Special |
|-------|------|-------------|--------|---------|
| Arrow Tower | 1 | Physical/Single | Ground | Fast fire rate, low damage |
| Cannon Tower | 1 | Physical/AOE | Ground | Slow fire rate, splash damage, ragdolls enemies |
| Magic Bolt Tower | 2 | Magic/Single | Ground + Air | Medium speed, bypasses armor |
| Frost Tower | 2 | Magic/AOE | Ground | Slows enemies in radius, no direct damage |
| Fire Tower | 3 | Magic/AOE | Ground | DOT (damage over time), sets enemies on fire |
| Lightning Tower | 3 | Magic/Chain | Ground + Air | Chains to 3-5 nearby enemies |
| Ballista | 3 | Physical/Pierce | Ground | Pierces through multiple enemies in a line |
| Void Tower | 4 | Void/AOE | All | Massive damage, slow attack, endgame tower |

### 5.2 Tower Placement Rules
- [ ] Towers MUST be placed on player-built structures (floors, walls, platforms)
- [ ] Cannot place towers directly on terrain (encourages building)
- [ ] Towers have placement radius (minimum spacing between towers)
- [ ] Tower facing/rotation matters for directional towers (Ballista)
- [ ] Towers can be picked up and repositioned during Prepare Phase
- [ ] Towers have HP — enemies can destroy them
- [ ] Destroyed towers drop partial materials for rebuilding

### 5.3 Tower Crafting
- [ ] Towers crafted at specific stations (matches crafting tier)
- [ ] Each tower requires resources + a Tower Core item (dropped by enemies/exploration)
- [ ] Tower upgrades: Level 1 → 2 → 3 (increased damage, range, fire rate)
- [ ] Tower upgrades done in-place during Prepare Phase (costs resources)
- [ ] Max tower count per player (scales with level/skill tree)

### 5.4 Tower Enhancement (Ties into Enhancement System)
- [ ] Towers can be enhanced like equipment (+0 to +15)
- [ ] Enhanced towers gain bonus damage, range, and special effects
- [ ] Enhancement failure: tower loses enhancement levels (NOT destroyed)
- [ ] Creates strategic choice: enhance personal gear or enhance towers?

### 5.5 Trap System
- [ ] **Spike Trap:** Damages enemies walking over (ragdoll trip + damage)
- [ ] **Tar Pit:** Slows enemies in area
- [ ] **Explosive Mine:** One-shot AOE explosion (sends enemies flying — comedy gold)
- [ ] **Spring Trap:** Launches enemies backward (ragdoll catapult)
- [ ] Traps placed on ground level (don't require built floors)
- [ ] Traps have charges — consumed on trigger, must be refilled between waves

---

## 6. GATHERING SYSTEM (Minecraft-style)

### 6.1 Resource Types
- [ ] **Tier 1 (Starter):** Wood, Stone, Fiber, Clay
- [ ] **Tier 2 (Intermediate):** Iron Ore, Copper, Thick Leather, Hardwood
- [ ] **Tier 3 (Advanced):** Crystal, Darksteel, Enchanted Fiber, Dragon Bone
- [ ] **Tier 4 (Endgame):** Void Essence, Celestial Ore, Primordial Shard

### 6.2 Gathering Mechanics
- [ ] Tool-gated gathering (stone pickaxe can't mine iron)
- [ ] Tool tiers match resource tiers
- [ ] Ragdoll physics on gathering (character stumbles when mining, trees fall with physics)
- [ ] Co-op gathering bonus (2+ players mining same node = faster + bonus resources)
- [ ] Resource nodes respawn between waves (partial respawn) and fully on boss kill

### 6.3 Biome-Locked Resources
- [ ] **Starting Zone (near Core):** Tier 1 resources
- [ ] **Mid-range Zone:** Tier 1-2 resources (further from Core)
- [ ] **Outer Zone:** Tier 2-3 resources (dangerous, near enemy spawners)
- [ ] **Boss Zones:** Tier 3-4 resources (unlocked after defeating bosses)
- [ ] **Hidden Areas:** Special resources behind puzzles/destructible terrain

---

## 7. BUILDING SYSTEM (Minecraft-style + Physics)

### 7.1 Building Mechanics
- [ ] Grid-based piece placement (snap system)
- [ ] 90-degree and 45-degree rotation
- [ ] Structural integrity system (supports required for tall structures)
- [ ] Material-based durability (wood < stone < iron < crystal)
- [ ] Physics-based destruction (enemies break through walls, towers collapse)
- [ ] Ragdoll building interactions (character wobbles while placing heavy pieces)
- [ ] Building only during Prepare Phase (or very limited during Defend)

### 7.2 Building Pieces
- [ ] **Foundations:** Floor tiles, platforms, ramps (REQUIRED for tower placement)
- [ ] **Walls:** Full walls, half walls, windows, doorframes, crenellations
- [ ] **Roofs:** Flat, angled, peaked (protect towers from aerial enemies)
- [ ] **Functional:** Crafting stations, storage, furnaces, anvils, tower workbench
- [ ] **Defensive:** Reinforced walls, gates, barricades, murder holes
- [ ] **Utility:** Bridges, stairs, elevated platforms (create tower vantage points)

### 7.3 Strategic Building
- [ ] Walls funnel enemies into chokepoints (tower kill zones)
- [ ] Elevated platforms give towers better range/angles
- [ ] Multiple layers of defense (outer wall → traps → inner wall → towers → Core)
- [ ] Enemies path around intact walls but break through if no path exists
- [ ] Building layout IS the strategy — bad layout = Core takes damage

---

## 8. MAP & EXPLORATION

### 8.1 Map Structure
- [ ] Core Base at center of map
- [ ] Concentric zones radiating outward (safe → moderate → dangerous)
- [ ] Enemy spawn points on map edges/corners
- [ ] Resource nodes distributed across zones
- [ ] Points of Interest (POIs) scattered throughout map
- [ ] Map size: medium (5-10 min walk from Core to map edge)

### 8.2 Points of Interest (Exploration Rewards)
- [ ] **Boss Altars:** Locations where boss summon fragments are used
- [ ] **Ancient Chests:** Contain rare equipment, recipes, Tower Cores
- [ ] **Boss Fragment Shrines:** Contain fragments needed to summon bosses
- [ ] **Combat Scroll Pedestals:** One-time-use powerful spell scrolls
- [ ] **Hidden Caves:** Contain Tier 3-4 resources, mini-boss guardians
- [ ] **Corrupted Nodes:** Enemy spawner sources (destroy to reduce wave difficulty)
- [ ] **NPC Camps:** Wandering merchants, quest givers (between waves)

### 8.3 Exploration Risk/Reward
- [ ] Further from Core = better loot but more dangerous overworld enemies
- [ ] Time spent exploring = less time building/preparing defenses
- [ ] Some POIs guarded by elite enemies (mini-boss encounters)
- [ ] Boss fragments REQUIRE exploration (can't just build and defend — must venture out)
- [ ] Co-op split: some players explore while others build (strategic choice)

### 8.4 Boss Summoning Items
- [ ] Each boss requires X fragments (scattered across map POIs)
- [ ] Fragments respawn after boss is defeated (for re-farming)
- [ ] Fragment locations change each playthrough (procedural placement)
- [ ] Mini-map markers for discovered fragment locations
- [ ] Boss summon altar located at specific map location (must travel there)

---

## 9. COMBAT SYSTEM (Dungeon Defenders Hybrid)

### 9.1 Combat Styles (Player Choice)
- [ ] **Melee Fighter:** Swords, hammers, axes — high damage, close range, ragdoll grapples
- [ ] **Archer/Ranger:** Bows, crossbows — medium damage, long range, precision shots
- [ ] **Mage/Caster:** Spell staffs, wands — AOE damage, elemental effects, crowd control
- [ ] **Brawler:** Fists, gauntlets — grab and throw enemies, ragdoll wrestling
- [ ] Players can switch styles by equipping different weapons (not locked to class)

### 9.2 Spell System
- [ ] **Fireball:** AOE explosion, launches enemies (ragdoll), DOT
- [ ] **Ice Lance:** Single target freeze, shatter on next hit
- [ ] **Lightning Bolt:** Chain damage to nearby enemies, stun
- [ ] **Earth Wall:** Summon temporary wall to redirect enemies
- [ ] **Heal Wave:** AOE heal for players and repair for nearby structures
- [ ] **Gravity Well:** Pull enemies together (combo setup for AOE)
- [ ] Spells cost Mana (regenerates over time, potions)
- [ ] Spell scrolls (found in exploration) = one-time powerful versions

### 9.3 Combat During Defend Phase
- [ ] Players move freely across entire map during waves
- [ ] Can fight at tower chokepoints OR rush to where Core is taking damage
- [ ] Enemies that reach Core take priority — high threat alert
- [ ] Players near Core get defense bonus (proximity buff)
- [ ] Revive downed allies by standing near them (ragdoll crawl animation)
- [ ] Combat XP earned during defend phase (primary XP source)

---

## 10. RPG FEATURES

### 10.1 Character Stats
- [ ] **Strength:** Melee damage, carry capacity, mining speed
- [ ] **Agility:** Movement speed, dodge recovery, attack speed
- [ ] **Vitality:** HP, stamina, ragdoll recovery speed
- [ ] **Intelligence:** Spell damage, mana pool, crafting quality bonus
- [ ] **Tower Mastery:** Tower damage bonus, tower range bonus, max tower count
- [ ] **Luck:** Drop rates, critical hits, enhancement luck modifier

### 10.2 Leveling System
- [ ] XP from combat (defend phase primary), gathering, crafting, boss kills
- [ ] Level cap: 50 (soft cap), 100 (hard cap via endgame)
- [ ] Stat points per level (player-allocated)
- [ ] Milestone unlocks every 10 levels (new abilities, recipes, tower types)

### 10.3 Skill Trees
- [ ] **Warrior Path:** Melee specialization — ragdoll grapples, power attacks, taunt
- [ ] **Ranger Path:** Ranged specialization — multi-shot, piercing arrows, traps
- [ ] **Mage Path:** Spell specialization — bigger AOE, lower cooldowns, new spells
- [ ] **Architect Path:** Tower/building specialization — tower buffs, faster building, extra tower slots
- [ ] Players can hybrid between paths (not locked to one)

### 10.4 Equipment Slots
- [ ] Head, Chest, Legs, Boots, Gloves
- [ ] Main Hand, Off Hand
- [ ] 2 Accessory slots (rings, amulets)
- [ ] Each piece visually affects ragdoll character model

---

## 11. CRAFTING SYSTEM

### 11.1 Crafting Stations (Tiered)
- [ ] **Workbench (Tier 1):** Basic tools, weapons, armor, Arrow/Cannon towers
- [ ] **Forge (Tier 2):** Metal weapons, medium armor, Magic/Frost towers
- [ ] **Arcane Table (Tier 3):** Enchanted gear, spell scrolls, Fire/Lightning/Ballista towers
- [ ] **Celestial Anvil (Tier 4):** Endgame gear, Void towers, legendary recipes

### 11.2 Recipe System
- [ ] Recipes discovered through: exploration POIs, boss drops, quest rewards
- [ ] Tiered recipes require matching station tier
- [ ] Component crafting (raw → processed → component → final item)
- [ ] Recipe quality: Normal → Fine → Superior → Masterwork (Intelligence stat)
- [ ] Tower recipes separate from gear recipes (dedicated Tower Workbench)

### 11.3 Resource Sinks
- [ ] Tier 1 materials always needed (building materials, tower upkeep, trap refills)
- [ ] Repair costs consume materials (post-wave rebuilding)
- [ ] Enhancement consumes materials on both success and failure
- [ ] Tower crafting + upgrading = constant resource demand
- [ ] Trap refueling between waves

---

## 12. ENHANCEMENT SYSTEM

### 12.1 Enhancement Levels
- [ ] Equipment AND towers can be enhanced from +0 to +25
- [ ] Each level increases stats (damage, defense, speed, tower damage, etc.)

### 12.2 Enhancement Tiers & Risk

| Level Range | Success Rate | On Failure | Risk Level |
|-------------|-------------|------------|------------|
| +0 to +5   | 100%        | N/A        | Safe       |
| +6 to +9   | 70-50%      | Stay same  | Low        |
| +10 to +14  | 50-30%      | -1 level   | Medium     |
| +15 to +19  | 30-15%      | -1 to -3 levels | High |
| +20 to +25  | 15-5%       | -3 levels OR break | Extreme |

### 12.3 Pity System (Failstack)
- [ ] Each failure grants +1 Failstack
- [ ] Each Failstack adds +0.5% to next enhancement success rate
- [ ] Failstacks are consumed on success
- [ ] **Hard pity:** After X consecutive failures, guaranteed success
  - +10-14: Guaranteed after 10 failures
  - +15-19: Guaranteed after 20 failures
  - +20-25: Guaranteed after 40 failures
- [ ] Pity progress bar visible in UI (transparent system)

### 12.4 Protection Mechanics
- [ ] **Enhancement Shield (crafted):** Prevents level regression on failure (consumed)
- [ ] **Restoration Scroll (crafted):** Restores broken equipment to +10 (consumed)
- [ ] **Lucky Charm (rare drop):** +10% success rate for one attempt
- [ ] **Premium Protection Stone (cash shop):** Prevents regression AND breakage (convenience)
  - MUST also be obtainable through gameplay (weekly quest reward, boss drop)

### 12.5 Enhancement UI
- [ ] Clear success rate percentage displayed
- [ ] Failstack counter visible
- [ ] Pity progress bar
- [ ] Cost forecast ("Expected attempts to guarantee: X")
- [ ] Celebratory failure messaging ("Failstack +3! Getting closer!")
- [ ] History log of recent enhancement attempts

### 12.6 Broken Equipment System
- [ ] Broken items retain appearance but lose ALL stats
- [ ] Can be restored at Celestial Anvil using Restoration materials
- [ ] Restored item returns to +10 (not original level)
- [ ] Broken equipment cannot be traded
- [ ] Visual indicator: cracked/damaged appearance on character model

### 12.7 Tower Enhancement
- [ ] Towers enhanced same way as gear but with separate materials (Tower Stones)
- [ ] On failure: towers lose enhancement levels but are NEVER destroyed
- [ ] Enhanced towers visually change (glow, larger, more elaborate)
- [ ] Strategic choice: enhance personal gear or invest in tower defense?

---

## 13. ENEMY WAVE SYSTEM

### 13.1 Wave Structure
- [ ] Waves attack every X in-game days (configurable, default: every 3-5 min real-time prep)
- [ ] Wave number escalates difficulty (more points, harder enemies)
- [ ] Total waves to reach final boss: 15-25 (configurable)
- [ ] Boss wave replaces normal wave when players summon a boss

### 13.2 Wave Composition (Point-Cost System)

| Enemy Type | Cost | Behavior | Special |
|-----------|------|----------|---------|
| Goblin (melee) | 1 | Rush Core, low HP | Ragdolls easily |
| Archer (ranged) | 2 | Stay at range, shoot towers | Targets towers first |
| Brute (heavy) | 4 | Slow, high HP | Destroys walls/buildings |
| Shaman (support) | 3 | Buffs nearby enemies | Heals allies, priority target |
| Bomber (siege) | 5 | Runs at Core, explodes | Massive Core damage, one-shot |
| Flyer (aerial) | 3 | Flies over walls | Ignores ground defenses |
| Tunneler (sapper) | 4 | Digs under walls | Bypasses wall defenses |
| Elite (mini-boss) | 8 | Random special abilities | Bonus loot on kill |
| Wave Boss | 15 | Appears every 5th wave | Major threat, good rewards |

### 13.3 Wave Scaling
- [ ] Base difficulty pool: 10 points (Wave 1)
- [ ] +5 points per wave
- [ ] +10 points per additional player in session
- [ ] Composition randomization within budget (procedural variety)
- [ ] Special themed waves: "Siege" (all Brutes/Bombers), "Swarm" (all Goblins), "Aerial Assault" (Flyers)
- [ ] Wave preview during Prepare Phase (shows enemy types coming)

### 13.4 Wave Spawning
- [ ] Spawn points on map edges (multiple directions)
- [ ] Spawn direction rotates/randomizes per wave (can't just defend one side)
- [ ] Pathfinding toward Core Base (primary target)
- [ ] Enemies attack obstacles in their path (walls, towers)
- [ ] Object pooling for performance
- [ ] Max active enemies cap: 40 (performance budget)
- [ ] Enemies use animation-driven movement, ragdoll on hit/death

### 13.5 Destroying Corrupted Nodes (Exploration Bonus)
- [ ] Overworld corrupted nodes act as bonus enemy spawn points during waves
- [ ] If players destroy a node during Prepare Phase, fewer enemies spawn from that direction
- [ ] Creates exploration incentive: explore and destroy nodes = easier defense

---

## 14. ENEMY NODE SPAWNER SYSTEM (Overworld)

### 14.1 Overworld Enemies (Non-Wave)
- [ ] **Enemy Camps:** Guard POIs and resources, hostile on proximity
- [ ] **Corrupted Nodes:** Destructible spawner objects (affect wave difficulty)
- [ ] **Roaming Packs:** Patrol between nodes, can stumble into player base
- [ ] **Mini-Boss Guards:** Protect boss fragment shrines and hidden caves

### 14.2 Spawner Mechanics
- [ ] Proximity activation (only active when players nearby)
- [ ] Spawn timer + maximum active enemies per node
- [ ] Difficulty scales with distance from Core
- [ ] Destroying node = area cleared, reduces next wave spawn points
- [ ] Nodes regenerate after X waves (can't permanently clear all)

---

## 15. BOSS FIGHTS

### 15.1 Boss Summoning
- [ ] Collect X boss fragments from map exploration (POIs, shrines, hidden areas)
- [ ] Travel to Boss Altar on map
- [ ] Activate altar → boss fight replaces next wave
- [ ] During boss fight, Core is STILL vulnerable (boss sends minions at Core)
- [ ] Boss fragments have procedural placement each playthrough

### 15.2 Bosses (Progression Gates)

| Boss | Fragments Needed | Unlock on Defeat |
|------|-----------------|------------------|
| Mushroom Golem (Tutorial Boss) | 3 | Tier 2 crafting, new map zone |
| Ancient Treant | 5 | Tier 3 crafting, advanced towers |
| Swamp Hydra | 7 | Tier 4 crafting, spell upgrades |
| Fire Dragon | 9 | Endgame zone, Void towers |
| Chaos Entity (Final Boss) | 12 | Victory / Endless Mode unlock |

### 15.3 Boss Mechanics
- [ ] Phase transitions (2-3 phases per boss)
- [ ] Ragdoll knockback on boss attacks (comedy + danger)
- [ ] Weak points that require player positioning
- [ ] Boss sends minion waves at Core during fight (split attention)
- [ ] Environmental hazards (arena-specific)
- [ ] Boss can destroy player structures in arena area

### 15.4 Co-op Boss Scaling
- [ ] **HP Scaling:** +30% per additional player
- [ ] **Minion Scaling:** +1 additional minion wave per extra player
- [ ] **Damage Scaling:** +4% per additional player
- [ ] Minimum difficulty floor (can't be trivialized)

### 15.5 Boss Rewards
- [ ] Guaranteed progression unlock (new tier/zone/recipes)
- [ ] Unique boss material drop (needed for top-tier crafting)
- [ ] Enhancement materials
- [ ] First-kill bonus: unique cosmetic
- [ ] Re-farmable (fragments respawn after defeat)

---

## 16. REWARD SYSTEM

### 16.1 Loot Distribution (Individual)
- [ ] Each player receives their own loot drops (no stealing)
- [ ] Wave clear: shared resource reward + individual random drops
- [ ] Boss kills: guaranteed drop for each participant
- [ ] Exploration POIs: first player to open gets primary loot, others get secondary

### 16.2 Reward Types
- [ ] **Resources:** Raw materials, processed components
- [ ] **Equipment:** Weapons, armor, accessories (tiered by content)
- [ ] **Tower Cores:** Required to craft towers (dropped by enemies + exploration)
- [ ] **Recipes:** New crafting blueprints
- [ ] **Enhancement Materials:** Failstacks, protection items, upgrade stones
- [ ] **Boss Fragments:** Summon fragments for boss encounters
- [ ] **Combat Scrolls:** One-use powerful spells found in exploration
- [ ] **Cosmetics:** Skins, emotes, building styles
- [ ] **Currency:** Gold (trading), Void Crystals (endgame vendor)

### 16.3 Reward Loops

| Loop | Frequency | Reward Type | Purpose |
|------|-----------|-------------|---------|
| Enemy Kill | Per kill | Resources, XP, Tower Cores | Immediate gratification |
| Wave Clear | Per wave | Equipment, materials, gold | Session milestone |
| Exploration | Per POI | Fragments, scrolls, recipes, gear | Exploration incentive |
| Boss Kill | Per boss | Progression unlock, unique drops | Major milestone |
| Corrupted Node | Per destroy | Reduced wave difficulty + loot | Exploration reward |
| Daily Quest | Daily | Currency, enhancement mats | Daily login incentive |
| Weekly Challenge | Weekly | Cosmetics, rare materials | Weekly engagement |
| Season Pass | Monthly | Cosmetic set, building styles | Long-term goal |

---

## 17. MONETIZATION STRATEGY

### 17.1 Base Game
- [ ] Premium purchase: $19.99 - $29.99
- [ ] All gameplay content included (no content paywalls)

### 17.2 Cosmetic Shop
- [ ] Character skins (ragdoll outfits, funny costumes)
- [ ] Weapon skins (visual only, no stat change)
- [ ] Tower skins (visual reskins of tower types)
- [ ] Building style packs (medieval, sci-fi, fantasy variants)
- [ ] Emotes and ragdoll poses
- [ ] Death animations (custom ragdoll deaths)
- [ ] Core Base skins (change the Core's appearance)

### 17.3 Battle Pass (Seasonal)
- [ ] Free track: resources, basic cosmetics
- [ ] Premium track ($9.99): exclusive cosmetics, building styles, emotes
- [ ] No gameplay advantages in premium track
- [ ] 60-day seasons

### 17.4 Enhancement Convenience (Regulation-Compliant)
- [ ] Premium Protection Stones: prevent enhancement regression
  - ALSO obtainable through weekly quest + boss drops
  - Cash shop version is CONVENIENCE, not EXCLUSIVE
- [ ] Enhancement material bundles (saves time, not power)
- [ ] All probabilities clearly displayed in-game
- [ ] Real-money cost equivalent shown for all purchases
- [ ] Parental controls for under-16 accounts

### 17.5 DLC Expansions
- [ ] New map layouts with unique biomes, enemies, bosses ($9.99 each)
- [ ] New tower types and spell schools
- [ ] Co-op compatible: non-DLC players can join DLC owner's session
- [ ] 2-3 DLCs per year planned

---

## 18. PROGRESSION FLOW

```
[GAME START]
    |
    v
[Spawn at Core Base] → Prepare Phase begins
    |
    v
[Gather Tier 1] → Craft basic gear + Arrow/Cannon towers → Build walls around Core
    |
    v
[WAVE 1-3] → Defend Core → Survive → Repair/Expand
    |
    v
[Explore map] → Find Mushroom Golem fragments (3) → Discover Tier 2 resource nodes
    |
    v
[SUMMON BOSS 1: Mushroom Golem] → Defeat → Unlock Tier 2 crafting + new map zone
    |
    v
[Gather Tier 2] → Craft better gear + Magic/Frost towers → Enhance gear (+0 to +9)
    |
    v
[WAVE 4-8] → Harder enemies (Brutes, Shamans) → Build layered defenses
    |
    v
[Explore deeper] → Find Treant fragments (5) → Combat scrolls + hidden caves
    |
    v
[SUMMON BOSS 2: Ancient Treant] → Defeat → Unlock Tier 3 + advanced towers
    |
    v
[WAVE 9-13] → Enhancement risk begins (+10-14) → Strategic tower enhancement
    |
    v
[Explore dangerous zones] → Hydra fragments (7) → Risk vs reward expeditions
    |
    v
[SUMMON BOSS 3: Swamp Hydra] → Defeat → Unlock Tier 4 + spell upgrades
    |
    v
[WAVE 14-18] → Full enemy roster → High-risk enhancement (+15-19)
    |
    v
[Explore endgame zone] → Dragon fragments (9) → Elite enemy encounters
    |
    v
[SUMMON BOSS 4: Fire Dragon] → Defeat → Unlock Void towers + endgame zone
    |
    v
[WAVE 19-23] → Extreme difficulty → Enhancement +20-25 (gear can break)
    |
    v
[Find Chaos Entity fragments (12)] → Final preparation → Max gear/towers
    |
    v
[SUMMON FINAL BOSS: Chaos Entity] → All mechanics combined → VICTORY
    |
    v
[ENDLESS MODE UNLOCKED] → Infinite waves → Leaderboards → Seasonal content
```

---

## 19. EXTENSIBILITY ARCHITECTURE (Future-Proof Design)

> **DESIGN PHILOSOPHY:** Every system must be built as a modular, data-driven plugin.
> New maps, classes, items, enemies, towers, buildings, and "braindead" comedy features
> must be addable WITHOUT touching core game code. If adding content requires modifying
> existing systems, the architecture is wrong.

### 19.1 Data-Driven Content Pipeline
- [ ] **All game content defined in ScriptableObjects (SO) or JSON/YAML data files**
  - Enemies: EnemyDefinition SO (stats, model, AI behavior ID, ragdoll config, loot table ID)
  - Towers: TowerDefinition SO (stats, model, projectile type, targeting logic ID, upgrade chain)
  - Items/Equipment: ItemDefinition SO (stats, slot, model, enhancement curve, rarity)
  - Spells: SpellDefinition SO (damage, AOE, element, VFX, ragdoll force)
  - Recipes: RecipeDefinition SO (inputs, output, station tier, unlock condition)
  - Buildings: BuildingPieceDefinition SO (model, HP, material tier, snap rules)
  - Maps: MapDefinition SO (terrain, Core position, spawn points, POI slots, biome rules)
  - Bosses: BossDefinition SO (phases, attacks, fragment count, rewards, scaling curve)
  - Player Classes: ClassDefinition SO (base stats, skill tree ID, starting gear, passive abilities)
- [ ] **Adding new content = creating new SO asset + dropping it in a content folder**
- [ ] No hardcoded IDs, enums, or switch statements for content types
- [ ] Content auto-discovered at load time via asset scanning or registry pattern

### 19.2 Registry & Plugin Systems
- [ ] **Content Registry:** Central registry that auto-discovers and indexes all SOs at startup
  - `EnemyRegistry`, `TowerRegistry`, `ItemRegistry`, `SpellRegistry`, `ClassRegistry`, etc.
  - New content auto-registers by existing in the correct asset folder
  - Registries expose query methods: `GetByID()`, `GetByTier()`, `GetByTag()`
- [ ] **Behavior Plugin System:** AI, tower targeting, spell effects as pluggable strategy scripts
  - Enemy AI: `IEnemyBehavior` interface → swap/add AI patterns without touching enemy core
  - Tower Targeting: `ITargetingStrategy` interface → "nearest", "strongest", "lowest HP", custom
  - Spell Effects: `ISpellEffect` interface → chain effects, combo systems, new elements
- [ ] **Event Bus / Message System:** Decoupled communication between systems
  - `OnEnemyKilled`, `OnWaveStart`, `OnWaveEnd`, `OnBossDefeated`, `OnCoreHit`, etc.
  - New features subscribe to events without modifying emitting systems
  - Braindead comedy features hook into events (e.g., "confetti on kill", "fart sound on ragdoll")

### 19.3 Modular System Interfaces
- [ ] **Each major system exposes a clean interface for extension:**

| System | Interface | "Add new X" requires |
|--------|-----------|---------------------|
| Enemies | `IEnemyBehavior` + `EnemyDefinition` SO | 1 SO + 1 model + optional AI script |
| Towers | `ITargetingStrategy` + `TowerDefinition` SO | 1 SO + 1 model + optional targeting script |
| Spells | `ISpellEffect` + `SpellDefinition` SO | 1 SO + 1 VFX + optional effect script |
| Items | `ItemDefinition` SO | 1 SO + 1 model |
| Buildings | `BuildingPieceDefinition` SO | 1 SO + 1 model + snap config |
| Classes | `ClassDefinition` SO + `SkillTreeDefinition` SO | 2 SOs + skill scripts |
| Maps | `MapDefinition` SO + scene file | 1 SO + 1 Unity scene |
| Bosses | `BossDefinition` SO + `IBossPhase` scripts | 1 SO + phase scripts + model |
| Recipes | `RecipeDefinition` SO | 1 SO (references existing items) |
| Loot Tables | `LootTableDefinition` SO | 1 SO (references existing items) |

### 19.4 Tag & Category System
- [ ] **Flexible tagging instead of rigid categories:**
  - Items tagged: `[weapon, melee, sword, fire, tier3]`
  - Enemies tagged: `[ground, armored, siege, swamp]`
  - Towers tagged: `[magic, aoe, slow, tier2]`
- [ ] Tags used for: filtering, wave composition rules, loot table conditions, UI display
- [ ] New tags addable without code changes (string-based or SO-based)
- [ ] Allows cross-cutting queries: "all fire-element items" or "all siege-type enemies"

### 19.5 Scaling & Balance Framework
- [ ] **All balance values in external data (NOT hardcoded)**
  - Damage formulas reference stat curves (SO)
  - Wave difficulty curves in data files
  - Enhancement success rates in data tables
  - Boss scaling multipliers in config
- [ ] **Balance profiles:** Swap entire balance configs for different game modes
  - "Normal", "Hard", "Endless", "Braindead Party Mode"
- [ ] **Live tuning:** Values reloadable at runtime for testing without rebuilding

### 19.6 Map System (Expandable)
- [ ] **Maps as self-contained packages:**
  - Each map = Unity scene + MapDefinition SO
  - Map SO defines: Core position, spawn point slots, biome type, resource distribution rules, POI slots, boss altar locations, ambient music/atmosphere
  - POIs and resources placed procedurally from map rules (not hardcoded positions)
- [ ] **Map-specific content:**
  - Maps can reference unique enemy sets, tower types, environmental hazards
  - DLC map can introduce new enemies that only appear on that map
- [ ] **Map selection:** Players choose map before session starts (like Dungeon Defenders map select)

### 19.7 Class System (Expandable)
- [ ] **Classes as data packs:**
  - ClassDefinition SO: base stats, stat growth curve, passive abilities, starting equipment
  - SkillTreeDefinition SO: tree layout, node connections, skill references
  - Each skill = `ISkillEffect` script + SkillDefinition SO
- [ ] **Adding a new class:** Create ClassDefinition SO + SkillTreeDefinition SO + skill scripts
- [ ] **No class-locked content:** Any class can use any weapon/spell, but class passives favor certain playstyles
- [ ] **Future classes examples:** Necromancer (summons), Engineer (auto-repair towers), Alchemist (potion throws), Jester (braindead comedy skills)

---

## 20. BRAINDEAD COMEDY FEATURE FRAMEWORK

> **DESIGN PHILOSOPHY:** This is a comedy game first. Absurd, stupid, hilarious features
> will be added over time. The architecture MUST support plugging in braindead ideas
> without breaking core gameplay. Comedy features are first-class citizens, not afterthoughts.

### 20.1 Comedy Feature Plugin System
- [ ] **Comedy features hook into the Event Bus — no core code changes needed**
- [ ] Each comedy feature = standalone MonoBehaviour/ScriptableObject that subscribes to game events
- [ ] Comedy features can be toggled on/off per session (host setting)
- [ ] Comedy features tagged by category: `[visual, audio, physics, gameplay, cosmetic]`

### 20.2 Comedy Feature Categories

**Physics Comedy (ragdoll interactions):**
- [ ] Exaggerated ragdoll reactions (1000x knockback mode)
- [ ] Ragdoll magnet (dead enemies stick to player character)
- [ ] Trampoline floors (bounce enemies and players sky-high)
- [ ] Slippery ice zones (everyone slides around uncontrollably)
- [ ] Giant head mode (big heads + tiny bodies on all characters)
- [ ] Low gravity zones in certain map areas
- [ ] Rubber weapons (swords that bend and boing on hit)

**Audio Comedy:**
- [ ] Fart sound on every ragdoll event
- [ ] Cartoon bonk sounds on melee hits
- [ ] Squeaky toy sound when enemies die
- [ ] Dramatic opera music when Core takes damage
- [ ] Announcer voice pack system (Morgan Freeman, wrestling announcer, etc.)
- [ ] Sound packs as cosmetic purchases

**Visual Comedy:**
- [ ] Confetti explosion on enemy kills
- [ ] Enemies wear silly hats (random per wave)
- [ ] Screen shake exaggeration mode
- [ ] Comic book hit effects (POW!, BAAM!, SPLAT!)
- [ ] Googly eyes on all characters and enemies
- [ ] Trail effects on ragdolling bodies (rainbow, fire, sparkles)

**Gameplay Comedy (braindead features):**
- [ ] Chicken launcher weapon (shoots live chickens that ragdoll into enemies)
- [ ] Banana peel trap (enemies slip and ragdoll chain-reaction)
- [ ] ACME anvil spell (drops anvil from sky, massive ragdoll impact)
- [ ] Friendly fire ragdoll mode (players knock each other around)
- [ ] Enemy costume waves (all enemies in tutus, or all as rubber ducks)
- [ ] Tower that throws enemies instead of killing them (catapult tower)
- [ ] Shrink ray spell (enemies become tiny but fast)
- [ ] Growth potion (player becomes giant temporarily, ragdolls everything)

### 20.3 How to Add a New Comedy Feature
```
ADDING A BRAINDEAD FEATURE (Developer Checklist):

1. Create a new script implementing IComedyFeature interface:
   - OnActivate() — setup
   - OnDeactivate() — cleanup
   - GetTags() — category tags for UI filtering

2. Subscribe to relevant game events via Event Bus:
   - Example: OnEnemyKilled → spawn confetti VFX at death position

3. Create a ComedyFeatureDefinition SO:
   - Name, description, icon, tags
   - Toggle: on/off
   - Intensity slider (if applicable)

4. Drop SO in Content/ComedyFeatures/ folder
   → Auto-registered at startup
   → Appears in session settings menu
   → Players can vote to enable/disable

NO CORE CODE CHANGES NEEDED.
```

### 20.4 Comedy Feature Governance
- [ ] Comedy features NEVER affect competitive balance (Endless Mode leaderboards disable gameplay comedy)
- [ ] Visual/audio comedy always allowed (cosmetic only)
- [ ] Gameplay comedy features = "Party Mode" toggle (separate from ranked/competitive)
- [ ] Community can suggest + vote on comedy features (Steam Workshop potential)

### 20.5 Seasonal Comedy Events
- [ ] Holiday-themed comedy features (Halloween: all enemies are skeletons, Christmas: snowball weapons)
- [ ] Limited-time braindead modifiers (April Fools: everything is upside down)
- [ ] Community challenge events with comedy constraints ("Win wave 10 with only banana peel traps")

---

## 21. CONTENT EXPANSION ROADMAP

> How each system grows over time. All additions use the extensibility architecture (Section 19).

### 21.1 Maps
| Release | Maps | Theme |
|---------|------|-------|
| Launch | 1 | Classic fantasy (meadow/forest/swamp) |
| DLC 1 | +1 | Volcanic / Underground |
| DLC 2 | +1 | Floating Islands / Sky |
| DLC 3 | +1 | Underwater / Coral Reef |
| Community | +N | Steam Workshop maps (if mod support added) |

### 21.2 Player Classes
| Release | Classes | Role |
|---------|---------|------|
| Launch | Warrior, Ranger, Mage, Architect | Core 4 combat styles |
| Update 1 | +Brawler | Pure ragdoll hand-to-hand specialist |
| DLC 1 | +Necromancer | Summon undead minions to fight |
| DLC 2 | +Alchemist | Potion-based combat + buffing |
| DLC 3 | +Jester | Comedy-focused class (braindead skills) |
| Future | +Engineer, +Summoner, +Ninja... | Community-requested |

### 21.3 Towers
| Release | Towers | New Mechanic |
|---------|--------|--------------|
| Launch | Arrow, Cannon, Magic Bolt, Frost, Fire, Lightning, Ballista, Void | Core 8 |
| Update 1 | +Catapult (throws enemies) | Physics-comedy tower |
| DLC 1 | +Lava Tower, +Poison Tower | New damage types |
| DLC 2 | +Wind Tower (pushes enemies back) | Crowd control |
| DLC 3 | +Bubble Tower (traps enemies) | Utility |
| Seasonal | +Holiday towers (snowball, pumpkin, etc.) | Cosmetic variants |

### 21.4 Enemies
| Release | New Enemies | Threat |
|---------|-------------|--------|
| Launch | 9 types (Goblin through Wave Boss) | Core roster |
| Update 1 | +Mimic (disguised as chest), +Necromancer | Exploration + support |
| DLC 1 | +Lava Golem, +Fire Imp | Map-specific |
| DLC 2 | +Cloud Rider, +Wind Spirit | Flying specialists |
| DLC 3 | +Sea Serpent, +Coral Crab | Underwater themed |

### 21.5 Items & Equipment
| Release | New Items | Source |
|---------|-----------|--------|
| Launch | Tier 1-4 full sets (4 weapon types, 5 armor slots) | Core crafting |
| Each Update | +2-3 unique weapons/armor from new bosses/maps | Boss drops + exploration |
| Seasonal | +Themed equipment (cosmetic variants with unique passives) | Battle pass + events |

### 21.6 Building Structures
| Release | New Pieces | Purpose |
|---------|-----------|---------|
| Launch | Walls, floors, ramps, roofs, gates, crenellations | Core defense building |
| Update 1 | +Drawbridge, +Moat (slows enemies) | Advanced defense |
| DLC per map | +Map-themed building sets (lava brick, cloud platform, coral wall) | Visual variety + map mechanics |
| Community | +Decorative packs (furniture, banners, statues) | Cosmetic expression |

---

## 22. TECHNICAL ARCHITECTURE

### 22.1 Networking
- [ ] Client-server model (dedicated or player-hosted)
- [ ] Photon Fusion 2 or Mirror for Unity networking
- [ ] Hip-only ragdoll sync
- [ ] Server-authoritative for: damage, loot, enhancement results, Core HP
- [ ] Client-side for: ragdoll visuals, particle effects, UI
- [ ] Tower targeting calculated server-side

### 22.2 Performance Targets
- [ ] 60 FPS on mid-range PC (RTX 3060 / RX 6700 XT equivalent)
- [ ] 30 FPS minimum on low-end (GTX 1060 / RX 580)
- [ ] Max active ragdoll bodies: 32
- [ ] Max active enemies: 40
- [ ] Physics solver iterations: 12
- [ ] Object pooling for all spawned entities
- [ ] Tower projectile pooling

### 22.3 World Design
- [ ] Pre-designed map layouts with procedural POI/resource placement
- [ ] Grid-based building zones around Core
- [ ] Chunk-based loading for large maps
- [ ] Persistent session saves (resume games)
- [ ] Map seed system for procedural elements (fragment locations, POI placement)

---

## 23. DEVELOPMENT PHASES

### Phase 1: Vertical Slice (3 months)
- [ ] **EXTENSIBILITY FOUNDATION FIRST:**
  - [ ] Content Registry system (auto-discover SOs from asset folders)
  - [ ] Event Bus / Message System (decoupled game events)
  - [ ] ScriptableObject templates for: Enemy, Tower, Item, Spell, BuildingPiece
  - [ ] Interface contracts: IEnemyBehavior, ITargetingStrategy, ISpellEffect
  - [ ] Tag system for content categorization
- [ ] Active ragdoll character controller
- [ ] Basic melee + ranged + spell combat
- [ ] 4-player co-op networking (hip sync)
- [ ] Core Base with HP system
- [ ] Prepare/Defend phase cycle
- [ ] Tier 1 gathering + basic crafting
- [ ] Basic building (walls, floors, foundations)
- [ ] 2 tower types (Arrow, Cannon) — built using TowerDefinition SO pipeline
- [ ] 3 enemy types (Goblin, Archer, Brute) — built using EnemyDefinition SO pipeline
- [ ] 3-wave defense cycle
- [ ] **VALIDATION: Can a new enemy/tower be added by ONLY creating a new SO? If not, fix architecture.**
- [ ] **PLAYTEST: Is the core loop fun with 4 players?**

### Phase 2: Core Systems (3 months)
- [ ] ClassDefinition SO + SkillTreeDefinition SO pipeline
- [ ] MapDefinition SO pipeline (map as self-contained package)
- [ ] ComedyFeature plugin interface + Event Bus hooks
- [ ] RPG stats + leveling system
- [ ] Full crafting chain (4 station tiers)
- [ ] 4 tower types (add Magic Bolt, Frost)
- [ ] Enhancement system (+0 to +15)
- [ ] Pity/failstack system
- [ ] Map exploration with 5+ POIs
- [ ] Boss fragment collection
- [ ] 1 boss fight (Mushroom Golem) — built using BossDefinition SO pipeline
- [ ] 5 enemy types
- [ ] Equipment system (all slots)
- [ ] Trap system (3 trap types)
- [ ] 1 comedy feature proof-of-concept (e.g., confetti on kill)

### Phase 3: Content Expansion (3 months)
- [ ] All 8 tower types
- [ ] 5 boss fights complete
- [ ] Full enhancement (+0 to +25)
- [ ] All crafting tiers
- [ ] Skill trees (4 paths)
- [ ] Full spell system (6+ spells)
- [ ] Enemy spawner node system
- [ ] All 9 enemy types
- [ ] Complete map with all zones/POIs
- [ ] 25-wave campaign progression

### Phase 4: Polish & Monetization (2 months)
- [ ] Cosmetic shop integration
- [ ] Battle pass system
- [ ] UI/UX polish (Core HP, wave preview, enhancement UI)
- [ ] Tutorial / new player experience
- [ ] Performance optimization
- [ ] Anti-cheat for enhancement system
- [ ] Localization
- [ ] QA ragdoll edge cases (3-5x normal QA time)
- [ ] Balancing pass (tower damage, enemy HP, enhancement rates)

### Phase 5: Early Access Launch
- [ ] Steam Early Access
- [ ] Community feedback integration
- [ ] Bug fixing sprint
- [ ] First seasonal content
- [ ] Endless Mode implementation

### Phase 6: Full Release + Live Service
- [ ] Full launch ($19.99 - $29.99)
- [ ] Season 1 Battle Pass
- [ ] Endless Mode + Leaderboards
- [ ] DLC planning (new maps, towers, bosses)
- [ ] Community tools (mod support consideration)

---

## 24. RISK REGISTER

| Risk | Probability | Impact | Mitigation |
|------|------------|--------|------------|
| Ragdoll networking desync | High | High | Hip-only sync, extensive QA, fallback to animation |
| Tower balance issues | High | Medium | Data-driven balancing, community feedback during Early Access |
| Enhancement backlash | Medium | High | Transparent pity system, protection items obtainable in-game |
| Performance (40 enemies + towers + ragdoll) | High | High | Strict ragdoll budget, animation LOD, object pooling, tower LOD |
| Core defense too easy/hard | Medium | Medium | Difficulty settings, wave scaling tuning, playtest extensively |
| Content exhaustion after final boss | High | High | Endless Mode, seasonal content, DLC maps |
| EU regulation changes | Medium | High | Cosmetic-first monetization, enhancement convenience NOT exclusive |
| Scope creep | High | High | Vertical slice first, cut features that don't serve core loop |

---

## 25. KEY DESIGN DECISIONS LOG

| Decision | Choice | Rationale | Date |
|----------|--------|-----------|------|
| Core game loop | Prepare → Defend → Repeat | Dungeon Defenders proven model + clear phase structure | 2026-03-12 |
| Win/Lose condition | Core destroyed = lose, final boss = win | Clear stakes, every wave matters | 2026-03-12 |
| Tower placement | Must be on player-built structures | Forces building engagement, creates strategic depth | 2026-03-12 |
| Combat style | Hybrid tower + action (DD-style) | Players fight alongside towers, not passive builder | 2026-03-12 |
| Boss access | Fragment collection from exploration | Forces exploration between waves, not just turtle | 2026-03-12 |
| Tone | Comedy-action | Ragdoll is inherently comedic; lean into it | 2026-03-12 |
| Enhancement | Punitive with pity (gear + towers) | High engagement + transparent fairness = retention | 2026-03-12 |
| Loot | Individual per player | Prevents co-op friction | 2026-03-12 |
| Wave system | Point-cost procedural | Static waves get boring; procedural = replayable | 2026-03-12 |
| Corrupted nodes | Destroyable to reduce wave difficulty | Links exploration to defense, rewards proactive play | 2026-03-12 |
| Architecture | Data-driven, modular, plugin-based | Game will grow indefinitely; new content must be addable without touching core code | 2026-03-12 |
| Comedy features | Event Bus plugin system | Braindead features are first-class citizens; must plug in without breaking gameplay | 2026-03-12 |
| Content format | ScriptableObjects + interface contracts | Adding enemy/tower/class/item = create SO + drop in folder, auto-registered | 2026-03-12 |
| Classes | Data-driven, not hardcoded | Future classes (Necromancer, Jester, etc.) must be addable as SO packs | 2026-03-12 |
| Maps | Self-contained scene + MapDefinition SO | DLC maps are independent packages with their own enemies/bosses/resources | 2026-03-12 |

---

## NOTES FOR FUTURE DISCUSSIONS
> This document is designed to be broken into smaller tasks using `/apex` task deconstruction.
> Each section (2-25) can be treated as a major epic.
> Each checkbox `[ ]` is a potential individual task.
> Priority order: Ragdoll (2) → Game Phases (3) → Core Base (4) → Towers (5) → Gathering (6) → Building (7) → Map (8) → Combat (9) → RPG (10) → Crafting (11) → Enhancement (12) → Waves (13) → Spawners (14) → Bosses (15) → Rewards (16) → Extensibility (19) → Comedy Framework (20)
>
> **EXTENSIBILITY RULE:** Before implementing ANY system, ask: "Can a new X be added by just creating a ScriptableObject?"
> If the answer is no, the architecture needs rework before proceeding.
