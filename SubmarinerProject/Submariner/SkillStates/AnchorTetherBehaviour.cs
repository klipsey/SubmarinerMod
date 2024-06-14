using SubmarinerMod.Modules.BaseStates;
using UnityEngine;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using SubmarinerMod.Submariner.Content;
using SubmarinerMod.Submariner.Components;
using static UnityEngine.UI.Image;

namespace SubmarinerMod.Submariner.SkillStates
{
    public class AnchorTetherBehaviour : AnchorBaseState
    {
        public GameObject chain = SubmarinerAssets.anchorTether;

        public static AnimationCurve yankSuitabilityCurve;

        private CharacterBody ownerBody;

        private SubmarinerController subController;

        private GameObject owner;

        private TeamIndex teamIndex = TeamIndex.None;

        private bool hasFired;

        private bool ownerIsInRange;
        public override void OnEnter()
        {
            base.OnEnter();
            ProjectileController component = GetComponent<ProjectileController>();
            if (component)
            {
                owner = component.owner;
                teamIndex = component.teamFilter.teamIndex;
            }
            PlayAnimation("Base", "SpawnToIdle");
            Util.PlaySound("Play_treeBot_R_yank", owner);
            subController = owner.GetComponent<SubmarinerController>();
            ownerBody = owner.GetComponent<CharacterBody>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(Vector3.Distance(owner.transform.position, base.transform.position) > 200f)
            {
                ownerIsInRange = false;
            }
            if (ownerIsInRange && !hasFired)
            {
                ChainUpdate(10f);
                hasFired = true;
            }
            if (!ownerIsInRange && base.fixedAge > 1f)
            {
                Object.Destroy(chain);
                EntityState.Destroy(this.gameObject);
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

        public override void OnExit()
        {
            if (ownerBody != null)
            {
                ownerBody = null;
            }
            base.OnExit();
        }
    }
}

