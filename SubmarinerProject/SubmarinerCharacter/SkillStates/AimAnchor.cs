using SubmarinerMod.Modules.BaseStates;
using UnityEngine;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using SubmarinerMod.SubmarinerCharacter.Content;
using SubmarinerMod.SubmarinerCharacter.Components;
using static UnityEngine.UI.Image;

namespace SubmarinerMod.SubmarinerCharacter.SkillStates
{
    public class AimAnchor : AimThrowableBase
    {
        public static string enterSoundString;

        public static string exitSoundString;
        public override void OnEnter()
        {
            maxDistance = 50f;
            arcVisualizerPrefab = SubmarinerAssets.throwable;
            projectilePrefab = SubmarinerAssets.anchorPrefab;
            endpointVisualizerPrefab = SubmarinerAssets.throwableEnd;
            damageCoefficient = SubmarinerConfig.anchorDamageCoefficient.Value;
            baseMinimumDuration = 0.25f;
            enterSoundString = "sfx_driver_button_foley";
            exitSoundString = "sfx_scout_cleaver_throw";
            base.OnEnter();
            PlayAnimation("Gesture, Override", "ChargeAnchor");
            Util.PlaySound(enterSoundString, base.gameObject);
            detonationRadius = 7f;
        }
        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override", "ThrowAnchor", "Harpoon.playbackRate", 1f / attackSpeedStat);
            outer.SetNextState(new RecoverAnchor());
            Util.PlaySound(exitSoundString, base.gameObject);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
