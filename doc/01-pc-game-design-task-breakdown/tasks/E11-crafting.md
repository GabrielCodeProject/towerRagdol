# Epic E11: Crafting System
**Layer:** 5
**Design Doc Sections:** 11.1–11.3
**Depends On:** E06 (Resources/Inventory), E10 (RPG — Intelligence stat affects quality)

---

## E11-T01: Crafting station entities (4 tiers)
- **Size:** M (2-3h)
- **Depends on:** E07-T02
- **Description:** Create 4 crafting station types as buildable objects: **Workbench (Tier 1)** — basic tools, weapons, armor, Arrow/Cannon towers. **Forge (Tier 2)** — metal weapons, medium armor, Magic/Frost towers. **Arcane Table (Tier 3)** — enchanted gear, spell scrolls, Fire/Lightning/Ballista towers. **Celestial Anvil (Tier 4)** — endgame gear, Void towers, legendary recipes. Each station = BuildingPiece SO + interaction trigger.
- **Test:** Place each station — interact opens crafting UI. Workbench shows Tier 1 recipes only. Forge shows Tier 2. Verify tier gating.
- **Design doc ref:** Section 11.1

## E11-T02: Recipe system
- **Size:** L (3-4h)
- **Depends on:** E11-T01, E19-T01
- **Description:** Implement recipe system using `RecipeDefinition` SOs: input materials (item + quantity list), output item reference, required station tier, unlock condition (default unlocked, discovered via exploration, boss drop, quest). Component crafting chain: raw → processed → component → final item. Recipes auto-register in RecipeRegistry. Crafting UI shows available recipes filtered by station tier and discovered status.
- **Test:** Open Workbench — see available Tier 1 recipes. Select recipe — shows required materials. Craft — materials consumed, item produced. Undiscovered recipes hidden.
- **Design doc ref:** Section 11.2

## E11-T03: Crafting quality system
- **Size:** M (2h)
- **Depends on:** E11-T02, E10-T01
- **Description:** Implement crafting quality tiers: Normal → Fine → Superior → Masterwork. Quality determined by Intelligence stat (higher Intelligence = higher chance of better quality). Quality affects output item stats (bonus %). Quality probability curve from BalanceProfile SO. UI shows quality chance before crafting. Higher quality items have visual indicator (name color, border).
- **Test:** Craft with low Intelligence — mostly Normal quality. Boost Intelligence — Fine/Superior appear. Verify stat bonuses per quality tier.
- **Design doc ref:** Section 11.2

## E11-T04: Recipe discovery system
- **Size:** M (2h)
- **Depends on:** E11-T02
- **Description:** Implement recipe discovery: recipes found through exploration POIs (ancient chests), boss drops, quest rewards. Discovered recipes persist in player save. New discovery notification UI. Recipe book UI showing all discovered recipes (and hints for undiscovered). Discovery events published via Event Bus.
- **Test:** Find recipe scroll in chest — recipe unlocked, notification appears. Recipe now available at correct station. Recipe book shows discovered status.
- **Design doc ref:** Section 11.2

## E11-T05: Tower-specific crafting (Tower Workbench)
- **Size:** M (2h)
- **Depends on:** E11-T02, E05-T05
- **Description:** Implement dedicated Tower Workbench for tower crafting. Tower recipes require resources + Tower Core item (dropped by enemies/exploration). Tower Workbench = separate station from gear crafting stations. Tower recipes separated in RecipeRegistry by tag. Tower Workbench available at all tiers but gates recipe visibility by player-unlocked tiers.
- **Test:** Open Tower Workbench — shows tower recipes. Craft Arrow Tower — requires Wood + Stone + Tower Core. Tower Core consumed. Tower appears in inventory as placeable.
- **Design doc ref:** Section 5.3, 11.1

## E11-T06: Resource sink balancing
- **Size:** M (2h)
- **Depends on:** E11-T02
- **Description:** Implement resource sinks: Tier 1 materials always needed for building upkeep, tower repairs, trap refills. Repair costs defined per building piece SO. Enhancement consumes materials on both success and failure. Tower crafting + upgrading as constant resource demand. Trap refueling between waves. All costs from BalanceProfile SO for tuning.
- **Test:** Repair damaged wall — materials consumed. Refuel traps — materials consumed. Verify Tier 1 resources stay valuable at endgame.
- **Design doc ref:** Section 11.3

## E11-T07: Crafting UI
- **Size:** L (3-4h)
- **Depends on:** E11-T02
- **Description:** Full crafting UI: recipe list (filterable by category, tier, discovered), selected recipe detail (input materials with have/need counts, output preview with quality chance), craft button (grayed if materials insufficient), batch crafting (craft X), progress bar for crafting time. Station-specific UI skin. Keyboard shortcuts for quick crafting.
- **Test:** Open crafting station — recipe list loads. Select recipe — details show. Materials insufficient — craft button disabled. Craft — progress bar, item produced.
- **Design doc ref:** Section 11.1, 11.2

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E11-T01 | M | E07-T02 |
| E11-T02 | L | E11-T01, E19-T01 |
| E11-T03 | M | E11-T02, E10-T01 |
| E11-T04 | M | E11-T02 |
| E11-T05 | M | E11-T02, E05-T05 |
| E11-T06 | M | E11-T02 |
| E11-T07 | L | E11-T02 |

**Total: 7 tasks, ~18-22h**
