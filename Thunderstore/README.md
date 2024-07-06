# Score's Phils Benthic Purity Swap

Highly inspired by [PhilsBenthicPuritySwap](https://thunderstore.io/package/BoaphilMods/PhilsBenthicPuritySwap/) but rewritten from the ground up since I wasn't satisfied with how PhilsBenthicPuritySwap implemented this swap.

### Changes
- Removed VoidAPI dependency.
- No longer creates new ItemDefs and instead modifies the vanilla items directly for compatibility reasons.
- Works with any mod that references the vanilla items from RoR2Content.Items/DLC1Content.Items or from the ItemCatalog.
- I'm still using the sprite and language assets from Phils mod, but that's the only thing that's left from the original.

### Benthic Bloom
- Changes Tier to Lunar
- Upgrades items as normal
- Reduces luck by 1 per Bloom
- No longer Corrupts 57 Leaf Clovers

### Purity
- Changes Tier to Void Red (or Void Green if GreenAlienHead is installed)
- Reduces cooldowns as normal
- No longer reduces luck
- Corrupts all Alien Heads