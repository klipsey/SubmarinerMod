using HG;
using Newtonsoft.Json.Linq;
using R2API;
using RoR2;
using RoR2.Projectile;
using SubmarinerMod.Modules;
using SubmarinerMod.Submariner.Components;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static RoR2.DotController;

namespace SubmarinerMod.Submariner.Content
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType Default;
        public static DamageAPI.ModdedDamageType SubmarinerRegeneration;

        internal static void Init()
        {
            Default = DamageAPI.ReserveDamageType();
            SubmarinerRegeneration = DamageAPI.ReserveDamageType();
            Hook();
        }
        private static void Hook()
        {
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
        }
        private static void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            DamageInfo damageInfo = damageReport.damageInfo;
            if (!damageReport.attackerBody || !damageReport.victimBody)
            {
                return;
            }
            HealthComponent victim = damageReport.victim;
            GameObject inflictorObject = damageInfo.inflictor;
            CharacterBody victimBody = damageReport.victimBody;
            EntityStateMachine victimMachine = victimBody.GetComponent<EntityStateMachine>();
            CharacterBody attackerBody = damageReport.attackerBody;
            GameObject attackerObject = damageReport.attacker.gameObject;
            if (NetworkServer.active)
            {
                if (attackerBody.baseNameToken == "KENKO_SUBMARINER_NAME")
                {
                    if (damageInfo.HasModdedDamageType(SubmarinerRegeneration))
                    {
                        if(victimBody && attackerBody)
                        {
                            float regen = Mathf.Clamp(victimBody.healthComponent.health * 0.025f, 0.01f, attackerBody.healthComponent.health * 0.5f);
                            if(attackerBody.TryGetComponent<SubmarinerController>(out var s))
                            {
                                s.SetCurrentMaxRegen(regen);
                                attackerBody.AddTimedBuff(SubmarinerBuffs.SubmarinerRegenBuff, 2.5f);
                                if (attackerBody.GetBuffCount(SubmarinerBuffs.SubmarinerRegenBuff) > 1)
                                {
                                    attackerBody.RemoveOldestTimedBuff(SubmarinerBuffs.SubmarinerRegenBuff);
                                }
                                Log.Debug("ok");
                            }
                        }
                    }
                }
            }
        }
    }
}
