using System;
using SubmarinerMod.Modules;
using SubmarinerMod.Submariner;
using SubmarinerMod.Submariner.Achievements;
using UnityEngine.UIElements;

namespace SubmarinerMod.Submariner.Content
{
    public static class SubmarinerTokens
    {
        public static void Init()
        {
            AddSubmarinerTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            //Language.PrintOutput("Spy.txt");
            //todo guide
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddSubmarinerTokens()
        {
            #region Submariner
            string prefix = SubmarinerSurvivor.SUBMARINER_PREFIX;

            string desc = "Submariner<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > ." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > ." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > ." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > ." + Environment.NewLine + Environment.NewLine;

            string lore = "Insert goodguy lore here";
            string outro = "..and so she left, Submarinering all over the place.";
            string outroFailure = "..and so she vanished, not Submarinering all over the place.";
            
            Language.Add(prefix + "NAME", "Submariner");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "");
            Language.Add(prefix + "LORE", lore);
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "N'kuhanas Blessing");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Hitting enemies with <style=cIsDamage>melee attacks</style> grants <style=cIsHealing>N'kuhanna's Restoration</style>.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_SWING_NAME", "AL811 Anchoring Device");
            Language.Add(prefix + "PRIMARY_SWING_DESCRIPTION", $"<style=cIsUtility>Stunning.</style> Swing in front dealing <style=cIsDamage>{SubmarinerStaticValues.swingDamageCoefficient * 100f}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_HARPOON_NAME", "Reel-In");
            Language.Add(prefix + "SECONDARY_HARPOON_DESCRIPTION", $"Fire out a harpoon dealing <style=cIsDamage>{SubmarinerStaticValues.harpoonDamageCoefficient / 2f * 100f}% damage</style>. " +
                $"<style=cIsUtility>Pulls</style> small enemies towards you and <style=cIsUtility>pulls</style> you towards large enemies. " +
                $"Colliding with a large enemy deals <style=cIsDamage>{SubmarinerStaticValues.harpoonDamageCoefficient * 100f}%</style> then starts a <style=cIsUtility>backflip.</style>");
            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_MINE_NAME", "Explosive Urchin");
            Language.Add(prefix + "UTILITY_MINE_DESCRIPTION", $"<style=cIsUtility>Backflip</style> firing out an <style=cIsHealing>Urchine Mine</style> that deals <style=cIsDamage>{SubmarinerStaticValues.mineDamageCoefficient * 100f}% damage</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_ANCHORTHROW_NAME", "Drop the Anchor!");
            Language.Add(prefix + "SPECIAL_ANCHORTHROW_DESCRIPTION", $"Throw your anchor, dealing <style=cIsDamage>{SubmarinerStaticValues.anchorDamageCoefficient * 100f}% damage</style> on impact. " +
                $"Running towards the anchor increases your <style=cIsUtility>movement speed</style>, while moving away from it <style=cIsDamage>slows</style> you down. " +
                $"Going too far away breaks its chain, giving a burst of <style=cIsUtility>movement speed</style>.");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(SubmarinerMasterAchievement.identifier), "Submariner: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(SubmarinerMasterAchievement.identifier), "As Submariner, beat the game or obliterate on Monsoon.");
            /*
            Language.Add(Tokens.GetAchievementNameToken(SpyUnlockAchievement.identifier), "Dressed to Kill");
            Language.Add(Tokens.GetAchievementDescriptionToken(SpyUnlockAchievement.identifier), "Get a Backstab.");
            */
            #endregion

            #endregion
        }
    }
}