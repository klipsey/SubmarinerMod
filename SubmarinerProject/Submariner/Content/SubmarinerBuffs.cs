using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace InterrogatorMod.Interrogator.Content
{
    public static class SubmarinerBuffs
    {
        public static BuffDef interrogatorGuiltyDebuff;
        public static BuffDef interrogatorGuiltyBuff;
        public static BuffDef interrogatorPressuredBuff;
        public static BuffDef interrogatorConvictBuff;
        public static void Init(AssetBundle assetBundle)
        {
            interrogatorGuiltyBuff = Modules.Content.CreateAndAddBuff("InterrogatorGuiltyBuff", assetBundle.LoadAsset<Sprite>("texGuiltyBuff"),
                SubmarinerAssets.interrogatorColor, true, false, false);

            interrogatorGuiltyDebuff = Modules.Content.CreateAndAddBuff("InterrogatorGuiltyDebuff", assetBundle.LoadAsset<Sprite>("texGuiltyDebuff"),
                SubmarinerAssets.interrogatorColor, false, true, false);

            interrogatorPressuredBuff = Modules.Content.CreateAndAddBuff("InterrogatorPressuredDebuff", Addressables.LoadAssetAsync<Sprite>("RoR2/Base/CritOnUse/texBuffFullCritIcon.tif").WaitForCompletion(),
                SubmarinerAssets.interrogatorColor, false, false, false);
            
            interrogatorConvictBuff = Modules.Content.CreateAndAddBuff("InterrogatorConvictBuff", assetBundle.LoadAsset<Sprite>("texConvictBuff"), 
                SubmarinerAssets.interrogatorColor, false, false, false);
        }
    }
}
