using HarmonyLib;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace ScoresPhilsBenthicPuritySwap
{
    [HarmonyPatch(typeof(ItemCatalog), nameof(ItemCatalog.SetItemDefs))]
    public class SetItemDefsFix
    {
        [HarmonyPrefix]
        private static void Prefix(ref ItemDef[] newItemDefs)
        {
            int headIdx = -1;
            int purityIdx = -1;
            int bloomIdx = -1;
            for (int i = 0; i < newItemDefs.Length; i++)
            {
                var itemDef = newItemDefs[i];

                if (itemDef.name == RoR2Content.Items.AlienHead.name)
                    headIdx = i;
                else if (itemDef.name == RoR2Content.Items.LunarBadLuck.name)
                    purityIdx = i;
                else if (itemDef.name == DLC1Content.Items.CloverVoid.name)
                    bloomIdx = i;
            }
            if (bloomIdx == -1 || purityIdx == -1)
            {
                Log.Error($"Benthic purity swap ran into a critical error! Bloom index {bloomIdx} or purity index {purityIdx} are invalid! Aborting procedures.");
                BenthicPuritySwap.abortPatching = true;
                return;
            }
            if (headIdx == -1)
            {
                Log.Error("AlienHead item could not be found in the item catalog. Unintended behaviour may occur...");
            }

            var bloomDef = newItemDefs[bloomIdx];
            bloomDef.tier = ItemTier.Lunar;
            var upperName = bloomDef.name.ToUpper();
            bloomDef.nameToken = $"PHILSBENTHICPURITYSWAP_{upperName}_NAME";
            bloomDef.pickupToken = $"PHILSBENTHICPURITYSWAP_{upperName}_PICKUP";
            bloomDef.descriptionToken = $"PHILSBENTHICPURITYSWAP_{upperName}_DESC";
            bloomDef.loreToken = $"PHILSBENTHICPURITYSWAP_{upperName}_LORE";
            bloomDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/CloverVoid/PickupCloverVoid.prefab").WaitForCompletion();
            bloomDef.pickupIconSprite = Assets.MainAssets.LoadAsset<Sprite>("assets/import/benthicpurityswap_icons/cloverLunar.png");
            newItemDefs[bloomIdx] = bloomDef;
            DLC1Content.Items.CloverVoid = bloomDef;

            var purityDef = newItemDefs[purityIdx];
            if (headIdx != -1 && newItemDefs[headIdx].tier == ItemTier.Tier2)
                purityDef.tier = ItemTier.VoidTier2;
            else
                purityDef.tier = ItemTier.VoidTier3;
            upperName = purityDef.name.ToUpper();
            purityDef.nameToken = $"PHILSBENTHICPURITYSWAP_{upperName}_NAME";
            purityDef.pickupToken = $"PHILSBENTHICPURITYSWAP_{upperName}_PICKUP";
            purityDef.descriptionToken = $"PHILSBENTHICPURITYSWAP_{upperName}_DESC";
            purityDef.loreToken = $"PHILSBENTHICPURITYSWAP_{upperName}_LORE";
            purityDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarBadLuck/PickupStarSeed.prefab").WaitForCompletion();
            purityDef.pickupIconSprite = Assets.MainAssets.LoadAsset<Sprite>("assets/import/benthicpurityswap_icons/voidbadluck.png");
            purityDef.requiredExpansion = bloomDef.requiredExpansion;
            newItemDefs[purityIdx] = purityDef;
            RoR2Content.Items.LunarBadLuck = purityDef;
        }
    }

    [HarmonyPatch(typeof(ItemCatalog), nameof(ItemCatalog.SetItemRelationships))]
    public class SetItemRelationshipsFix
    {
        [HarmonyPrefix]
        private static void Prefix(ref ItemRelationshipProvider[] newProviders)
        {
            if (BenthicPuritySwap.abortPatching)
                return;

            for (int i = 0; i < newProviders.Length; i++)
            {
                if (newProviders[i].relationshipType == DLC1Content.ItemRelationshipTypes.ContagiousItem)
                {
                    var itemPairs = newProviders[i].relationships;
                    for (int j = 0; j < itemPairs.Length; j++)
                    {
                        var pair = itemPairs[j];
                        if (pair.itemDef1 && pair.itemDef2 && pair.itemDef1.name == RoR2Content.Items.Clover.name && pair.itemDef2.name == DLC1Content.Items.CloverVoid.name)
                        {
                            newProviders[i].relationships[j] = new ItemDef.Pair
                            {
                                itemDef1 = RoR2Content.Items.AlienHead,
                                itemDef2 = RoR2Content.Items.LunarBadLuck
                            };
                        }
                    }
                }
            }
        }
    }
}
