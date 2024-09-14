using SubmarinerMod.Modules.BaseStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using SubmarinerMod.SubmarinerCharacter.Content;
using R2API;
using EntityStates;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using SubmarinerMod.SubmarinerCharacter.Components;
using MonoMod.RuntimeDetour;

namespace SubmarinerMod.SubmarinerCharacter.SkillStates
{
    public class HarpoonShot : BaseSubmarinerSkillState
    {
        private Transform modelTransform;

        public static GameObject dashPrefab = SubmarinerAssets.dashEffect;

        public static GameObject projectilePrefab = SubmarinerAssets.hookPrefab;
        public static float smallHopVelocity = 12f;

        public static float baseDuration = 1f;

        public static float dashDuration = 0.5f;

        public static float pushAwayForce = 10f;
        public static float pushAwayYFactor = 2.5f;

        public static float speedCoefficient = 5f;

        public static string beginSoundString = "sfx_driver_dodge";

        public static string endSoundString = "sfx_submariner_dash";

        public static float damageCoefficient = SubmarinerStaticValues.harpoonDamageCoefficient;

        public static float procCoefficient = 1f;

        public static GameObject hitEffectPrefab = SubmarinerAssets.batHitEffectRed;

        public static float hitPauseDuration = 0.012f;

        private OverlapAttack attack;
        private List<HurtBox> victimsStruck = new List<HurtBox>();

        private Vector3 dashVector = Vector3.zero;

        private ChildLocator childLocator;

        public GameObject hookInstance;

        protected ProjectileStickOnImpact hookStickOnImpact;

        private bool isStuck;

        private bool hasHit;

        private bool hadHookInstance;

        private CameraTargetParams.AimRequest aimRequest;

        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
            StartAimMode(0.5f + baseDuration, false);

            dashVector = base.GetAimRay().direction;

            this.attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.damageType = DamageType.Stun1s;
            attack.procCoefficient = 1f;
            attack.teamIndex = base.GetTeam();
            attack.isCrit = base.RollCrit();
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 1f;
            attack.damage = damageCoefficient * damageStat;
            attack.hitBoxGroup = FindHitBoxGroup("HarpoonKickHitbox");
            attack.hitEffectPrefab = SubmarinerAssets.batHitEffect;
            attack.AddModdedDamageType(DamageTypes.SubmarinerRegeneration);

            FireProjectile();
            Util.PlaySound("Play_loader_m2_launch", base.gameObject);
            PlayAnimation("Gesture, Override", "FireHarpoon", "Harpoon.playbackRate", baseDuration);
        }

        private void CreateDashEffect()
        {
            Transform transform = childLocator.FindChild("Chest");
            if (transform && dashPrefab)
            {
                EffectManager.SpawnEffect(dashPrefab, new EffectData
                {
                    origin = transform.position,
                    rotation = Util.QuaternionSafeLookRotation(-dashVector)

                }, true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            StartAimMode(2f, false);

            if (base.isAuthority)
            {
                if (this.hookStickOnImpact)
                {
                    if (this.hookStickOnImpact.stuckBody)
                    {
                        Rigidbody component = hookStickOnImpact.stuckBody.GetComponent<Rigidbody>();
                        if (component && component.mass >= hookInstance.GetComponent<ProjectileGrappleController>().yankMassLimit)
                        {
                            if (this.hookStickOnImpact.stuck && !this.isStuck)
                            {
                                if (NetworkServer.active)
                                {
                                    characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                                }
                                PlayAnimation("FullBody, Override", "HarpoonPullStart", "Dash.playbackRate", dashDuration);
                                base.characterMotor.velocity.y = 0f;
                                base.characterMotor.velocity += dashVector * (5f * 10.5f);
                            }
                            this.isStuck = this.hookStickOnImpact.stuck;
                            if (attack.Fire(victimsStruck))
                            {
                                PlayAnimation("FullBody, Override", "HarpoonEndToKick", "Dash.playbackRate", dashDuration);

                                hasHit = true;
                                base.characterMotor.velocity.y = 0f;
                                base.characterDirection.forward = dashVector;
                                base.characterBody.isSprinting = true;

                                base.characterMotor.Motor.ForceUnground();
                                Vector3 knockback = -base.characterDirection.forward;
                                knockback.y = pushAwayYFactor;
                                base.characterMotor.velocity = knockback * pushAwayForce;

                                outer.SetNextState(new Bounce
                                {
                                    faceDirection = dashVector
                                });
                            }
                        }
                    }
                }

                if (base.isAuthority && !this.hookInstance && this.hadHookInstance)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public void SetHookReference(GameObject hook)
        {
            this.hookInstance = hook;
            this.hookStickOnImpact = hook.GetComponent<ProjectileStickOnImpact>();
            this.hadHookInstance = true;
        }
        public void FireProjectile()
        {
            if (base.isAuthority)
            {
                Ray aimRay = GetAimRay();
                FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
                fireProjectileInfo.position = aimRay.origin;
                fireProjectileInfo.rotation = Quaternion.LookRotation(aimRay.direction);
                fireProjectileInfo.crit = base.characterBody.RollCrit();
                fireProjectileInfo.damage = damageStat * (damageCoefficient / 2f);
                fireProjectileInfo.force = 0f;
                fireProjectileInfo.damageColorIndex = DamageColorIndex.Default;
                fireProjectileInfo.procChainMask = default(ProcChainMask);
                fireProjectileInfo.projectilePrefab = projectilePrefab;
                fireProjectileInfo.owner = base.gameObject;
                FireProjectileInfo fireProjectileInfo2 = fireProjectileInfo;
                ProjectileManager.instance.FireProjectile(fireProjectileInfo2);
            }
        }
        public override void OnExit()
        {
            base.OnExit();

            if (!hasHit)
            {
                base.PlayCrossfade("FullBody, Override", "BufferEmpty", 0.1f);
            }
            if (NetworkServer.active && characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.3f);
            }


        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
