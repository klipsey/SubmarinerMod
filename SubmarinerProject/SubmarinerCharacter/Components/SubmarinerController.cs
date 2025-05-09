﻿using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.HudOverlay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using SubmarinerMod.SubmarinerCharacter.Content;
using System;
using SubmarinerMod.SubmarinerCharacter.SkillStates;
using static UnityEngine.UI.GridLayoutGroup;

namespace SubmarinerMod.SubmarinerCharacter.Components
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

        public bool pauseTimer = false;

        private ParticleSystem swordEffect;

        public float movementSpeedAnchorIncrease = 1f;
        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            ModelLocator modelLocator = this.GetComponent<ModelLocator>();
            this.childLocator = modelLocator.modelBaseTransform.GetComponentInChildren<ChildLocator>();
            this.animator = modelLocator.modelBaseTransform.GetComponentInChildren<Animator>();
            this.characterModel = modelLocator.modelBaseTransform.GetComponentInChildren<CharacterModel>();
            this.skillLocator = this.GetComponent<SkillLocator>();
            this.skinController = modelLocator.modelTransform.gameObject.GetComponent<ModelSkinController>();

            this.Invoke("ApplySkin", 0.3f);
        }
        private void Start()
        {
        }
        #region tooMuchCrap
        public void ApplySkin()
        {
            if (this.skinController)
            {
                if(!SubmarinerConfig.enableFunnyMode.Value) childLocator.FindChild("BeastModel").gameObject.SetActive(false);
            }
        }

        #endregion
        private void FixedUpdate()
        {

        }

        public void EnableAnchor()
        {
            childLocator.FindChild("AnchorModel").gameObject.SetActive(true);
            childLocator.FindChild("AnchorModel").gameObject.GetComponent<SkinnedMeshRenderer>().materials[0] = SubmarinerAssets.ghostMat;
        }
        public void DisableAnchor() 
        {
            childLocator.FindChild("AnchorModel").gameObject.SetActive(false);
        }

        public void ResetAnchorMaterial()
        {
            childLocator.FindChild("AnchorModel").gameObject.GetComponent<SkinnedMeshRenderer>().materials[0] = SubmarinerAssets.anchorMat;
        }
        private void OnDestroy()
        {
        }
    }
}
