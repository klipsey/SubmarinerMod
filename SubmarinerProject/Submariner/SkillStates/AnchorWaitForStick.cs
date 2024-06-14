using SubmarinerMod.Modules.BaseStates;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;

namespace SubmarinerMod.Submariner.SkillStates
{
    public class AnchorWaitForStick : AnchorBaseState
    {
        protected override bool shouldStick => true;

        protected override bool shouldRevertToWaitForStickOnSurfaceLost => false;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && base.projectileStickOnImpact.stuck)
            {
                outer.SetNextState(new AnchorTetherBehaviour());
            }
        }
    }
}

