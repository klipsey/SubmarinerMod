using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using RoR2.Projectile;
using RoR2;
using static UnityEngine.SendMouseEvents;
using R2API;
using SubmarinerMod.Submariner.Content;

namespace SubmarinerMod.Submariner.Components
{
    public class AnchorConnectionComponent : MonoBehaviour
    {
        public GameObject chain = SubmarinerAssets.anchorTether;

        public static AnimationCurve yankSuitabilityCurve;

        private CharacterBody ownerBody;

        private SubmarinerController subController;

        private GameObject owner;

        private TeamIndex teamIndex = TeamIndex.None;

        private bool hasFired;

        private bool hasBroken;

        private float previousPosition;

        public bool ownerIsInRange = true;

        private float timer;
        public void Start()
        {
            ProjectileController component = GetComponent<ProjectileController>();
            if (component)
            {
                owner = component.owner;
                teamIndex = component.teamFilter.teamIndex;
            }
            subController = owner.GetComponent<SubmarinerController>();
            ownerBody = owner.GetComponent<CharacterBody>();
            chain.GetComponent<DestroyOnCondition>().anchor = this.gameObject.GetComponent<AnchorConnectionComponent>();
            ownerBody.AddTimedBuff(RoR2.RoR2Content.Buffs.CloakSpeed, 3f);
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (previousPosition <= Vector3.Distance(owner.transform.position, base.transform.position) - 5f || previousPosition >= Vector3.Distance(owner.transform.position, base.transform.position) + 5f && !hasBroken)
            {
                previousPosition = Vector3.Distance(owner.transform.position, base.transform.position);
                ownerBody.GetComponent<SubmarinerController>().movementSpeedAnchorIncrease = Mathf.Abs(Util.Remap(Vector3.Distance(owner.transform.position, base.transform.position), 25f, 50f, 1.5f, 0.85f));
                ownerBody.RecalculateStats();
            }
            if (Vector3.Distance(owner.transform.position, base.transform.position) > 75f)
            {
                ownerIsInRange = false;
                hasBroken = true;
            }
            else
            {
                ownerIsInRange = true;
            }
            if (ownerIsInRange && !hasFired)
            {
                ChainUpdate(16f);
                hasFired = true;
            }
            if (!ownerIsInRange && timer >= 1f)
            {
                Object.Destroy(gameObject);
            }
        }

        private void ChainUpdate(float num)
        {
            Vector3 position = transform.position;
            EffectData effectData = new EffectData
            {
                scale = 1f,
                origin = position,
                genericFloat = num,
            };
            effectData.SetHurtBoxReference(owner);
            EffectManager.SpawnEffect(chain, effectData, transmit: false);
        }

        public void OnDestroy()
        {
            Util.PlaySound("sfx_chainsnap", owner);
            ownerBody.GetComponent<SubmarinerController>().movementSpeedAnchorIncrease = 1f;
            ownerBody.RecalculateStats();
            ownerBody.AddTimedBuff(RoR2.RoR2Content.Buffs.CloakSpeed, 3f);
            if (ownerBody != null)
            {
                ownerBody = null;
            }
        }
    }
}
