using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using SubmarinerMod.Modules.BaseStates;

namespace SubmarinerMod.SubmarinerCharacter.SkillStates
{
    public class BeastImpact : BaseSubmarinerState
    {
        public HealthComponent victimHealthComponent;

        public Vector3 idealDirection;

        public bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                if (victimHealthComponent)
                {
                    DamageInfo damageInfo = new DamageInfo
                    {
                        attacker = base.gameObject,
                        damage = damageStat * BeastRide.knockbackDamageCoefficient,
                        crit = isCrit,
                        procCoefficient = 1f,
                        damageColorIndex = DamageColorIndex.Item,
                        damageType = DamageType.Stun1s | DamageType.BonusToLowHealth,
                        position = base.characterBody.corePosition
                    };
                    victimHealthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, victimHealthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, victimHealthComponent.gameObject);
                }
                base.healthComponent.TakeDamageForce(idealDirection * (0f - BeastRide.knockbackForce), alwaysApply: true);
            }
            if (base.isAuthority)
            {
                AddRecoil(-0.5f * BeastRide.recoilAmplitude * 3f, -0.5f * BeastRide.recoilAmplitude * 3f, -0.5f * BeastRide.recoilAmplitude * 8f, 0.5f * BeastRide.recoilAmplitude * 3f);
                EffectManager.SimpleImpactEffect(BeastRide.knockbackEffectPrefab, base.characterBody.corePosition, base.characterDirection.forward, transmit: true);
                outer.SetNextStateToMain();
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(victimHealthComponent ? victimHealthComponent.gameObject : null);
            writer.Write(idealDirection);
            writer.Write(isCrit);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            GameObject gameObject = reader.ReadGameObject();
            victimHealthComponent = (gameObject ? gameObject.GetComponent<HealthComponent>() : null);
            idealDirection = reader.ReadVector3();
            isCrit = reader.ReadBoolean();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
