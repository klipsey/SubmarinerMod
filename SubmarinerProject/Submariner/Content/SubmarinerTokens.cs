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

            string desc = "Submariner is close range combatant that regenerates health quickly as she fights more enemies. Positioning properly is important to managing her regeneration and movement speed bonuses.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Keep up the pressure with melee hits to continuously regenerate health." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use Reel-In to pull in smaller enemies and use it to avoid attacks by hitting larger enemies." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Urchin Mine is a great way to quickly escape a sticky situation and to setup high damaging mines for later." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Stay within Drop The Anchor!'s range, the movement speed buff can be an incredible tool to fight with." + Environment.NewLine + Environment.NewLine;

            string lore = "";
            string outro = "..and so she left, never to be left alone again.";
            string outroFailure = "..and so she vanished, with only silence by her side.";
            
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
            Language.Add(prefix + "PRIMARY_SWING_DESCRIPTION", $"Swing in front dealing <style=cIsDamage>{SubmarinerStaticValues.swingDamageCoefficient * 100f}% damage</style>.");
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

            Language.Add(prefix + "UTILITY_BEAST_NAME", "Cult's Best Friend");
            Language.Add(prefix + "UTILITY_BEAST_DESCRIPTION", $"<style=cIsDamage>Slayer.</style> Ride a <style=cIsHealing>Beast</style>, which quickly runs forward dealing <style=cIsDamage>{SubmarinerStaticValues.beastDamageCoefficient * 100f}% damage</style>. " +
                $"Killing a lightweight enemy <style=cIsUtility>speeds up</style> the <style=cIsHealing>Beast</style> and increases <style=cIsDamage>damage</style> dealt. Running into large enemies stops you and <style=cIsUtility>stuns</style> the enemy.");

            #endregion

            #region Special
            Language.Add(prefix + "SPECIAL_ANCHORTHROW_NAME", "Drop the Anchor!");
            Language.Add(prefix + "SPECIAL_ANCHORTHROW_DESCRIPTION", $"Throw your anchor, dealing <style=cIsDamage>{SubmarinerStaticValues.anchorDamageCoefficient * 100f}% damage</style> on impact. " +
                $"Running towards the anchor increases your <style=cIsUtility>movement speed</style>, while moving away from it <style=cIsDamage>slows</style> you down. " +
                $"Going too far away <style=cIsHealth>breaks</style> the anchors chain, granting a burst of <style=cIsUtility>movement speed</style>.");
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