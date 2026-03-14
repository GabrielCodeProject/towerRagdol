# Epic E17: Monetization
**Layer:** 8
**Design Doc Sections:** 17.1–17.5
**Depends On:** E16 (Reward System — season pass, currency), E12 (Enhancement — protection items)

---

## E17-T01: Base game purchase and content gating
- **Size:** S (1-2h)
- **Depends on:** None
- **Description:** Define base game scope: premium purchase $19.99–$29.99. ALL gameplay content included (no content paywalls). Verify no progression-affecting items are cash-shop exclusive. Create `MonetizationConfig` SO defining what's purchasable vs earnable. Audit checklist for each monetizable item.
- **Test:** Review all systems — verify no gameplay advantage is purchase-only. Every enhancement protection item also obtainable in-game.
- **Design doc ref:** Section 17.1

## E17-T02: Cosmetic shop system
- **Size:** L (3-4h)
- **Depends on:** E19-T01, E16-T06
- **Description:** Implement cosmetic shop: browse/purchase UI with categories. **Character skins** (ragdoll outfits, funny costumes), **Weapon skins** (visual only), **Tower skins** (visual reskins), **Building style packs** (medieval, sci-fi, fantasy), **Emotes and ragdoll poses**, **Death animations** (custom ragdoll deaths), **Core Base skins**. Each cosmetic = `CosmeticDefinition` SO. Purchase with premium currency or Gold. Owned cosmetics applied via equipment system.
- **Test:** Open shop — cosmetics displayed by category. Purchase skin — currency deducted, skin available. Apply skin — visual changes on character. No stat change.
- **Design doc ref:** Section 17.2

## E17-T03: Premium currency system
- **Size:** M (2h)
- **Depends on:** E16-T06
- **Description:** Implement premium currency ("Gems" or similar): purchased with real money via Steam IAP. Used for cosmetic shop purchases and battle pass. Currency bundles with bonus amounts at higher tiers. Currency balance UI. Transaction history. Refund-safe purchase flow. No gameplay items purchasable with premium currency only.
- **Test:** Purchase premium currency bundle — balance increases. Buy cosmetic — balance decreases. Verify no gameplay items in premium-only category.
- **Design doc ref:** Section 17.2

## E17-T04: Battle Pass integration
- **Size:** M (2h)
- **Depends on:** E16-T08, E17-T03
- **Description:** Integrate battle pass with monetization: free track available to all. Premium track ($9.99 or premium currency) unlocks exclusive cosmetics, building styles, emotes. No gameplay advantages in premium track. Season duration: 60 days. Premium purchase UI. Season countdown timer. Verify premium rewards are cosmetic-only.
- **Test:** Free player — access free track rewards. Purchase premium — premium track unlocks. Verify zero stat/gameplay items in premium track.
- **Design doc ref:** Section 17.3

## E17-T05: Enhancement convenience items (regulation-compliant)
- **Size:** M (2-3h)
- **Depends on:** E12-T04, E17-T03
- **Description:** Implement cash shop enhancement convenience: **Premium Protection Stones** (prevent regression AND breakage). MUST also be obtainable through gameplay (weekly quest reward + boss drops). Enhancement material bundles (saves time, not power). All probabilities clearly displayed in-game. Real-money cost equivalent shown for all purchases. Cash shop version = CONVENIENCE, not EXCLUSIVE.
- **Test:** Buy Protection Stone from shop — available. Earn same stone from weekly quest — identical item. Verify probability display. Verify real-money equivalent shown.
- **Design doc ref:** Section 17.4

## E17-T06: Parental controls
- **Size:** M (2h)
- **Depends on:** E17-T03
- **Description:** Implement parental controls for under-16 accounts: spending limits per month, purchase confirmation requirements, ability to disable cash shop access entirely. Age verification at account creation. Controls configurable by parent/guardian. Log all transactions for parental review. Comply with platform regulations.
- **Test:** Under-16 account — spending limit enforced. Exceed limit — purchase blocked. Parental disable — shop inaccessible. Transaction log visible.
- **Design doc ref:** Section 17.4

## E17-T07: DLC expansion framework
- **Size:** M (2-3h)
- **Depends on:** E19-T08, E17-T02
- **Description:** Implement DLC expansion support: new map layouts as self-contained packages (scene + MapDefinition SO). DLC can introduce unique enemies, towers, spell schools, biomes, bosses. DLC content auto-registers via Content Registry when installed. Co-op compatible: non-DLC players can JOIN DLC owner's session (see content, just can't host). DLC detection and content gating. Planned: 2-3 DLCs per year at $9.99 each.
- **Test:** Install DLC — new map appears in map selection, new content registers. Non-DLC player joins DLC host — can play. Non-DLC player can't host DLC map.
- **Design doc ref:** Section 17.5

---

## Summary
| Task | Size | Depends On |
|------|------|-----------|
| E17-T01 | S | None |
| E17-T02 | L | E19-T01, E16-T06 |
| E17-T03 | M | E16-T06 |
| E17-T04 | M | E16-T08, E17-T03 |
| E17-T05 | M | E12-T04, E17-T03 |
| E17-T06 | M | E17-T03 |
| E17-T07 | M | E19-T08, E17-T02 |

**Total: 7 tasks, ~16-20h**
