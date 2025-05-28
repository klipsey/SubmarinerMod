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

            if(characterMotor.isGrounded)
            {
                base.characterMotor.Motor.ForceUnground();
            }

            base.characterMotor.ApplyForce(Vector3.up * 12f, alwaysApply: true, disableAirControlUntilCollision: false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterDirection.forward = faceDirection;
                base.characterBody.isSprinting = true;
                base.characterMotor.disableAirControlUntilCollision = false;
                if (base.fixedAge >= baseDuration || base.characterMotor.isGrounded)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if(base.characterMotor.isGrounded)
            {
                PlayCrossfade("FullBody, Override", "BufferEmpty", 0.1f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}