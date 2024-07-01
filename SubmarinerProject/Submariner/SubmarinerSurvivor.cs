using BepInEx.Configuration;
using SubmarinerMod.Modules;
using SubmarinerMod.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RoR2.UI;
using R2API;
using R2API.Networking;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using SubmarinerMod.Submariner.Components;
using SubmarinerMod.Submariner.Content;
using SubmarinerMod.Submariner.SkillStates;
using HG;
using EntityStates;
using R2API.Networking.Interfaces;
using EmotesAPI;
using System.Runtime.CompilerServices;

namespace SubmarinerMod.Submariner
{
    public class SubmarinerSurvivor : SurvivorBase<SubmarinerSurvivor>
    {
        public override string assetBundleName => "submariner";
        public override string bodyName => "SubmarinerBody";
        public override string masterName => "SubmarinerMonsterMaster";
        public override string modelPrefabName => "mdlSubmariner";
        public override string displayPrefabName => "SubmarinerDisplay";

        public const string SUBMARINER_PREFIX = SubmarinerPlugin.DEVELOPER_PREFIX + "_SUBMARINER_";
        public override string survivorTokenPrefix => SUBMARINER_PREFIX;

        internal static GameObject characterPrefab;

        public static SkillDef convictScepterSkillDef;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = SUBMARINER_PREFIX + "NAME",
            subtitleNameToken = SUBMARINER_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texSubmarinerIcon"),
            bodyColor = SubmarinerAssets.SubmarinerColor,
            sortPosition = 5.99f,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 130f,
            healthRegen = 1.5f,
            armor = 20f,
            damage = 12f, 

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Model",
                },
                new CustomRendererInfo
                {
                    childName = "AnchorModel",
                },
                new CustomRendererInfo
                {
                    childName = "GearModel",
                },
                new CustomRendererInfo
                {
                    childName = "BeastModel",
                },
                new CustomRendererInfo
                {
                    childName = "VisorModel",
                },
                new CustomRendererInfo
                {
                    childName = "HarpoonModel",
                },
                new CustomRendererInfo
                {
                    childName = "HeadModel",
                },
                new CustomRendererInfo
                {
                    childName = "RopeModel",
                },
        };

        public override UnlockableDef characterUnlockableDef => SubmarinerUnlockables.characterUnlockableDef;

        public override ItemDisplaysBase itemDisplays => new SubmarinerItemDisplays();
        public override AssetBundle assetBundle { get; protected set; }
        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }
        public override void Initialize()
        {

            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            //need the character unlockable before you initialize the survivordef

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            SubmarinerConfig.Init();

            SubmarinerUnlockables.Init();

            base.InitializeCharacter();

            CameraParams.InitializeParams();

            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

            DamageTypes.Init();

            SubmarinerStates.Init();
            SubmarinerTokens.Init();

            SubmarinerAssets.Init(assetBundle);

            SubmarinerBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            characterPrefab = bodyPrefab;

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<SubmarinerController>();
        }
        public void AddHitboxes()
        {
            Prefabs.SetupHitBoxGroup(characterModelObject, "MeleeHitbox", "MeleeHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "HarpoonKickHitbox", "HarpoonKickHitbox");
        }

        public override void InitializeEntityStateMachines()
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(SkillStates.MainState), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
            //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Dash");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Hook");
        }

        #region skills
        public override void InitializeSkills()
        {
            bodyPrefab.AddComponent<SubmarinerPassive>();
            Skills.CreateSkillFamilies(bodyPrefab);
            AddPassiveSkills();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtilitySkills();
            AddSpecialSkills();
        }

        private void AddPassiveSkills()
        {
            SubmarinerPassive passive = bodyPrefab.GetComponent<SubmarinerPassive>();

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;

            passive.SubmarinerPassiveSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = SUBMARINER_PREFIX + "PASSIVE_NAME",
                skillNameToken = SUBMARINER_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = SUBMARINER_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassive"),
                keywordTokens = new string[] { Tokens.submarinerRegenBuff },
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, passive.SubmarinerPassiveSkillDef);
        }

        private void AddPrimarySkills()
        {
            SteppedSkillDef anchorSkillDef = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "Anchor",
                    SUBMARINER_PREFIX + "PRIMARY_SWING_NAME",
                    SUBMARINER_PREFIX + "PRIMARY_SWING_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texPrimary"),
                    new SerializableEntityStateType(typeof(SkillStates.Swing)),
                    "Weapon"
                ));
            anchorSkillDef.stepCount = 2;
            anchorSkillDef.stepGraceDuration = 0.25f;
            anchorSkillDef.keywordTokens = new string[]{ };

            Skills.AddPrimarySkills(bodyPrefab, anchorSkillDef);
        }

        private void AddSecondarySkills()
        {
            SkillDef Harpoon = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Harpoon",
                skillNameToken = SUBMARINER_PREFIX + "SECONDARY_HARPOON_NAME",
                skillDescriptionToken = SUBMARINER_PREFIX + "SECONDARY_HARPOON_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondary"),

                activationState = new SerializableEntityStateType(typeof(HarpoonShot)),

                activationStateMachineName = "Hook",
                interruptPriority = InterruptPriority.Any,

                baseMaxStock = 2,
                baseRechargeInterval = 4f,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = true,
                mustKeyPress = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, Harpoon);
        }

        private void AddUtilitySkills()
        {
            SkillDef mine = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Mine",
                skillNameToken = SUBMARINER_PREFIX + "UTILITY_MINE_NAME",
                skillDescriptionToken = SUBMARINER_PREFIX + "UTILITY_MINE_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtility"),

                activationState = new SerializableEntityStateType(typeof(BackFlip)),
                activationStateMachineName = "Dash",
                interruptPriority = InterruptPriority.Skill,

                baseRechargeInterval = 7f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,

            });

            SkillDef beast = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Beast",
                skillNameToken = SUBMARINER_PREFIX + "UTILITY_BEAST_NAME",
                skillDescriptionToken = SUBMARINER_PREFIX + "UTILITY_BEAST_DESCRIPTION",
                keywordTokens = new string[] { Tokens.slayerKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texAltUtility"),

                activationState = new SerializableEntityStateType(typeof(BeastRide)),
                activationStateMachineName = "Body",
                interruptPriority = InterruptPriority.Skill,

                baseRechargeInterval = 7f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,

            });

            Skills.AddUtilitySkills(bodyPrefab, mine, beast);
        }

        private void AddSpecialSkills()
        {
            SkillDef anchorThrow = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "AnchorThrow",
                skillNameToken = SUBMARINER_PREFIX + "SPECIAL_ANCHORTHROW_NAME",
                skillDescriptionToken = SUBMARINER_PREFIX + "SPECIAL_ANCHORTHROW_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecial"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(AimAnchor)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 16f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = true,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            Skills.AddSpecialSkills(bodyPrefab, anchorThrow);
        }

        private void InitializeScepter()
        {
        }
        #endregion skills

        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texDefaultSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
            //uncomment this when you have another skin
            defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshSubmariner",
                "meshAnchor",
                "meshGear",
                "meshBeast",
                "meshVisor",
                "meshHarpoon",
                "meshHead",
                "meshRope");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            /*
            defaultSkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Tie"),
                    shouldActivate = true,
                }
            };
            */
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            /*
            #region MasterySkin

            ////creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(SUBMARINER_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMonsoonSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                SubmarinerUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
                "meshSpyAlt",
                "meshRevolverAlt",//no gun mesh replacement. use same gun mesh
                "meshKnifeAlt",
                "meshWatchAlt",
                null,
                "meshVisorAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = SubmarinerAssets.spyMonsoonMat;
            masterySkin.rendererInfos[1].defaultMaterial = SubmarinerAssets.spyMonsoonMat;
            masterySkin.rendererInfos[2].defaultMaterial = SubmarinerAssets.spyMonsoonMat;
            masterySkin.rendererInfos[3].defaultMaterial = SubmarinerAssets.spyMonsoonMat;
            masterySkin.rendererInfos[5].defaultMaterial = SubmarinerAssets.spyVisorMonsoonMat;

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("Tie"),
                    shouldActivate = false,
                }
            };
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            skins.Add(masterySkin);

            #endregion
            */
            skinController.skins = skins.ToArray();
        }
        #endregion skins


        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            SubmarinerAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

            if (SubmarinerPlugin.emotesInstalled) Emotes();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Emotes()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                var skele = SubmarinerAssets.mainAssetBundle.LoadAsset<GameObject>("submariner_emoteskeleton");
                CustomEmotesAPI.ImportArmature(SubmarinerSurvivor.characterPrefab, skele);
            };
        }


        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("SubmarinerBody"))
            {
                foreach (LanguageTextMeshController i in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>())
                {
                    if (i && i.token == "LOADOUT_SKILL_MISC") i.token = "Passive";
                }
            }
        }
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if(self.baseNameToken == "KENKO_SUBMARINER_NAME")
                {
                    SubmarinerController sController = self.gameObject.GetComponent<SubmarinerController>();
                    if(sController)
                    {
                        if (self.HasBuff(SubmarinerBuffs.SubmarinerRegenBuff))
                        {
                            self.regen *= 1.1f * self.GetBuffCount(SubmarinerBuffs.SubmarinerRegenBuff);
                        }

                        self.moveSpeed *= sController.movementSpeedAnchorIncrease;
                    }

                    if(self.HasBuff(SubmarinerBuffs.SubmarinerBeastBuff))
                    {
                        self.moveSpeed *= 1f + 0.1f * self.GetBuffCount(SubmarinerBuffs.SubmarinerBeastBuff);
                        self.damage += 2.5f * self.GetBuffCount(SubmarinerBuffs.SubmarinerBeastBuff);
                    }
                }
            }
        }
        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            CharacterBody attackerBody = damageReport.attackerBody;
            if (attackerBody && damageReport.attackerMaster && damageReport.victim)
            {
                if(attackerBody.baseNameToken == "KENKO_SUBMARINER_NAME")
                {
                    if(attackerBody.HasBuff(SubmarinerBuffs.SubmarinerBeastBuff))
                    {
                        attackerBody.AddBuff(SubmarinerBuffs.SubmarinerBeastBuff);
                        attackerBody.RecalculateStats();
                    }
                }
            }
        }
    }
}