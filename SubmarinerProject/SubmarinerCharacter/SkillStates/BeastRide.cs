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
using EntityStates.Merc;
using EntityStates.Loader;
using EntityStates.BrotherMonster;

namespace SubmarinerMod.SubmarinerCharacter.SkillStates
{
    public class BeastRide : BaseCharacterMain
    {
        public float baseDuration = 5f;

        public float speedMultiplier = 2.2f;

        public static float chargeDamageCoefficient = SubmarinerConfig.beastDamageCoefficient.Value;

        public static float awayForceMagnitude = 0f;

        public static float upwardForceMagnitude = 2100f;

        public static GameObject impactEffectPrefab = EntityStates.Loader.LoaderMeleeAttack.overchargeImpactEffectPrefab;

        public static float hitPauseDuration = 0.04f;

        public static string impactSoundString = "Play_acrid_m2_bite_shoot";

        public static float recoilAmplitude = 1f;

        public static string startSoundString = "Play_acrid_shift_fly_loop";

        public static string endSoundString = "Stop_acrid_shift_fly_loop";

        public static GameObject knockbackEffectPrefab = EntityStates.Loader.LoaderMeleeAttack.overchargeImpactEffectPrefab;

        public static float knockbackDamageCoefficient = 10f;

        public static float massThresholdForKnockback = 250f;

        public static float knockbackForce = 8000f;

        private uint soundID;

        private float duration;

        private float hitPauseTimer;

        private Vector3 idealDirection;

        private OverlapAttack attack;

        private bool inHitPause;

        private List<HurtBox> victimsStruck = new List<HurtBox>();

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration;
            if (base.isAuthority)
            {
                if (base.inputBank)
                {
                    idealDirection = base.inputBank.aimDirection;
                    idealDirection.y = 0f;
                }
                UpdateDirection();
            }
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = true;
            }
            if (base.characterDirection)
            {
                base.characterDirection.forward = idealDirection;
            }
            soundID = Util.PlaySound(startSoundString, base.gameObject);
            if (!SubmarinerConfig.enableFunnyMode.Value) base.FindModelChild("BeastModel").gameObject.SetActive(true);
            PlayCrossfade("FullBody, Override", "BeastStart", 0.1f);
            base.modelAnimator.SetFloat("aimWeight", 0f);
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
                base.characterBody.AddBuff(SubmarinerBuffs.SubmarinerBeastBuff);
            }
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "MeleeHitbox");
            }
            attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = chargeDamageCoefficient * damageStat;
            attack.hitEffectPrefab = impactEffectPrefab;
            attack.forceVector = Vector3.up * upwardForceMagnitude;
            attack.pushAwayForce = awayForceMagnitude;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            attack.damageType = DamageType.BonusToLowHealth;
            attack.damageType.damageSource = DamageSource.Utility;
        }

        public override void OnExit()
        {
            AkSoundEngine.StopPlayingID(soundID);
            Util.PlaySound(endSoundString, base.gameObject);
            if (!outer.destroying && base.characterBody)
            {
                PlayAnimation("FullBody, Override", "BeastEnd", "Dash.playbackRate", 0.4f);
                base.characterBody.isSprinting = false;
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
                    base.characterBody.SetBuffCount(SubmarinerBuffs.SubmarinerBeastBuff.buffIndex, 0);
                }
            }
            if (base.characterMotor && !base.characterMotor.disableAirControlUntilCollision)
            {
                base.characterMotor.velocity += GetIdealVelocity();
            }
            if (base.modelLocator)
            {
                base.modelLocator.normalizeToFloor = false;
            }
            base.modelAnimator.SetFloat("aimWeight", 1f);
            base.OnExit();
            if (!SubmarinerConfig.enableFunnyMode.Value) base.FindModelChild("BeastModel").gameObject.SetActive(false);
        }

        private void UpdateDirection()
        {
            if (base.inputBank)
            {
                Vector2 vector = Util.Vector3XZToVector2XY(base.inputBank.moveVector);
                if (vector != Vector2.zero)
                {
                    vector.Normalize();
                    idealDirection = new Vector3(vector.x, 0f, vector.y).normalized;
                }
            }
        }

        private Vector3 GetIdealVelocity()
        {
            return base.characterDirection.forward * base.characterBody.moveSpeed * speedMultiplier;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
            else
            {
                if (!base.isAuthority)
                {
                    return;
                }
                if (base.characterBody)
                {
                    base.characterBody.isSprinting = true;
                }
                if (base.skillLocator.special && base.inputBank.skill4.down)
                {
                    base.skillLocator.special.ExecuteIfReady();
                }
                UpdateDirection();
                if (!inHitPause)
                {
                    if (base.characterDirection)
                    {
                        base.characterDirection.moveVector = idealDirection;
                        if (base.characterMotor && !base.characterMotor.disableAirControlUntilCollision)
                        {
                            base.characterMotor.rootMotion += GetIdealVelocity() * Time.fixedDeltaTime;
                        }
                    }
                    attack.damage = damageStat * (chargeDamageCoefficient);
                    if (!attack.Fire(victimsStruck))
                    {
                        return;
                    }
                    Util.PlaySound(impactSoundString, base.gameObject);
                    inHitPause = true;
                    hitPauseTimer = hitPauseDuration;
                    AddRecoil(-0.5f * recoilAmplitude, -0.5f * recoilAmplitude, -0.5f * recoilAmplitude, 0.5f * recoilAmplitude);
                    PlayAnimation("FullBody, Override", "BeastEat", "Dash.playbackRate", hitPauseDuration);
                    for (int i = 0; i < victimsStruck.Count; i++)
                    {
                        float num = 0f;
                        HurtBox hurtBox = victimsStruck[i];
                        if (!hurtBox.healthComponent)
                        {
                            continue;
                        }
                        CharacterMotor component = hurtBox.healthComponent.GetComponent<CharacterMotor>();
                        if (component)
                        {
                            num = component.mass;
                        }
                        else
                        {
                            Rigidbody component2 = hurtBox.healthComponent.GetComponent<Rigidbody>();
                            if (component2)
                            {
                                num = component2.mass;
                            }
                        }
                        if (num >= massThresholdForKnockback)
                        {
                            outer.SetNextState(new BeastImpact
                            {
                                victimHealthComponent = hurtBox.healthComponent,
                                idealDirection = idealDirection,
                                isCrit = attack.isCrit
                            });
                            break;
                        }
                    }
                }
                else
                {
                    base.characterMotor.velocity = Vector3.zero;
                    hitPauseTimer -= Time.fixedDeltaTime;
                    if (hitPauseTimer < 0f)
                    {
                        inHitPause = false;
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
