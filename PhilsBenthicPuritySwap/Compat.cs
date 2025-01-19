using RoR2;
using System.Linq;
using System.Runtime.CompilerServices;
using LG = LookingGlass.ItemStatsNameSpace;

namespace ScoresPhilsBenthicPuritySwap
{
    public static class Compat
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void LGCompat()
        {
            if (BenthicPuritySwap.abortPatching || LG.ItemDefinitions.allItemDefinitions?.Any() != true)
            {
                Log.Error("Cant do compatibility stuff with LookingGlass");
                return;
            }

            if (LG.ItemDefinitions.allItemDefinitions.TryGetValue((int)RoR2Content.Items.LunarBadLuck.itemIndex, out var purityDef))
            {
                purityDef.descriptions.Remove("Luck: ");
                purityDef.valueTypes.Remove(LG.ItemStatsDef.ValueType.Death);
                purityDef.measurementUnits.Remove(LG.ItemStatsDef.MeasurementUnits.Number);
                purityDef.calculateValuesNew = (luck, stackCount, procChance) => [stackCount + 1];
            }
            else
                Log.Error("Purity index didnt match lookingglass stats");

            if (LG.ItemDefinitions.allItemDefinitions.TryGetValue((int)DLC1Content.Items.CloverVoid.itemIndex, out var bloomDef))
            {
                bloomDef.descriptions.Add("Luck: ");
                bloomDef.valueTypes.Add(LG.ItemStatsDef.ValueType.Death);
                bloomDef.measurementUnits.Add(LG.ItemStatsDef.MeasurementUnits.Number);
                bloomDef.calculateValuesNew = (luck, stackCount, procChance) => [stackCount * 3, -stackCount];
            }
            else
                Log.Error("Bloom index didnt match lookingglass stats");
        }
    }
}
