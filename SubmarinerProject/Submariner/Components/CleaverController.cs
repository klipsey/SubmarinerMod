using RoR2;
using UnityEngine;
using RoR2.Projectile;
using Unity;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using R2API;
using InterrogatorMod.Interrogator.Content;

namespace InterrogatorMod.Interrogator.Components
{
    [RequireComponent(typeof(ProjectileDamage))]
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(ProjectileStickOnImpact))]
    public class CleaverController : NetworkBehaviour
    {
        private GameObject owner;
        private CharacterBody ownerBody;
        private GameObject victim;
        private CharacterBody victimBody;
        private HealthComponent victimHealth;
        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
        private ProjectileStickOnImpact projectileStickOnImpact;
        private bool hasAppliedAllyDamage;
        private void Start()
        {
            if(NetworkServer.active)
            {
                projectileController = this.GetComponent<ProjectileController>();
                owner = projectileController.owner;
                if(owner)
                {
                    ownerBody = owner.GetComponent<CharacterBody>();
                }
                projectileDamage = this.GetComponent<ProjectileDamage>();
                projectileStickOnImpact = this.GetComponent<ProjectileStickOnImpact>();
                hasAppliedAllyDamage = false;
            }
        }

        private void FixedUpdate()
        {
            if (hasAppliedAllyDamage) this.enabled = false;
            if (!NetworkServer.active || !projectileStickOnImpact.stuck) return;
            if(!projectileStickOnImpact.victim) return;
            victim = projectileStickOnImpact.victim;
            victimBody = projectileStickOnImpact.victim.GetComponent<CharacterBody>();
            if (!victimBody) return;
            victimHealth = victimBody.healthComponent;
            if (!victimHealth) return;
            TeamIndex teamIndex = victimBody.teamComponent.teamIndex;
            if(teamIndex == ownerBody.teamComponent.teamIndex)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    attacker = owner,
                    inflictor = owner,
                    damage = projectileDamage.damage * 0.25f,
                    damageColorIndex = DamageColorIndex.SuperBleed,
                    damageType = projectileDamage.damageType,
                    crit = projectileDamage.crit,
                    force = Vector3.zero,
                    position = this.transform.position,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = projectileController.procCoefficient,
                };
                damageInfo.AddModdedDamageType(DamageTypes.InterrogatorPressure);
                RpcPlayHitSound(victim);
                victimHealth.TakeDamage(damageInfo);
                hasAppliedAllyDamage = true;
            }
        }

        [ClientRpc]
        private void RpcPlayHitSound(GameObject victim)
        {
            if (NetworkServer.active)
            {
                Util.PlaySound("sfx_scout_cleaver_hit", victim);
            }
        }
    }
}
