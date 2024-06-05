using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.HudOverlay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using InterrogatorMod.Interrogator.Content;
using System;

namespace InterrogatorMod.Interrogator.Components
{
    public class SubmarinerController : MonoBehaviour
    {
        private CharacterBody characterBody;
        private ModelSkinController skinController;
        private ChildLocator childLocator;
        private CharacterModel characterModel;
        private Animator animator;
        private SkillLocator skillLocator;
        private Material[] swordMat;
        private Material[] batMat;

        public CharacterBody convictedVictimBody;
        public string currentSkinNameToken => this.skinController.skins[this.skinController.currentSkinIndex].nameToken;
        public string altSkinNameToken => SubmarinerSurvivor.INTERROGATOR_PREFIX + "MASTERY_SKIN_NAME";

        private bool hasPlayed = false;
        public bool isConvicted => this.characterBody.HasBuff(SubmarinerBuffs.interrogatorConvictBuff);
        private bool stopwatchOut = false;
        public bool pauseTimer = false;

        public float convictDurationMax = SubmarinerStaticValues.baseConvictTimerMax;

        private int guiltyCounter = 0;

        private uint playID1;

        private ParticleSystem swordEffect;
        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            this.childLocator = modelLocator.modelBaseTransform.GetComponentInChildren<ChildLocator>();
            this.animator = modelLocator.modelBaseTransform.GetComponentInChildren<Animator>();
            this.characterModel = modelLocator.modelBaseTransform.GetComponentInChildren<CharacterModel>();
            this.skillLocator = this.GetComponent<SkillLocator>();
            this.skinController = modelLocator.modelTransform.gameObject.GetComponent<ModelSkinController>();

            Hook();

            this.Invoke("ApplySkin", 0.3f);
        }
        private void Start()
        {
        }
        #region tooMuchCrap
        private void Hook()
        {
            On.RoR2.FriendlyFireManager.ShouldSplashHitProceed += FriendlyFireManager_ShouldSplashHitProceed;
            On.RoR2.FriendlyFireManager.ShouldDirectHitProceed += FriendlyFireManager_ShouldDirectHitProceed;
            On.RoR2.FriendlyFireManager.ShouldSeekingProceed += FriendlyFireManager_ShouldSeekingProceed;
        }
        public void ApplySkin()
        {
            if (this.skinController)
            {
                this.swordEffect = this.childLocator.FindChild("SpecialEffectHand").gameObject.GetComponent<ParticleSystem>();
                this.swordMat = new Material[1];
                this.batMat = new Material[1];
                this.swordMat[0] = SubmarinerAssets.swordMat;
                this.batMat[0] = SubmarinerAssets.batMat;
            }
        }
        private bool FriendlyFireManager_ShouldSeekingProceed(On.RoR2.FriendlyFireManager.orig_ShouldSeekingProceed orig, HealthComponent victim, TeamIndex attackerTeamIndex)
        {
            if (victim.body.baseNameToken == "KENKO_INTERROGATOR_NAME" && attackerTeamIndex == victim.body.teamComponent.teamIndex)
            {
                return true;
            }
            else return orig.Invoke(victim, attackerTeamIndex);
        }

        private bool FriendlyFireManager_ShouldDirectHitProceed(On.RoR2.FriendlyFireManager.orig_ShouldDirectHitProceed orig, HealthComponent victim, TeamIndex attackerTeamIndex)
        {
            if (victim.body.baseNameToken == "KENKO_INTERROGATOR_NAME" && attackerTeamIndex == victim.body.teamComponent.teamIndex)
            {
                return true;
            }
            else return orig.Invoke(victim, attackerTeamIndex);
        }

        private bool FriendlyFireManager_ShouldSplashHitProceed(On.RoR2.FriendlyFireManager.orig_ShouldSplashHitProceed orig, HealthComponent victim, TeamIndex attackerTeamIndex)
        {
            if (victim.body.baseNameToken == "KENKO_INTERROGATOR_NAME" && attackerTeamIndex == victim.body.teamComponent.teamIndex)
            {
                return true;
            }
            else return orig.Invoke(victim, attackerTeamIndex);
        }
        private void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            if (itemIndex == DLC1Content.Items.EquipmentMagazineVoid.itemIndex)
            {
                this.convictDurationMax = SubmarinerStaticValues.baseConvictTimerMax + this.characterBody.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
            }
        }

        #endregion
        private void FixedUpdate()
        {
            if ((!characterBody.HasBuff(SubmarinerBuffs.interrogatorConvictBuff) || !convictedVictimBody.healthComponent.alive) && guiltyCounter > 0 && !hasPlayed)
            {
                hasPlayed = true;
                DisableSword();
            }

            if(skillLocator.secondary.CanExecute() && !childLocator.FindChild("CleaverModel").gameObject.activeSelf)
            {
                childLocator.FindChild("CleaverModel").gameObject.SetActive(true);
            }
            else if(!skillLocator.secondary.CanExecute() && childLocator.FindChild("CleaverModel").gameObject.activeSelf)
            {
                childLocator.FindChild("CleaverModel").gameObject.SetActive(false);
            }
        }

        public void AddToCounter()
        {
            guiltyCounter++;
        }
        public void EnableSword()
        {
            this.childLocator.FindChild("MeleeModel").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = SubmarinerAssets.swordMesh;
            hasPlayed = false;
        }
        public void DisableSword() 
        {
            this.childLocator.FindChild("MeleeModel").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = SubmarinerAssets.batMesh;

            if (NetworkServer.active)
            {
                if (characterBody.HasBuff(SubmarinerBuffs.interrogatorConvictBuff)) characterBody.RemoveOldestTimedBuff(SubmarinerBuffs.interrogatorConvictBuff);
                for (int i = this.guiltyCounter; i > 0; i--)
                {
                    this.characterBody.RemoveBuff(SubmarinerBuffs.interrogatorGuiltyBuff);
                }
            }

            guiltyCounter = 0;
            convictedVictimBody = null;
        }
        private void OnDestroy()
        {
            AkSoundEngine.StopPlayingID(this.playID1);

            if (this.characterBody && this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onItemAddedClient -= this.Inventory_onItemAddedClient;
            }

            On.RoR2.FriendlyFireManager.ShouldSplashHitProceed -= FriendlyFireManager_ShouldSplashHitProceed;
            On.RoR2.FriendlyFireManager.ShouldDirectHitProceed -= FriendlyFireManager_ShouldDirectHitProceed;
            On.RoR2.FriendlyFireManager.ShouldSeekingProceed -= FriendlyFireManager_ShouldSeekingProceed;
        }
    }
}
