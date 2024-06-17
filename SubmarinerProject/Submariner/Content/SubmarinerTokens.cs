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
            Language.Add(prefix + "PASSIVE_DESCRIPTION", $"Hitting enemies with <style=cIsDamage>melee attacks</style> grants <style=cIsHealing>N'kuhanas Regeneration</style>.");
            #endregion

            #region Primary
            Language.Add(prefix + "PRIMARY_SWING_NAME", "AL811 Anchoring Device");
            Language.Add(prefix + "PRIMARY_SWING_DESCRIPTION", $"<style=cIsUtility>Stunning.</style> Swing in front dealing <style=cIsDamage>{SubmarinerStaticValues.swingDamageCoefficient * 100f}% damage</style>.");
            #endregion

            #region Secondary
            Language.Add(prefix + "SECONDARY_HARPOON_NAME", "Harpoon");
            Language.Add(prefix + "SECONDARY_HARPOON_DESCRIPTION", $"Fire out a harpoon dealing <style=cIsDamage>{SubmarinerStaticValues.harpoonDamageCoefficient / 2f * 100f}% damage</style>. " +
                $"<style=cIsUtility>Pulls</style> small enemies towards you and <style=cIsUtility>pulls</style> you towards large enemies. " +
                $"Colliding with a large enemy deals <style=cIsDamage>{SubmarinerStaticValues.harpoonDamageCoefficient * 100f}%</style> then starts a <style=cIsUtility>backflip.</style>");
            #endregion

            #region Utility 
            Language.Add(prefix + "UTILITY_MINE_NAME", "Urchin Mine");
            Language.Add(prefix + "UTILITY_MINE_DESCRIPTION", $"<style=cIsUtility>Backflip</style> firing out an <style=cIsHealing>Urchine Mine</style> that deals <style=cIsDamage>{SubmarinerStaticValues.mineDamageCoefficient * 100f}% damage</style>.");
            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_ANCHORTHROW_NAME", "Anchor Drop");
            Language.Add(prefix + "SPECIAL_ANCHORTHROW_DESCRIPTION", $"Throw your anchor for <style=cIsDamage>{SubmarinerStaticValues.anchorDamageCoefficient * 100f}% damage</style>. Running towards the anchor grants <style=cIsUtility>movespeed</style> while " +
                $"running away <style=cIsDamage>slows</style> you down. <style=cIsDamage>Break</style> the chain attached to your anchor to gain a burst of <style=cIsUtility>movespeed</style>.");

            Language.Add(prefix + "SPECIAL_SCEPTER_ANCHORTHROW_NAME", "Punish");
            Language.Add(prefix + "SPECIAL_SCEPTER_ANCHORTHROW_DESCRIPTION", $"Target a <color=#FFBF66>Guilty</color> enemy and force them to fight you for 10 seconds. Your primary can no longer hit you but will continuously add <color=#FFBF66>Guilty's</color> buff to you. " +
                $"During this time all external <style=cIsDamage>damage</style> is negated but all your <style=cIsDamage>damage</style> dealt to others is <style=cIsUtility>negated</style>." + Tokens.ScepterDescription("Convict can target enemies without Guilty and damage you deal is no longer negated but is reduced by 75%."));
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