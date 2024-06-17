using SubmarinerMod.Modules.BaseStates;
using UnityEngine;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using SubmarinerMod.Submariner.Content;
using SubmarinerMod.Submariner.Components;
using static UnityEngine.UI.Image;

namespace SubmarinerMod.Submariner.SkillStates
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
            damageCoefficient = SubmarinerStaticValues.anchorDamageCoefficient;
            baseMinimumDuration = 0.25f;
            enterSoundString = "sfx_driver_button_foley";
            enterSoundString = "sfx_scout_cleaver_throw";
            base.OnEnter();
            Util.PlaySound(enterSoundString, base.gameObject);
            detonationRadius = 7f;
        }
        public override void OnExit()
        {
            base.OnExit();
            outer.SetNextState(new RecoverAnchor());
            Util.PlaySound(exitSoundString, base.gameObject);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
