using SubmarinerMod.Modules.BaseStates;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.Projectile;

namespace SubmarinerMod.Submariner.SkillStates
{
    public class AnchorBaseState : BaseSubmarinerState
    {
        [SerializeField]
        public string enterSoundString;

        protected ProjectileStickOnImpact projectileStickOnImpact { get; private set; }

        protected virtual bool shouldStick => false;

        protected virtual bool shouldRevertToWaitForStickOnSurfaceLost => false;

        public override void OnEnter()
        {
            base.OnEnter();
            projectileStickOnImpact = GetComponent<ProjectileStickOnImpact>();
            if (projectileStickOnImpact.enabled != shouldStick)
            {
                projectileStickOnImpact.enabled = shouldStick;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && shouldRevertToWaitForStickOnSurfaceLost && !projectileStickOnImpact.stuck)
            {
                outer.SetNextState(new AnchorWaitForStick());
            }
        }
    }
}
