using RoR2;
using SubmarinerMod.Modules.Achievements;
using SubmarinerMod.SubmarinerCharacter;

namespace SubmarinerMod.SubmarinerCharacter.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 0)]
    public class SubmarinerMasterAchievement : BaseMasteryAchievement
    {
        public const string identifier = SubmarinerSurvivor.SUBMARINER_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = SubmarinerSurvivor.SUBMARINER_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => SubmarinerSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}