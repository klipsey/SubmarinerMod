﻿using RoR2;
using UnityEngine;
using SubmarinerMod.SubmarinerCharacter;
using SubmarinerMod.SubmarinerCharacter.Achievements;

namespace SubmarinerMod.SubmarinerCharacter.Content
{
    public static class SubmarinerUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;

        public static void Init()
        {
            /*
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockableDef(SpyMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(SpyMasteryAchievement.unlockableIdentifier),
                SubmarinerSurvivor.instance.assetBundle.LoadAsset<Sprite>("texMonsoonSkin"));
            */
            /*
            if (true == false)
            {
                characterUnlockableDef = Modules.Content.CreateAndAddUnlockableDef(SpyUnlockAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(SpyUnlockAchievement.unlockableIdentifier),
                SpySurvivor.instance.assetBundle.LoadAsset<Sprite>("texSpyIcon"));
            }
            */
        }
    }
}
