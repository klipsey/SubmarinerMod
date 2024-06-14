using UnityEngine;
using EntityStates;
using SubmarinerMod.Modules.BaseStates;
using RoR2;
using UnityEngine.AddressableAssets;
using SubmarinerMod.Submariner.Content;
using static R2API.DamageAPI;

namespace SubmarinerMod.Submariner.SkillStates
{
    public class Swing : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            RefreshState();
            hitboxGroupName = "MeleeHitbox";

            damageType = DamageType.Stun1s;
            damageCoefficient = SubmarinerStaticValues.swingDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 600f;
            bonusForce = Vector3.zero;
            baseDuration = 1.2f;

            //0-1 multiplier of baseduration, used to time when the hitbox is out (usually based on the run time of the animation)
            //for example, if attackStartPercentTime is 0.5, the attack will start hitting halfway through the ability. if baseduration is 3 seconds, the attack will start happening at 1.5 seconds
            attackStartPercentTime = 0.65f;
            attackEndPercentTime = 1f;

            //this is the point at which the attack can be interrupted by itself, continuing a combo
            earlyExitPercentTime = 1f;

            hitStopDuration = 0.15f;
            attackRecoil = 0.75f;
            hitHopVelocity = 6f;

            swingSoundString = "Play_loader_m1_swing";
            hitSoundString = "";
            muzzleString = swingIndex % 2 == 0 ? "SwingMuzzle1" : "SwingMuzzle2";
            playbackRateParam = "Swing.playbackRate";
            swingEffectPrefab = SubmarinerAssets.batSwingEffect;
            moddedDamageTypeHolder.Add(DamageTypes.SubmarinerRegeneration);
            hitEffectPrefab = SubmarinerAssets.batHitEffect;

            impactSound = SubmarinerAssets.batImpactSoundEvent.index;

            base.OnEnter();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("MeleePivot").rotation = Util.QuaternionSafeLookRotation(direction);
            }

            base.FireAttack();
        }

        protected override void PlaySwingEffect()
        {
            Util.PlaySound(this.swingSoundString, this.gameObject);
            if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    this.swingEffectPrefab = Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                }
            }
        }

        protected override void PlayAttackAnimation()
        {
            PlayCrossfade("Gesture, Override", "Swing" + (1 + swingIndex), playbackRateParam, duration * 1.3f, duration * 0.3f);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}