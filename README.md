# Dress Up Colonist: New & Improved

A RimWorld mod that allows you to fully customize the visual appearance of your colonists' apparel, headwear, and body models without affecting their actual equipment stats, armor values, or performance.

Updated the 1.5 mod: https://steamcommunity.com/sharedfiles/filedetails/?id=3232918791 to 1.6 and 1.7

---

## Features

* **Cosmetic Apparel & Headgear Overrides:** Change how any piece of clothing, armor, or headgear looks on a colonist without modifying what they are actually wearing.
* **Ideology Style Selection:** Pick alternate visual styles (such as Techist, Morbid, etc.) for supported items with live icon previews and hover tooltips.
* **Body Type & Model Customization:** Adjust visual body types (`Male`, `Female`, `Thin`, `Hulk`, `Fat`), RGB color tints, width/height/thickness scaling, and directional layer sorting offsets (`North`, `South`, `East`, `West`).
* **Animal Model Overrides:** Replace a colonist's visual model with vanilla or modded animals, complete with a dedicated reset-to-default control.
* **Full Loadout Copy & Paste:** Clone an entire colonist's cosmetic setup—including all apparel items, styles, body types, colors, scales, and layer offsets—onto another colonist in one click using **Copy All** and **Paste All**.
* **Per-Item Copy, Paste & Reset:** Quickly duplicate or reset settings on individual apparel pieces, with built-in safeguards preventing headgear and chest apparel mismatches.

---

## What Changed (vs. Original Mod)

* **RimWorld 1.6 & 1.7 Support:** Updated Harmony patches, render tree handling, and `PawnKindDef` lookups to work cleanly with modern RimWorld versions.
* **Redesigned UI & Tooltips:** Replaced clipped text lists with clean, enlarged icon grids for body types and styles featuring hover tooltips.
* **Sanitized Default Spawning:** Newly added cosmetic overrides now spawn unstyled (`null` style) and use baseline primary materials (such as Cloth or Steel) by default instead of inheriting random ideological shapes or invalid materials.
* **Bug & Stability Fixes:**
  * Fixed static constructor, `HarmonyException`, and stack overflow crashes on startup.
  * Fixed `ThingMaker.MakeThing` null-stuff errors and apparel duplication issues when rendering animal models.
  * Filtered baby/child body types out of the adult body type selector and hid body type options for headgear to prevent model errors.
  * Cleaned up repository structure and removed outdated localization files.
* **Package ID & Compatibility:** Updated `packageId` to `Willow.DressUpColonistNew` and marked `Udon.DressUpColonist` as incompatible so both versions are not loaded simultaneously.

---

## How It Works

1. **Pawn Data Component (`ThingComp`):** Each colonist stores a lightweight list of cosmetic override entries saving the target `ThingDef`, `ThingStyleDef`, `BodyTypeDef`, RGB color values, scale multipliers, and directional layer offsets.
2. **Dynamic Render Tree Patching:** Using **Harmony**, the mod hooks into RimWorld's `PawnRenderTree` setup (`Patch_PawnRenderTree_SetupDynamicNodes`) to intercept apparel and body rendering.
3. **Visual-Only Generation:** Instead of modifying the pawn's inventory or `Pawn_ApparelTracker`, the mod generates temporary visual graphics using the stored override settings and injects them into the render tree, calling `SetAllGraphicsDirty()` whenever changes are made in the UI so updates appear immediately on the map.

---

## Requirement. 
harmony. but you already knew that. 
