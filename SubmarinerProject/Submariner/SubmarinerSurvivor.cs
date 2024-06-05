using BepInEx.Configuration;
using InterrogatorMod.Modules;
using InterrogatorMod.Modules.Characters;
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
using InterrogatorMod.Interrogator.Components;
using InterrogatorMod.Interrogator.Content;
using InterrogatorMod.Interrogator.SkillStates;
using HG;
using EntityStates;
using R2API.Networking.Interfaces;
using EmotesAPI;
using System.Runtime.CompilerServices;

namespace InterrogatorMod.Interrogator
{
    public class SubmarinerSurvivor : SurvivorBase<SubmarinerSurvivor>
    {
        public override string assetBundleName => "interrogator";
        public override string bodyName => "InterrogatorBody";
        public override string masterName => "InterrogatorMonsterMaster";
        public override string modelPrefabName => "mdlInterrogator";
        public override string displayPrefabName => "InterrogatorDisplay";

        public const string INTERROGATOR_PREFIX = SubmarinerPlugin.DEVELOPER_PREFIX + "_INTERROGATOR_";
        public override string survivorTokenPrefix => INTERROGATOR_PREFIX;

        internal static GameObject characterPrefab;

        public static SkillDef convictScepterSkillDef;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = INTERROGATOR_PREFIX + "NAME",
            subtitleNameToken = INTERROGATOR_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texInterrogatorIcon"),
            bodyColor = SubmarinerAssets.interrogatorColor,
            sortPosition = 5.99f,

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 160f,
            healthRegen = 2.5f,
            regenGrowth = 2.5f * 0.2f,
            armor = 20f,
            damage = 12f,

            damageGrowth = 2.4f,
            healthGrowth = 160f * 0.3f,


            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Model",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "MeleeModel",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "CleaverModel",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "JacketModel",
                    dontHotpoo = true,
                },
                new CustomRendererInfo
                {
                    childName = "VisorModel",
                    dontHotpoo = true,
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

            SubmarinerAssets.InitAssets();

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
            bool tempAdd(CharacterBody body) => body.HasBuff(SubmarinerBuffs.interrogatorConvictBuff);
            bool tempAdd2(CharacterBody body) => body.HasBuff(SubmarinerBuffs.interrogatorGuiltyDebuff);
            float pee(CharacterBody body) => 2f * body.radius;
            bodyPrefab.AddComponent<SubmarinerController>();
            bodyPrefab.AddComponent<SubmarinerTracker>();
            TempVisualEffectAPI.AddTemporaryVisualEffect(SubmarinerAssets.interrogatorConvicted, pee, tempAdd);
            TempVisualEffectAPI.AddTemporaryVisualEffect(SubmarinerAssets.interrogatorGuilty, pee, tempAdd2);
        }
        public void AddHitboxes()
        {
            Prefabs.SetupHitBoxGroup(characterModelObject, "MeleeHitbox", "MeleeHitbox");
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
            Prefabs.AddEntityStateMachine(bodyPrefab, "Interrogate");
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
            if (SubmarinerPlugin.scepterInstalled) InitializeScepter();
        }

        private void AddPassiveSkills()
        {
            SubmarinerPassive passive = bodyPrefab.GetComponent<SubmarinerPassive>();

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;

            passive.interrogatorPassive = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = INTERROGATOR_PREFIX + "PASSIVE_NAME",
                skillNameToken = INTERROGATOR_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = INTERROGATOR_PREFIX + "PASSIVE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texInterrogatorPassive"),
                keywordTokens = new string[] { Tokens.interrogatorGuiltyKeyword },
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

            Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, passive.interrogatorPassive);
        }

        private void AddPrimarySkills()
        {
            SteppedSkillDef batSkillDef = Skills.CreateSkillDef<SteppedSkillDef>(new SkillDefInfo
                (
                    "Brutal Bash",
                    INTERROGATOR_PREFIX + "PRIMARY_SWING_NAME",
                    INTERROGATOR_PREFIX + "PRIMARY_SWING_DESCRIPTION",
                    assetBundle.LoadAsset<Sprite>("texSwingIcon"),
                    new SerializableEntityStateType(typeof(SkillStates.Swing)),
                    "Weapon"
                ));
            batSkillDef.stepCount = 2;
            batSkillDef.stepGraceDuration = 1f;
            batSkillDef.keywordTokens = new string[]{ };

            Skills.AddPrimarySkills(bodyPrefab, batSkillDef);
        }

        private void AddSecondarySkills()
        {
            SkillDef Cleaver = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Affray",
                skillNameToken = INTERROGATOR_PREFIX + "SECONDARY_AFFRAY_NAME",
                skillDescriptionToken = INTERROGATOR_PREFIX + "SECONDARY_AFFRAY_DESCRIPTION",
                keywordTokens = new string[] { Tokens.interrogatorPressuredKeyword, Tokens.slayerKeyword },
                skillIcon = assetBundle.LoadAsset<Sprite>("texInterrogatorCleaverIcon"),

                activationState = new SerializableEntityStateType(typeof(ThrowCleaver)),

                activationStateMachineName = "Weapon2",
                interruptPriority = InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 5f,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                beginSkillCooldownOnSkillEnd = true,
                mustKeyPress = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, Cleaver);
        }

        private void AddUtilitySkills()
        {
            SkillDef dash = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Falsify",
                skillNameToken = INTERROGATOR_PREFIX + "UTILITY_FALSIFY_NAME",
                skillDescriptionToken = INTERROGATOR_PREFIX + "UTILITY_FALSIFY_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texFalsifyIcon"),

                activationState = new SerializableEntityStateType(typeof(Falsify)),
                activationStateMachineName = "Weapon2",
                interruptPriority = InterruptPriority.Skill,

                baseRechargeInterval = 6f,
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

            Skills.AddUtilitySkills(bodyPrefab, dash);
        }

        private void AddSpecialSkills()
        {
            SkillDef convict = Skills.CreateSkillDef<SubmarinerSkillDef>(new SkillDefInfo
            {
                skillName = "Convict",
                skillNameToken = INTERROGATOR_PREFIX + "SPECIAL_CONVICT_NAME",
                skillDescriptionToken = INTERROGATOR_PREFIX + "SPECIAL_CONVICT_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texConvictIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Convict)),
                activationStateMachineName = "Interrogate",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 16f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSpecialSkills(bodyPrefab, convict);
        }

        private void InitializeScepter()
        {
            convictScepterSkillDef = Skills.CreateSkillDef<SubmarinerSkillDef>(new SkillDefInfo
            {
                skillName = "Convict Scepter",
                skillNameToken = INTERROGATOR_PREFIX + "SPECIAL_SCEPTER_CONVICT_NAME",
                skillDescriptionToken = INTERROGATOR_PREFIX + "SPECIAL_SCEPTER_CONVICT_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texConvictScepter"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(ConvictScepter)),
                activationStateMachineName = "Interrogate",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 16f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(convictScepterSkillDef, bodyName, SkillSlot.Special, 0);
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
                "meshInterrogator",
                "meshBat",
                "meshCleaver",
                "meshJacket",
                "meshVisor");

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
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(INTERROGATOR_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texMonsoonSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                InterrogatorUnlockables.masterySkinUnlockableDef);

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
            masterySkin.rendererInfos[0].defaultMaterial = InterrogatorAssets.spyMonsoonMat;
            masterySkin.rendererInfos[1].defaultMaterial = InterrogatorAssets.spyMonsoonMat;
            masterySkin.rendererInfos[2].defaultMaterial = InterrogatorAssets.spyMonsoonMat;
            masterySkin.rendererInfos[3].defaultMaterial = InterrogatorAssets.spyMonsoonMat;
            masterySkin.rendererInfos[5].defaultMaterial = InterrogatorAssets.spyVisorMonsoonMat;

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
            //HUD.onHudTargetChangedGlobal += HUDSetup;
            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
            On.RoR2.HealthComponent.TakeDamage += new On.RoR2.HealthComponent.hook_TakeDamage(HealthComponent_TakeDamage);
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;

            if(SubmarinerPlugin.emotesInstalled) Emotes();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void Emotes()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                var skele = SubmarinerAssets.mainAssetBundle.LoadAsset<GameObject>("interrogator_emoteskeleton");
                CustomEmotesAPI.ImportArmature(SubmarinerSurvivor.characterPrefab, skele);
            };
        }


        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("InterrogatorBody"))
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
                if(self.baseNameToken == "KENKO_INTERROGATOR_NAME")
                {
                    SubmarinerController iController = self.gameObject.GetComponent<SubmarinerController>();
                    if(iController)
                    {
                        if (self.HasBuff(SubmarinerBuffs.interrogatorGuiltyBuff))
                        {
                            for(int i = 0; i < self.GetBuffCount(SubmarinerBuffs.interrogatorGuiltyBuff); i++)
                            {
                                self.attackSpeed += 0.15f;
                                self.damage += 0.5f;
                            }
                        }
                    }
                }
                if(self.HasBuff(SubmarinerBuffs.interrogatorPressuredBuff))
                {
                    self.attackSpeed *= 1.15f;
                    self.moveSpeed *= 1.15f;
                    self.armor *= 0.9f;
                    self.damage *= 0.9f;
                }
            }
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active && self.alive || !self.godMode || self.ospTimer <= 0f)
            {
                CharacterBody victimBody = self.body;
                CharacterBody attackerBody = null;

                if (damageInfo.attacker)
                {
                    attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                }

                if (damageInfo.damage > 0 && !damageInfo.rejected && victimBody && attackerBody)
                {
                    if(victimBody.HasBuff(SubmarinerBuffs.interrogatorConvictBuff) && !attackerBody.HasBuff(SubmarinerBuffs.interrogatorConvictBuff)
                        || !victimBody.HasBuff(SubmarinerBuffs.interrogatorConvictBuff) && attackerBody.HasBuff(SubmarinerBuffs.interrogatorConvictBuff))
                    {
                        if (attackerBody.baseNameToken == "KENKO_INTERROGATOR_NAME" &&
                            attackerBody.skillLocator.special.skillNameToken == INTERROGATOR_PREFIX + "SPECIAL_SCEPTER_CONVICT_NAME")
                        {
                            damageInfo.damage *= 0.25f;
                        }
                        else damageInfo.rejected = true;
                    }

                    if (victimBody.baseNameToken == "KENKO_INTERROGATOR_NAME")
                    {
                        SubmarinerController iController = victimBody.GetComponent<SubmarinerController>();
                        if (iController)
                        {
                            if (attackerBody)
                            {
                                if (attackerBody.teamComponent.teamIndex == victimBody.teamComponent.teamIndex)
                                {
                                    damageInfo.damage *= 0.25f;
                                    if (attackerBody.HasBuff(SubmarinerBuffs.interrogatorGuiltyDebuff)) attackerBody.RemoveOldestTimedBuff(SubmarinerBuffs.interrogatorGuiltyDebuff);
                                    attackerBody.AddTimedBuff(SubmarinerBuffs.interrogatorGuiltyDebuff, SubmarinerStaticValues.baseConvictTimerMax);
                                }
                                else
                                {
                                    if (attackerBody.HasBuff(SubmarinerBuffs.interrogatorGuiltyDebuff)) attackerBody.RemoveBuff(SubmarinerBuffs.interrogatorGuiltyDebuff);
                                    attackerBody.AddBuff(SubmarinerBuffs.interrogatorGuiltyDebuff);
                                }
                            }
                        }
                    }
                    else if (attackerBody.baseNameToken == "KENKO_INTERROGATOR_NAME")
                    {
                        SubmarinerController iController = attackerBody.GetComponent<SubmarinerController>();
                        if (iController)
                        {
                            if (attackerBody.teamComponent.teamIndex == victimBody.teamComponent.teamIndex)
                            {
                                damageInfo.damage *= 0.25f;
                            }
                        }
                    }
                }
            }

            orig.Invoke(self, damageInfo);
        }
        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            CharacterBody attackerBody = damageReport.attackerBody;
            if (attackerBody && damageReport.attackerMaster && damageReport.victim)
            {
                if(attackerBody.baseNameToken == "KENKO_INTERROGATOR_NAME" && damageReport.damageInfo.HasModdedDamageType(DamageTypes.InterrogatorPressure))
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.attacker = damageReport.attacker;
                    blastAttack.inflictor = damageReport.attacker;
                    blastAttack.teamIndex = damageReport.attackerTeamIndex;
                    blastAttack.baseDamage = 0.1f;
                    blastAttack.baseForce = 0f;
                    blastAttack.position = damageReport.victim.body.corePosition;
                    blastAttack.radius = 12f;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                    blastAttack.bonusForce = Vector3.zero;
                    blastAttack.damageType = DamageType.AOE;
                    blastAttack.AddModdedDamageType(DamageTypes.InterrogatorPressureBleed);
                    blastAttack.Fire();

                    if (damageReport.victim.gameObject.TryGetComponent<NetworkIdentity>(out var identity))
                    {
                        new SyncBloodExplosion(identity.netId, damageReport.victim.gameObject).Send(NetworkDestination.Clients);
                    }
                }
            }
        }
        internal static void HUDSetup(HUD hud)
        {
            /*
            if (hud.targetBodyObject && hud.targetMaster && hud.targetMaster.bodyPrefab == InterrogatorSurvivor.characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;
                Transform skillsContainer = hud.equipmentIcons[0].gameObject.transform.parent;

                // ammo display for atomic
                Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                GameObject stealthTracker = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                stealthTracker.name = "AmmoTracker";
                stealthTracker.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                GameObject.DestroyImmediate(stealthTracker.transform.GetChild(0).gameObject);
                MonoBehaviour.Destroy(stealthTracker.GetComponentInChildren<LevelText>());
                MonoBehaviour.Destroy(stealthTracker.GetComponentInChildren<ExpBar>());

                stealthTracker.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);
                GameObject.DestroyImmediate(stealthTracker.transform.Find("ExpBarRoot").gameObject);

                stealthTracker.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                RectTransform rect = stealthTracker.GetComponent<RectTransform>();
                rect.localScale = new Vector3(0.8f, 0.8f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.offsetMin = new Vector2(120f, -40f);
                rect.offsetMax = new Vector2(120f, -40f);
                rect.pivot = new Vector2(0.5f, 0f);
                //positional data doesnt get sent to clients? Manually making offsets works..
                rect.anchoredPosition = new Vector2(50f, 0f);
                rect.localPosition = new Vector3(120f, -40f, 0f);

                GameObject chargeBarAmmo = GameObject.Instantiate(InterrogatorAssets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
                chargeBarAmmo.name = "StealthMeter";
                chargeBarAmmo.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                rect = chargeBarAmmo.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                rect.anchorMin = new Vector2(100f, 2f);
                rect.anchorMax = new Vector2(100f, 2f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(100f, 2f);
                rect.localPosition = new Vector3(100f, 2f, 0f);
                rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                ConvictHudController stealthComponent = stealthTracker.AddComponent<ConvictHudController>();

                stealthComponent.targetHUD = hud;
                stealthComponent.targetText = stealthTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();
                stealthComponent.durationDisplay = chargeBarAmmo;
                stealthComponent.durationBar = chargeBarAmmo.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                stealthComponent.durationBarColor = chargeBarAmmo.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
            }
            */
        }
    }
}