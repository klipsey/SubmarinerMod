using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SubmarinerMod.Submariner.Content
{
    public static class SubmarinerBuffs
    {
        public static BuffDef SubmarinerRegenBuff;
        public static void Init(AssetBundle assetBundle)
        {
            SubmarinerRegenBuff = Modules.Content.CreateAndAddBuff("SubmarinerRegenBuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Croco/texBuffRegenBoostIcon.tif").WaitForCompletion(),
                SubmarinerAssets.SubmarinerColor, true, false, false);
        }
    }
}
