using SubmarinerMod.Modules.BaseStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using SubmarinerMod.Submariner.Content;
using R2API;
using EntityStates;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using SubmarinerMod.Submariner.Components;
using MonoMod.RuntimeDetour;
using System.ComponentModel;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace SubmarinerMod.Submariner.SkillStates
{
    public class BackFlip : BaseSubmarinerSkillState
    {
        public static GameObject dashPrefab = SubmarinerAssets.dashEffect;

        public static GameObject projectilePrefab = SubmarinerAssets.minePrefab;
        public static float smallHopVelocity = 12f;

        public static float baseDuration = 0.5f;

        public static float dashDuration = 0.25f;

        public static float pushAwayForce = 2.5f;
        public static float pushAwayYFactor = 10f;

        public static float speedCoefficient = 7f;

        public static string beginSoundString = "sfx_driver_dodge";

        public static string endSoundString = "sfx_submariner_dash";

        public static float damageCoefficient = SubmarinerStaticValues.mineDamageCoefficient;

        public static float procCoefficient = 1f;

        public static GameObject hitEffectPrefab = SubmarinerAssets.batHitEffectRed;

        public static float hitPauseDuration = 0.012f;

        public bool mine = true;

        private Vector3 dashVector = Vector3.zero;

        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();

            StartAimMode(0.5f + baseDuration, false);

            dashVector = base.GetAimRay().direction;

            if(mine) FireProjectile();

            if (base.isAuthority)
            {
                if (NetworkServer.active)
                {
                    characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                base.characterMotor.velocity.y = 0f;
                base.characterDirection.forward = dashVector;
                base.characterBody.isSprinting = true;

                base.characterMotor.Motor.ForceUnground();
                Vector3 knockback = -base.characterDirection.forward;
                knockback.y = pushAwayYFactor;
                base.characterMotor.velocity = knockback * pushAwayForce;
            }
            PlayAnimation("FullBody, Override", "BackFlip", "Dash.playbackRate", baseDuration + dashDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            StartAimMode(2f, false);

            if (base.isAuthority && fixedAge >= baseDuration + dashDuration)
            {
                outer.SetNextStateToMain();
            }
        }
        public void FireProjectile()
        {
            Ray aimRay = GetAimRay();
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, damageStat * damageCoefficient, 200f, Util.CheckRoll(critStat, base.characterBody.master));
            }
        }
        public override void OnExit()
        {
            if (NetworkServer.active && characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.3f);
            }

            SmallHop(characterMotor, smallHopVelocity);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
