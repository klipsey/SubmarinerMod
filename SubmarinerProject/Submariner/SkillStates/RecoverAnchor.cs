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
    public class RecoverAnchor : BaseSubmarinerSkillState
    {
        public static GameObject muzzleEffectPrefab;

        public static float baseDuration = 0.6f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayAnimation("FullBody, Override", "ThrowAnchor", "Slash.playbackRate", duration);
            submarinerController.DisableAnchor();
        }

        public override void OnExit()
        {
            base.OnExit();
            submarinerController.EnableAnchor();
            PlayCrossfade("Stance, Override", "Empty", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
