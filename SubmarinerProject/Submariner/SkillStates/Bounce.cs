using UnityEngine;
using RoR2;
using EntityStates;
using EntityStates.Loader;
using UnityEngine.Networking;
using SubmarinerMod.Modules.BaseStates;

namespace SubmarinerMod.Submariner.SkillStates
{
    internal class CometBounce : BaseSubmarinerSkillState
    {
        public static float baseDuration = 1f;

        internal Vector3 faceDirection;

        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterDirection.forward = faceDirection;
                base.characterBody.isSprinting = true;

                if (base.fixedAge >= baseDuration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}