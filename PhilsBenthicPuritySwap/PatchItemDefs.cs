using HarmonyLib;
using RoR2;
using RoR2.ExpansionManagement;
using System;
using System.Linq;
using UnityEngine;

namespace ScoresPhilsBenthicPuritySwap
{
    [HarmonyPatch(typeof(ItemCatalog), nameof(ItemCatalog.SetItemDefs))]
    public class SetItemDefsFix
    {

        [HarmonyPrefix]
        private static void Prefix(ref ItemDef[] newItemDefs)
        {
            int purityIdx = -1;
            int bloomIdx = -1;
            var purityTier = ItemTier.VoidTier3;

            for (int i = 0; i < newItemDefs.Length; i++)
            {
                var itemDef = newItemDefs[i];
                if (itemDef)
                {
                    if (itemDef.name == RoR2Content.Items.AlienHead.name)
                        purityTier = itemDef.tier == ItemTier.Tier2 ? ItemTier.VoidTier2 : purityTier;
                    else if (itemDef.name == RoR2Content.Items.LunarBadLuck.name)
                        purityIdx = i;
                    else if (itemDef.name == DLC1Content.Items.CloverVoid.name)
                        bloomIdx = i;
                }
            }

            if (purityIdx == -1)
            {
                Log.Error($"Benthic purity swap ran into a critical error! Purity index {purityIdx} is invalid! Aborting procedures.");
                BenthicPuritySwap.abortPatching = true;
                return;
            }

            var purityDef = newItemDefs[purityIdx];
            purityDef.tier = purityTier;
            purityDef.pickupIconSprite = Assets.MainAssets.LoadAsset<Sprite>("assets/import/benthicpurityswap_icons/voidbadluck.png");
            purityDef.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(ex => ex.name.Contains("DLC1"));
            Log.Info("expansion" + purityDef.requiredExpansion?.name);

            newItemDefs[purityIdx] = purityDef;
            RoR2Content.Items.LunarBadLuck = purityDef;

            if (bloomIdx == -1)
            {
                Log.Error("Benthic bloom could not be found in the item catalog. Unintended behaviour may occur...");
                return;
            }

            var bloomDef = newItemDefs[bloomIdx];
            bloomDef.tier = ItemTier.Lunar;
            bloomDef.pickupIconSprite = Assets.MainAssets.LoadAsset<Sprite>("assets/import/benthicpurityswap_icons/cloverLunar.png");

            newItemDefs[bloomIdx] = bloomDef;
            DLC1Content.Items.CloverVoid = bloomDef;
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

            var newPair = new ItemDef.Pair
            {
                itemDef1 = RoR2Content.Items.AlienHead,
                itemDef2 = RoR2Content.Items.LunarBadLuck
            };

            bool removed = false;
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
                            newProviders[i].relationships[j] = newPair;
                            removed = true;
                            break;
                        }
                    }

                    if (!removed)
                    {
                        Log.Error("Unable to find original void pair.");
                        Array.Resize(ref itemPairs, itemPairs.Length + 1);
                        itemPairs[itemPairs.Length - 1] = newPair;
                        newProviders[i].relationships = itemPairs;
                    }

                    break;
                }
            }
        }
    }
}
