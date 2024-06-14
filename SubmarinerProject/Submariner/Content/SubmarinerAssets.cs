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
using SubmarinerMod.Submariner.Components;
using SubmarinerMod.Submariner.SkillStates;

namespace SubmarinerMod.Submariner.Content
{
    public static class SubmarinerAssets
    {
        //AssetBundle
        internal static AssetBundle mainAssetBundle;

        //Materials
        internal static Material commandoMat;
        internal static Material anchorMat;

        //Shader
        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");

        //Effects
        internal static GameObject bloodSplatterEffect;
        internal static GameObject bloodExplosionEffect;
        internal static GameObject bloodSpurtEffect;

        internal static GameObject batSwingEffect;
        internal static GameObject swordSwingEffect;
        internal static GameObject batHitEffect;

        internal static GameObject batHitEffectRed;

        internal static GameObject dashEffect;

        internal static GameObject SubmarinerGuilty;
        internal static GameObject SubmarinerConvicted;
        internal static GameObject SubmarinerConvictedConsume;

        //Models
        //Projectiles
        internal static GameObject hookPrefab;
        //Sounds
        internal static NetworkSoundEventDef batImpactSoundEvent;
        internal static NetworkSoundEventDef swordImpactSoundEvent;

        //Colors
        internal static Color SubmarinerColor = new Color(61f / 255f, 229f / 255f, 84f / 255f);
        internal static Color SubmarinerSecondaryColor = new Color(70f / 255f, 63f / 255f, 94f / 255f);

        //Crosshair
        public static void Init(AssetBundle assetBundle)
        {
            mainAssetBundle = assetBundle;
        }
        public static void InitAssets()
        {
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
                Object.DestroyImmediate(startingTrans.GetChild(num).gameObject);
            }
        }

        private static void CreateMaterials()
        {
            anchorMat = mainAssetBundle.LoadAsset<Material>("matSubmariner");
        }

        private static void CreateModels()
        {
        }
        #region effects
        private static void CreateEffects()
        {
            bloodExplosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("DriverBloodExplosion", false);

            Material bloodMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
            Material bloodMat2 = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();

            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();
            bloodExplosionEffect.GetComponentInChildren<Light>().gameObject.SetActive(false);

            bloodExplosionEffect.GetComponentInChildren<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync <PostProcessProfile>("RoR2/Base/title/ppLocalGold.asset").WaitForCompletion();

            Modules.Content.CreateAndAddEffectDef(bloodExplosionEffect);

            bloodSpurtEffect = mainAssetBundle.LoadAsset<GameObject>("BloodSpurtEffect");

            bloodSpurtEffect.transform.Find("Blood").GetComponent<ParticleSystemRenderer>().material = bloodMat2;
            bloodSpurtEffect.transform.Find("Trails").GetComponent<ParticleSystemRenderer>().trailMaterial = bloodMat2;

            dashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherDashEffect.prefab").WaitForCompletion().InstantiateClone("SubmarinerDashEffect");
            dashEffect.AddComponent<NetworkIdentity>();
            Object.Destroy(dashEffect.transform.Find("Point light").gameObject);
            Object.Destroy(dashEffect.transform.Find("Flash, White").gameObject);
            Object.Destroy(dashEffect.transform.Find("NoiseTrails").gameObject);
            dashEffect.transform.Find("Donut").localScale *= 0.5f;
            dashEffect.transform.Find("Donut, Distortion").localScale *= 0.5f;
            dashEffect.transform.Find("Dash").GetComponent<ParticleSystemRenderer>().material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/ColorRamps/texRampDefault.png").WaitForCompletion());
            dashEffect.transform.Find("Dash").GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", SubmarinerColor);
            Modules.Content.CreateAndAddEffectDef(dashEffect);

            batHitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit.prefab").WaitForCompletion().InstantiateClone("InterreogatorBatHitEffect");
            batHitEffect.AddComponent<NetworkIdentity>();
            Modules.Content.CreateAndAddEffectDef(batHitEffect);

            batHitEffectRed = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/HitsparkBandit.prefab").WaitForCompletion().InstantiateClone("InterreogatorBatRedHitEffect");
            batHitEffectRed.AddComponent<NetworkIdentity>();
            batHitEffectRed.transform.Find("Particles").Find("TriangleSparksLarge").gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            batHitEffectRed.transform.Find("Particles").Find("TriangleSparks").gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);
            Modules.Content.CreateAndAddEffectDef(batHitEffectRed);

            batSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("SubmarinerBatSwing", false);
            batSwingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion();
            var swing = batSwingEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            swing.startLifetimeMultiplier *= 2f;

            swordSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("SubmarinerswordSwing", false);
            swordSwingEffect.transform.GetChild(0).localScale *= 1.5f;
            swordSwingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matGenericSwingTrail.mat").WaitForCompletion();
            swordSwingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", SubmarinerColor);
            swing = swordSwingEffect.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            swing.startLifetimeMultiplier *= 2f;

            bloodSplatterEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone("SubmarinerSplat", true);
            bloodSplatterEffect.AddComponent<NetworkIdentity>();
            bloodSplatterEffect.transform.GetChild(0).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(1).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(2).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(3).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(4).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(5).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(6).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(7).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(8).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(9).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(10).gameObject.SetActive(false);
            bloodSplatterEffect.transform.Find("Decal").GetComponent<Decal>().Material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion();
            bloodSplatterEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;
            bloodSplatterEffect.transform.GetChild(12).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(13).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(14).gameObject.SetActive(false);
            bloodSplatterEffect.transform.GetChild(15).gameObject.SetActive(false);
            bloodSplatterEffect.transform.localScale = Vector3.one;
            SubmarinerMod.Modules.Content.CreateAndAddEffectDef(bloodSplatterEffect);

            SubmarinerConvicted = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SlowOnHit/SlowDownTime.prefab").WaitForCompletion().InstantiateClone("Convicted", true);
            SubmarinerConvicted.AddComponent<NetworkIdentity>();
            SubmarinerConvicted.transform.Find("Visual").GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color(166f / 255f, 159f / 255f, 20f / 255f));
            SubmarinerConvicted.transform.Find("Visual").GetChild(1).gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_TintColor", new Color(166f / 255f, 159f / 255f, 20f / 255f));

            Material fakeMerc = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposed.mat").WaitForCompletion());
            fakeMerc.SetTexture("_MainTex", mainAssetBundle.LoadAsset<Texture>("texGuilty"));
            fakeMerc.SetColor("_TintColor", SubmarinerColor);
            SubmarinerGuilty = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercExposeEffect.prefab").WaitForCompletion().InstantiateClone("Guilty", true);
            SubmarinerGuilty.AddComponent<NetworkIdentity>();
            SubmarinerGuilty.transform.Find("Visual, On").Find("PulseEffect, Ring").gameObject.GetComponent<ParticleSystemRenderer>().material = fakeMerc;

            fakeMerc = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposed.mat").WaitForCompletion());
            fakeMerc.SetTexture("_MainTex", mainAssetBundle.LoadAsset<Texture>("texGuilty"));
            fakeMerc.SetColor("_TintColor", Color.red);
            SubmarinerConvictedConsume = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercExposeConsumeEffect.prefab").WaitForCompletion().InstantiateClone("ConvictMarked", true);
            SubmarinerConvictedConsume.AddComponent<NetworkIdentity>();
            SubmarinerConvictedConsume.transform.Find("Visual, Consumed").Find("PulseEffect, Ring (1)").gameObject.GetComponent<ParticleSystemRenderer>().material = fakeMerc;
            SubmarinerConvictedConsume.gameObject.GetComponent<EffectComponent>().soundName = "sfx_SUBMARINER_point";
            Object.Destroy(SubmarinerConvictedConsume.transform.Find("Visual, Consumed").Find("PulseEffect, Slash").gameObject);

            Modules.Content.CreateAndAddEffectDef(SubmarinerConvictedConsume);
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
            hookPrefab.transform.Find("FistMesh").rotation = new Quaternion(Quaternion.identity.x, -90f, Quaternion.identity.z, Quaternion.identity.w);
            hookPrefab.transform.Find("FistMesh").Find("RopeFront").gameObject.GetComponent<LineRenderer>().material.SetColor("_TintColor", new Color(61f / 255f, 229f / 255f, 84f / 255f));
            Object.Destroy(hookPrefab.transform.Find("FistMesh").Find("RopeFront").Find("Dust").gameObject);
            Object.Destroy(hookPrefab.transform.Find("FistMesh").Find("RopeFront").Find("Sparks, Fast").gameObject);
            Object.Destroy(hookPrefab.transform.Find("FistMesh").Find("RopeFront").Find("Point Light").gameObject);
            Object.Destroy(hookPrefab.transform.Find("RopeEnd").Find("Dust").gameObject);
            Object.Destroy(hookPrefab.transform.Find("RopeEnd").Find("Sparks, Fast").gameObject);
            Object.Destroy(hookPrefab.transform.Find("RopeEnd").Find("Point Light").gameObject);

            hookPrefab.GetComponent<ProjectileStickOnImpact>().ignoreWorld = true;

            Modules.Content.AddProjectilePrefab(hookPrefab);
        }
        #endregion

        #region sounds
        private static void CreateSounds()
        {
            batImpactSoundEvent = Modules.Content.CreateAndAddNetworkSoundEventDef("sfx_interrogator_self_damage");
            swordImpactSoundEvent = Modules.Content.CreateAndAddNetworkSoundEventDef("Play_merc_sword_impact");
        }
        #endregion

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