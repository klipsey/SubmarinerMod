using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.HudOverlay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using SubmarinerMod.Submariner.Content;
using System;
using SubmarinerMod.Submariner.SkillStates;
using static UnityEngine.UI.GridLayoutGroup;

namespace SubmarinerMod.Submariner.Components
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

        public float maxRegen = 0f;

        public bool pauseTimer = false;

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
        }
        public void ApplySkin()
        {
            if (this.skinController)
            {
            }
        }

        public void SetCurrentMaxRegen(float regen)
        {
            if(regen > maxRegen)
            {
                maxRegen = regen;
            }
        }
        #endregion
        private void FixedUpdate()
        {
            if(!characterBody.HasBuff(SubmarinerBuffs.SubmarinerRegenBuff))
            {
                maxRegen = 0f;
            }    
        }

        public void EnableSword()
        {
            //this.childLocator.FindChild("AnchorModel").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = SubmarinerAssets.swordMesh;
        }
        public void DisableSword() 
        {
            //this.childLocator.FindChild("AnchorModel").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = SubmarinerAssets.batMesh;

            convictedVictimBody = null;
        }
        private void OnDestroy()
        {
        }
    }
}
