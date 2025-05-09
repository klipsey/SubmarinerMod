using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using RoR2.Projectile;
using RoR2;
using static UnityEngine.SendMouseEvents;
using R2API;
using SubmarinerMod.SubmarinerCharacter.Content;

namespace SubmarinerMod.SubmarinerCharacter.Components
{
    public class AnchorConnectionComponent : MonoBehaviour
    {
        public GameObject chain = SubmarinerAssets.anchorTether;

        public static AnimationCurve yankSuitabilityCurve;

        private CharacterBody ownerBody;

        private SubmarinerController subController;

        private GameObject owner;

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
            }
            if(owner)
            {
                subController = owner.GetComponent<SubmarinerController>();
                ownerBody = owner.GetComponent<CharacterBody>();

                if(ownerBody && NetworkServer.active) ownerBody.AddTimedBuff(RoR2.RoR2Content.Buffs.CloakSpeed, 3f);
            }
            chain.GetComponent<DestroyOnCondition>().anchor = this.gameObject.GetComponent<AnchorConnectionComponent>();
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (previousPosition <= Vector3.Distance(owner.transform.position, base.transform.position) - 5f || previousPosition >= Vector3.Distance(owner.transform.position, base.transform.position) + 5f && !hasBroken)
            {
                previousPosition = Vector3.Distance(owner.transform.position, base.transform.position);
                subController.movementSpeedAnchorIncrease = Mathf.Abs(Util.Remap(Vector3.Distance(owner.transform.position, base.transform.position), 25f, 50f, 1.5f, 0.85f));
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
                Object.Destroy(this.gameObject);
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
            if(ownerBody)
            {
                Util.PlaySound("sfx_chainsnap", owner);
                if(subController)
                {
                    subController.movementSpeedAnchorIncrease = 1f;
                    subController.ResetAnchorMaterial();
                    ownerBody.RecalculateStats();
                    if (NetworkServer.active) ownerBody.AddTimedBuff(RoR2.RoR2Content.Buffs.CloakSpeed, 3f);
                }
                ownerBody = null;
            }
        }
    }
}
