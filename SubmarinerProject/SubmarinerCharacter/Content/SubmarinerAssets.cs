using RoR2;
using UnityEngine;
using SubmarinerMod.Modules;
using RoR2.Projectile;
using RoR2.UI;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API;
using UnityEngine.Rendering.PostProcessing;
using ThreeEyedGames;
using SubmarinerMod.SubmarinerCharacter.Components;
using SubmarinerMod.SubmarinerCharacter.SkillStates;
using System;
using System.Linq;
using Rewired.ComponentControls.Effects;
using System.IO;
using System.Reflection;

namespace SubmarinerMod.SubmarinerCharacter.Content
{
    public static class SubmarinerAssets
    {
        //AssetBundle
        internal static AssetBundle mainAssetBundle;

        //Materials
        internal static Material commandoMat;
        internal static Material anchorMat;
        internal static Material ghostMat;

        //Shader
        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");

        //Effects
        internal static GameObject regenerativeEffect;

        internal static GameObject anchorLandingEffect;

        internal static GameObject submarinerSwingEffect;
        internal static GameObject submarinerHitEffect;

        internal static GameObject dashEffect;

        internal static GameObject throwable;
        internal static GameObject throwableEnd;

        internal static GameObject anchorTether;
        //Models
        //Projectiles
        internal static GameObject hookPrefab;
        internal static GameObject minePrefab;
        internal static GameObject mineExplosionPrefab;
        internal static GameObject anchorPrefab;
        //Sounds
        internal static NetworkSoundEventDef impactSound;
        internal static NetworkSoundEventDef swordImpactSoundEvent;

        //Colors
        internal static Color SubmarinerColor = new Color(61f / 255f, 229f / 255f, 84f / 255f);
        internal static Color SubmarinerSecondaryColor = new Color(70f / 255f, 63f / 255f, 94f / 255f);

        //Crosshair
        public static void Init(AssetBundle assetBundle)
        {
            mainAssetBundle = assetBundle;

            CreateMaterials();

            CreateModels();

            CreateEffects();

            CreateSounds();

            CreateProjectiles();

            CreateUI();
        }

        private static void CleanChildren(Transform startingTrans)
        {
            for (int num = startingTrans.childCount - 1; num >= 0; num--)
            {
                if (startingTrans.GetChild(num).childCount > 0)
                {
                    CleanChildren(startingTrans.GetChild(num));
                }
                UnityEngine.Object.DestroyImmediate(startingTrans.GetChild(num).gameObject);
            }
        }

        private static void CreateMaterials()
        {
            anchorMat = mainAssetBundle.LoadAsset<Material>("matSubmariner");
            ghostMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matGhostEffect.mat").WaitForCompletion();
        }

        private static void CreateModels()
        {
        }
        #region effects
        private static void CreateEffects()
        {
            anchorLandingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegacrabAntimatterExplosionSimple.prefab").WaitForCompletion().InstantiateClone("SubmarinerAnchorLandingEffect", false);
            if (!anchorLandingEffect.GetComponent<EffectComponent>()) anchorLandingEffect.AddComponent<EffectComponent>();
            var ec = anchorLandingEffect.GetComponent<EffectComponent>();

            ec.applyScale = true;
            ec.soundName = "Play_voidDevastator_m2_secondary_explo";

            Modules.Content.CreateAndAddEffectDef(anchorLandingEffect);

            regenerativeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoRegenEffect.prefab").WaitForCompletion().InstantiateClone("SubmarinerRegenerativeEffect", false);

            throwable = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BasicThrowableVisualizer.prefab").WaitForCompletion();
            throwableEnd = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRainIndicator.prefab").WaitForCompletion();

            anchorTether = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/EntangleOrbEffect.prefab").WaitForCompletion().InstantiateClone("SubmarinerChains");
            anchorTether.AddComponent<NetworkIdentity>();
            Material[] hi = new Material[1];
            hi[0] = Addressables.LoadAssetAsync<Material>("RoR2/Base/Gravekeeper/matGravekeeperHookChain.mat").WaitForCompletion();
            anchorTether.transform.GetChild(0).GetComponent<LineRenderer>().materials = hi;
            anchorTether.transform.GetChild(0).GetComponent<LineRenderer>().textureMode = LineTextureMode.Tile;
            anchorTether.transform.localScale *= 0.5f;
            UnityEngine.Object.Destroy(anchorTether.transform.GetChild(0).GetChild(0).gameObject);
            anchorTether.gameObject.GetComponent<AkEvent>().enabled = false;
            anchorTether.gameObject.GetComponent<AkGameObj>().enabled = false;
            anchorTether.gameObject.AddComponent<DestroyOnCondition>();
            Modules.Content.CreateAndAddEffectDef(anchorTether);

            dashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherDashEffect.prefab").WaitForCompletion().InstantiateClone("SubmarinerDashEffect");
            dashEffect.AddComponent<NetworkIdentity>();
            UnityEngine.Object.Destroy(dashEffect.transform.Find("Point light").gameObject);
            UnityEngine.Object.Destroy(dashEffect.transform.Find("Flash, White").gameObject);
            UnityEngine.Object.Destroy(dashEffect.transform.Find("NoiseTrails").gameObject);
            dashEffect.transform.Find("Donut").localScale *= 0.5f;
            dashEffect.transform.Find("Donut, Distortion").localScale *= 0.5f;
            dashEffect.transform.Find("Dash").GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampDefault.png").WaitForCompletion());
            dashEffect.transform.Find("Dash").GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", SubmarinerColor);
            Modules.Content.CreateAndAddEffectDef(dashEffect);

            submarinerHitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit.prefab").WaitForCompletion().InstantiateClone("SubmarinerHitEffect");
            submarinerHitEffect.AddComponent<NetworkIdentity>();
            Modules.Content.CreateAndAddEffectDef(submarinerHitEffect);

            submarinerSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("SubmarinerSwing", false);
            submarinerSwingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion();
            var swing = submarinerSwingEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            swing.startLifetimeMultiplier *= 2f;
        }

        #endregion

        #region projectiles
        private static void CreateProjectiles()
        {
            hookPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderYankHook.prefab").WaitForCompletion().InstantiateClone("SubmarinerHarpoon");
            if(!hookPrefab.GetComponent<NetworkIdentity>())hookPrefab.AddComponent<NetworkIdentity>();
            ProjectileGrappleController harpoon = hookPrefab.GetComponent<ProjectileGrappleController>();
            harpoon.ownerHookStateType = new EntityStates.SerializableEntityStateType(typeof(HarpoonShot));
            harpoon.maxTravelDistance = 120f;
            harpoon.lookAcceleration = 0f;
            harpoon.moveAcceleration = 0f;
            harpoon.muzzleStringOnBody = "HandL";
            harpoon.minHookDistancePitchModifier = 0f;
            harpoon.maxHookDistancePitchModifier = 60f;
            harpoon.nearBreakDistance = 0f;

            hookPrefab.transform.Find("FistMesh").gameObject.GetComponent<MeshRenderer>().materials = new Material[1];
            hookPrefab.transform.Find("FistMesh").gameObject.GetComponent<MeshRenderer>().materials[0] = anchorMat;
            hookPrefab.transform.Find("FistMesh").gameObject.GetComponent<MeshFilter>().mesh = mainAssetBundle.LoadAsset<Mesh>("meshHarpoonProjectile");
            hookPrefab.transform.Find("FistMesh").rotation = new Quaternion(90f, Quaternion.identity.x, Quaternion.identity.z, Quaternion.identity.w);
            hookPrefab.transform.Find("FistMesh").Find("RopeFront").gameObject.GetComponent<LineRenderer>().material.SetColor("_TintColor", new Color(61f / 255f, 229f / 255f, 84f / 255f));
            UnityEngine.Object.Destroy(hookPrefab.transform.Find("FistMesh").Find("RopeFront").Find("Dust").gameObject);
            UnityEngine.Object.Destroy(hookPrefab.transform.Find("FistMesh").Find("RopeFront").Find("Sparks, Fast").gameObject);
            UnityEngine.Object.Destroy(hookPrefab.transform.Find("FistMesh").Find("RopeFront").Find("Point Light").gameObject);
            UnityEngine.Object.Destroy(hookPrefab.transform.Find("RopeEnd").Find("Dust").gameObject);
            UnityEngine.Object.Destroy(hookPrefab.transform.Find("RopeEnd").Find("Sparks, Fast").gameObject);
            UnityEngine.Object.Destroy(hookPrefab.transform.Find("RopeEnd").Find("Point Light").gameObject);

            hookPrefab.GetComponent<ProjectileStickOnImpact>().ignoreWorld = true;
            hookPrefab.GetComponent<ProjectileDamage>().damageType.damageSource = DamageSource.Secondary;

            Modules.Content.AddProjectilePrefab(hookPrefab);

            minePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMine.prefab").WaitForCompletion().InstantiateClone("SubmarinerMine");
            minePrefab.gameObject.GetComponent<ProjectileController>().ghostPrefab = PrefabAPI.InstantiateClone(minePrefab.gameObject.GetComponent<ProjectileController>().ghostPrefab, "SubmarinerMineGhost");
            GameObject ghost = minePrefab.gameObject.GetComponent<ProjectileController>().ghostPrefab;
            ghost.GetComponent<EngiMineAnimator>().enabled = false;

            mineExplosionPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMineExplosion.prefab").WaitForCompletion().InstantiateClone("SubmarinerMineExplosion");
            mineExplosionPrefab.GetComponent<EffectComponent>().soundName = "Play_acrid_shift_land";

            Modules.Content.CreateAndAddEffectDef(mineExplosionPrefab);

            ProjectileImpactExplosion boom = minePrefab.AddComponent<ProjectileImpactExplosion>();
            boom.blastDamageCoefficient = 1f;
            boom.blastProcCoefficient = 1f;
            boom.blastRadius = 12f;
            boom.canRejectForce = true;
            boom.fireChildren = false;
            boom.destroyOnEnemy = true;
            boom.destroyOnWorld = false;
            boom.impactOnWorld = false;
            boom.lifetime = 8f;
            boom.lifetimeAfterImpact = 0.3f;
            boom.impactEffect = mineExplosionPrefab;

            var pd = minePrefab.GetComponent<ProjectileDamage>();
            pd.damageType.damageSource = DamageSource.Utility;
            MeshFilter meshF = ghost.transform.Find("mdlEngiMine").Find("EngiMineMesh").gameObject.AddComponent<MeshFilter>();
            meshF.mesh = mainAssetBundle.LoadAsset<Mesh>("meshMine");
            MeshRenderer meshR = ghost.transform.Find("mdlEngiMine").Find("EngiMineMesh").gameObject.AddComponent<MeshRenderer>();
            meshR.materials = new Material[1];
            meshR.materials[0] = anchorMat;
            meshR.material = anchorMat;

            Component.DestroyImmediate(ghost.transform.Find("mdlEngiMine").Find("EngiMineMesh").gameObject.GetComponent<SkinnedMeshRenderer>());
            ghost.transform.Find("mdlEngiMine").Find("EngiMineArmature").gameObject.SetActive(false);

            Modules.Content.AddProjectilePrefab(minePrefab);

            anchorPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/CryoCanisterProjectile.prefab").WaitForCompletion().InstantiateClone("SubmarinerAnchor");
            if (!anchorPrefab.GetComponent<NetworkIdentity>()) anchorPrefab.AddComponent<NetworkIdentity>();

            anchorPrefab.gameObject.GetComponent<ProjectileSimple>().lifetime = 16f;
            anchorPrefab.gameObject.GetComponent<ProjectileSimple>().desiredForwardSpeed = 30f;

            GameObject ghost2 = mainAssetBundle.LoadAsset<GameObject>("mdlAnchor");
            ghost2.transform.localScale *= 2f;
            if (!ghost2.GetComponent<NetworkIdentity>()) ghost2.AddComponent<NetworkIdentity>();
            if (!ghost2.GetComponent<ProjectileGhostController>()) ghost2.AddComponent<ProjectileGhostController>();

            anchorPrefab.gameObject.GetComponent<ProjectileController>().ghostPrefab = ghost2;

            GameObject modelTransform = new GameObject();
            modelTransform.name = "AnchorGhostTransform";
            modelTransform.transform.localScale = Vector3.one * 0.75f;
            modelTransform.transform.SetParent(anchorPrefab.transform, false);
            modelTransform.transform.localScale *= 2f;
            anchorPrefab.gameObject.GetComponent<ProjectileController>().ghostTransformAnchor = anchorPrefab.transform.Find("AnchorGhostTransform");

            SubmarinerStickOnImpact stick = anchorPrefab.AddComponent<SubmarinerStickOnImpact>();
            stick.stickSoundString = "Play_parent_attack1_slam";
            stick.ignoreCharacters = true;
            stick.ignoreWorld = false;
            stick.alignNormals = true;
            stick.alignRotationPlease = ghost2.transform.rotation;
            stick.alignLocationPlease = ghost2.transform.position;
            stick.impactEffect = anchorLandingEffect;

            Component.Destroy(anchorPrefab.GetComponent<ProjectileImpactExplosion>());
            Component.Destroy(anchorPrefab.GetComponent<ApplyTorqueOnStart>());
            pd = anchorPrefab.GetComponent<ProjectileDamage>();
            pd.damageType.damageSource = DamageSource.Special;
            pd.damageType = DamageType.Stun1s;

            /*

            Prefabs.AddEntityStateMachine(bodyPrefab, "Main", typeof(SkillStates.AnchorBaseState), typeof(SkillStates.AnchorTetherBehaviour));

            */

            AnchorConnectionComponent c = anchorPrefab.AddComponent<AnchorConnectionComponent>();
            c.enabled = false;
            Modules.Content.AddProjectilePrefab(anchorPrefab);
        }
        #endregion

        #region sounds
        private static void CreateSounds()
        {
            LoadSoundbank();

            impactSound = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_bellBody_attackLand");
            swordImpactSoundEvent = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_merc_sword_impact");
        }
        #endregion
        internal static void LoadSoundbank()
        {
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("SubmarinerMod.submariner_bank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }
        private static void CreateUI()
        {
        }

        #region helpers
        private static GameObject CreateImpactExplosionEffect(string effectName, Material bloodMat, Material decal, float scale = 1f)
        {
            GameObject newEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone(effectName, true);

            newEffect.transform.Find("Spikes, Small").gameObject.SetActive(false);

            newEffect.transform.Find("PP").gameObject.SetActive(false);
            newEffect.transform.Find("Point light").gameObject.SetActive(false);
            newEffect.transform.Find("Flash Lines").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLargeDirectional.mat").WaitForCompletion();

            newEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.GetChild(6).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;

            var boom = newEffect.transform.Find("Fire").GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.5f;
            boom = newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.3f;
            boom = newEffect.transform.GetChild(6).GetComponent<ParticleSystem>().main;
            boom.startLifetimeMultiplier = 0.4f;

            newEffect.transform.Find("Physics").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/MagmaWorm/matFracturedGround.mat").WaitForCompletion();

            newEffect.transform.Find("Decal").GetComponent<Decal>().Material = decal;
            newEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;

            newEffect.transform.Find("FoamSplash").gameObject.SetActive(false);
            newEffect.transform.Find("FoamBilllboard").gameObject.SetActive(false);
            newEffect.transform.Find("Dust").gameObject.SetActive(false);
            newEffect.transform.Find("Dust, Directional").gameObject.SetActive(false);

            newEffect.transform.localScale = Vector3.one * scale;

            newEffect.AddComponent<NetworkIdentity>();

            ParticleSystemColorFromEffectData PSCFED = newEffect.AddComponent<ParticleSystemColorFromEffectData>();
            PSCFED.particleSystems = new ParticleSystem[]
            {
                newEffect.transform.Find("Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(6).GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(3).GetComponent<ParticleSystem>()
            };
            PSCFED.effectComponent = newEffect.GetComponent<EffectComponent>();

            SubmarinerMod.Modules.Content.CreateAndAddEffectDef(newEffect);

            return newEffect;
        }
        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return commandoMat;

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return CreateMaterial(materialName, emission, Color.black);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return CreateMaterial(materialName, emission, emissionColor, 0f);
        }
        #endregion
    }
}