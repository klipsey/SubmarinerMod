using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using InterrogatorMod.Interrogator.Content;
using InterrogatorMod.Interrogator.Components;
using R2API;

namespace InterrogatorMod.Interrogator.SkillStates
{
    public class ThrowCleaver : GenericProjectileBaseState
    {
        public static float baseDuration = 0.2f;
        public static float baseDelayDuration = 0.3f * baseDuration;
        public GameObject cleaver = SubmarinerAssets.cleaverPrefab;
        public SubmarinerController interrogatorController;
        private ChildLocator childLocator;
        public override void OnEnter()
        {
            interrogatorController = base.gameObject.GetComponent<SubmarinerController>();
            base.attackSoundString = "sfx_scout_baseball_hit";

            base.baseDuration = baseDuration;
            base.baseDelayBeforeFiringProjectile = baseDelayDuration;

            base.damageCoefficient = damageCoefficient;
            base.force = 120f;

            base.projectilePitchBonus = -3.5f;

            base.OnEnter();
        }

        public override void FireProjectile()
        {
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                aimRay = this.ModifyProjectileAimRay(aimRay);
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, 0f, this.projectilePitchBonus);
                DamageAPI.ModdedDamageTypeHolderComponent moddedDamage = cleaver.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                moddedDamage.Add(DamageTypes.InterrogatorPressure);
                if(base.characterBody.HasBuff(SubmarinerBuffs.interrogatorConvictBuff)) moddedDamage.Add(DamageTypes.InterrogatorConvict);
                ProjectileManager.instance.FireProjectile(cleaver, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), this.gameObject, this.damageStat * SubmarinerStaticValues.cleaverDamageCoefficient, this.force, this.RollCrit(), DamageColorIndex.Default, null, -1f);
                if (moddedDamage.Has(DamageTypes.InterrogatorPressure)) moddedDamage.Remove(DamageTypes.InterrogatorPressure);
                if (moddedDamage.Has(DamageTypes.InterrogatorConvict)) moddedDamage.Remove(DamageTypes.InterrogatorConvict);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public override void PlayAnimation(float duration)
        {
            if (base.GetModelAnimator())
            {
                base.PlayAnimation("Gesture, Override", "SwingCleaver", "Swing.playbackRate", this.duration * 5.5f);
            }
        }
    }
}