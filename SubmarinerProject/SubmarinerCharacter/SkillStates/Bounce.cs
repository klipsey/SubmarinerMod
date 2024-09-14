using UnityEngine;
using RoR2;
using EntityStates;
using SubmarinerMod.SubmarinerCharacter.Content;
using SubmarinerMod.Modules.BaseStates;

namespace SubmarinerMod.SubmarinerCharacter.SkillStates
{
    internal class Bounce : BaseSubmarinerSkillState
    {
        public static float baseDuration = 1f;

        public Vector3 faceDirection;

        public override void OnEnter()
        {
            RefreshState();
            base.OnEnter();

            if (!base.characterMotor.isGrounded)
            {
                base.characterMotor.ApplyForce(Vector3.up * 12f, alwaysApply: true, disableAirControlUntilCollision: false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterDirection.forward = faceDirection;
                base.characterBody.isSprinting = true;
                base.characterMotor.disableAirControlUntilCollision = false;
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