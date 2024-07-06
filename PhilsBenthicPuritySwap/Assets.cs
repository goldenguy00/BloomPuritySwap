using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ScoresPhilsBenthicPuritySwap
{
    internal class Assets
    {
        public static AssetBundle MainAssets { get; private set; }
        public const string bundleName = "benthicpurityswap";

        public static string AssetBundlePath
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(BenthicPuritySwap.PInfo.Location), bundleName);
            }
        }

        public static void Init()
        {
            MainAssets = AssetBundle.LoadFromFile(AssetBundlePath);
        }
    }
}
