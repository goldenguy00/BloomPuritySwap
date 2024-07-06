using BepInEx;
using HarmonyLib;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;

namespace ScoresPhilsBenthicPuritySwap
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency("com.Borbo.GreenAlienHead", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class BenthicPuritySwap : BaseUnityPlugin
    {
        public const string PluginGUID    = "com." + PluginAuthor + "." + PluginName;
        public const string PluginAuthor  = "score";
        public const string PluginName    = "ScoresPhilsBenthicPuritySwap";
        public const string PluginVersion = "1.0.0";

        public static PluginInfo PInfo { get; private set; }

        public static bool abortPatching = false;

        public void Awake()
        {
            PInfo = Info;
            Log.Init(Logger);
            Assets.Init();

            var harm = new Harmony(PluginGUID);
            harm.CreateClassProcessor(typeof(SetItemDefsFix)).Patch();
            harm.CreateClassProcessor(typeof(SetItemRelationshipsFix)).Patch();

            IL.RoR2.CharacterMaster.OnInventoryChanged += (il) =>
            {
                var c = new ILCursor(il);
                //18	0036	ldsfld	class RoR2.ItemDef RoR2.RoR2Content/Items::LunarBadLuck
                if (c.TryGotoNext(MoveType.Before,
                    i => i.MatchLdsfld("RoR2.RoR2Content/Items", "LunarBadLuck")
                    ))
                {
                    c.Remove();
                    c.Emit(OpCodes.Ldsfld, typeof(DLC1Content.Items).GetFieldCached(nameof(DLC1Content.Items.CloverVoid)));
                }
                else
                {
                    Log.Error("IL Hook failed for CharacterMaster_OnInventoryChanged. Unable to swap bad luck from Purity to Benthic Bloom.");
                }
            };
        }
    }
}
