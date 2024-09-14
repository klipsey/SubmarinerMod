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
    public class RecoverAnchor : BaseSubmarinerSkillState
    {
        public static GameObject muzzleEffectPrefab;

        public static float baseDuration = 1f;

        private float duration;

        public override void OnEnter()
        {
            RefreshState();
            submarinerController.DisableAnchor();
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
        }

        public override void OnExit()
        {
            base.OnExit();
            submarinerController.EnableAnchor();
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
