using System.Reflection;
using UnityEngine;

namespace ScoresPhilsBenthicPuritySwap
{
    internal class Assets
    {
        public static AssetBundle MainAssets { get; private set; }
        public const string bundleName = "ScoresPhilsBenthicPuritySwap.benthicpurityswap";

        public static void Init()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(bundleName);
            if (stream != null)
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
            }
            else
            {
                Log.Error("Critical error encountered when loading asset bundle. aborting procedures.");
                BenthicPuritySwap.abortPatching = true;
            }
        }
    }
}
