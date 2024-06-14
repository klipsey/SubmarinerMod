using UnityEngine;
using EntityStates;
using SubmarinerMod.Modules.BaseStates;
using RoR2;
using UnityEngine.AddressableAssets;
using SubmarinerMod.Submariner.Content;
using UnityEngine.Networking;
using SubmarinerMod.Submariner.Components;
using static RoR2.OverlapAttack;

namespace SubmarinerMod.Submariner.SkillStates
{
    public class Convict : BaseSubmarinerSkillState
    {
        public GameObject markedPrefab = SubmarinerAssets.SubmarinerConvictedConsume;
        private float baseDuration = 0.5f;

        private float duration;

        private HurtBox victim;

        private CharacterBody victimBody;

        private CameraTargetParams.AimRequest aimRequest;
        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            aimRequest?.Dispose();
        }
    }
}