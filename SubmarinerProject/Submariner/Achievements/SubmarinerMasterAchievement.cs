using RoR2;
using InterrogatorMod.Modules.Achievements;
using InterrogatorMod.Interrogator;

namespace InterrogatorMod.Interrogator.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, null)]
    public class SubmarinerMasterAchievement : BaseMasteryAchievement
    {
        public const string identifier = SubmarinerSurvivor.INTERROGATOR_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = SubmarinerSurvivor.INTERROGATOR_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => SubmarinerSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}