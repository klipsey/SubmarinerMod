﻿using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using RoR2.Projectile;
using RoR2;
using static UnityEngine.SendMouseEvents;
using R2API;
using SubmarinerMod.SubmarinerCharacter.Content;

namespace SubmarinerMod.SubmarinerCharacter.Components
{
    public class SubmarinerStickOnImpact : NetworkBehaviour, IProjectileImpactBehavior
    {
        public string stickSoundString;

        public ParticleSystem[] stickParticleSystem;

        public GameObject impactEffect;

        public bool ignoreCharacters;

        public bool ignoreWorld;

        public bool alignNormals = true;

        public UnityEvent stickEvent;

        private ProjectileController projectileController;

        private Rigidbody rigidbody;

        private bool wasEverEnabled;

        private GameObject _victim;

        private bool hasFired;

        [SyncVar]
        private GameObject syncVictim;

        [SyncVar]
        private sbyte hitHurtboxIndex = -1;

        [SyncVar]
        private Vector3 localPosition;

        [SyncVar]
        private Quaternion localRotation;

        [SyncVar]
        public Quaternion alignRotationPlease;

        [SyncVar]
        public Vector3 alignLocationPlease;

        private NetworkInstanceId ___syncVictimNetId;

        public Transform stuckTransform { get; private set; }

        public CharacterBody stuckBody { get; private set; }

        public GameObject victim
        {
            get
            {
                return _victim;
            }
            private set
            {
                _victim = value;
                NetworksyncVictim = value;
            }
        }

        public bool stuck => hitHurtboxIndex != -1;

        public GameObject NetworksyncVictim
        {
            get
            {
                return syncVictim;
            }
            [param: In]
            set
            {
                SetSyncVarGameObject(value, ref syncVictim, 1u, ref ___syncVictimNetId);
            }
        }

        public sbyte NetworkhitHurtboxIndex
        {
            get
            {
                return hitHurtboxIndex;
            }
            [param: In]
            set
            {
                SetSyncVar(value, ref hitHurtboxIndex, 2u);
            }
        }

        public Vector3 NetworklocalPosition
        {
            get
            {
                return localPosition;
            }
            [param: In]
            set
            {
                SetSyncVar(value, ref localPosition, 4u);
            }
        }

        public Quaternion NetworklocalRotation
        {
            get
            {
                return localRotation;
            }
            [param: In]
            set
            {
                SetSyncVar(value, ref localRotation, 8u);
            }
        }

        private void Awake()
        {
            projectileController = GetComponent<ProjectileController>();
            rigidbody = GetComponent<Rigidbody>();
        }

        public void FixedUpdate()
        {
            UpdateSticking();
        }

        private void OnEnable()
        {
            if (wasEverEnabled)
            {
                Collider component = GetComponent<Collider>();
                component.enabled = false;
                component.enabled = true;
            }
            wasEverEnabled = true;
        }

        private void OnDisable()
        {
            if (NetworkServer.active)
            {
                Detach();
            }
        }

        [Server]
        public void Detach()
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.Projectile.ProjectileStickOnImpact::Detach()' called on client");
                return;
            }
            victim = null;
            stuckTransform = null;
            NetworkhitHurtboxIndex = -1;
            UpdateSticking();
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (base.enabled)
            {
                base.transform.Find("AnchorGhostTransform").localPosition = alignLocationPlease;
                base.transform.Find("AnchorGhostTransform").localRotation = alignRotationPlease;
                TrySticking(impactInfo.collider, impactInfo.estimatedImpactNormal);
                if (!hasFired)
                {
                    Vector3 effectPos = this.transform.localPosition;
                    RaycastHit raycastHit;
                    if (Physics.Raycast(effectPos, Vector3.one, out raycastHit, 10f, LayerIndex.world.mask))
                    {
                        effectPos = raycastHit.point;
                    }
                    if (impactEffect)
                    {
                        EffectManager.SpawnEffect(impactEffect, new EffectData
                        {
                            origin = effectPos,
                            rotation = Quaternion.identity,
                            scale = 1.5f,
                        }, true);
                    }
                    BlastAttack impactAttack = new BlastAttack();
                    GameObject owner = gameObject.GetComponent<ProjectileController>().owner;
                    impactAttack.attacker = owner;
                    impactAttack.inflictor = owner;
                    impactAttack.teamIndex = TeamComponent.GetObjectTeam(owner);
                    impactAttack.baseDamage = SubmarinerStaticValues.anchorDamageCoefficient * owner.GetComponent<CharacterBody>().damage;
                    impactAttack.baseForce = 600f;
                    impactAttack.position = transform.position;
                    impactAttack.procCoefficient = 1f;
                    impactAttack.radius = 10f;
                    impactAttack.damageType = DamageType.Stun1s | DamageType.AOE;
                    impactAttack.falloffModel = BlastAttack.FalloffModel.None;
                    impactAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                    impactAttack.Fire();
                    hasFired = true;
                }
            }
        }

        private bool TrySticking(Collider hitCollider, Vector3 impactNormal)
        {
            if (victim)
            {
                return false;
            }
            GameObject gameObject = null;
            sbyte networkhitHurtboxIndex = -1;
            HurtBox component = hitCollider.GetComponent<HurtBox>();
            if (component)
            {
                HealthComponent healthComponent = component.healthComponent;
                if (healthComponent)
                {
                    gameObject = healthComponent.gameObject;
                }
                networkhitHurtboxIndex = (sbyte)component.indexInGroup;
            }
            if (!gameObject && !ignoreWorld)
            {
                gameObject = hitCollider.gameObject;
                networkhitHurtboxIndex = -2;
            }
            if (gameObject == projectileController.owner || (ignoreCharacters && component))
            {
                gameObject = null;
                networkhitHurtboxIndex = -1;
            }
            if (gameObject)
            {
                stickEvent.Invoke();
                base.GetComponent<AnchorConnectionComponent>().enabled = true;
                ParticleSystem[] array = stickParticleSystem;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].Play();
                }
                if (stickSoundString.Length > 0)
                {
                    Util.PlaySound(stickSoundString, base.gameObject);
                }
                if (alignNormals && impactNormal != Vector3.zero)
                {
                    base.transform.rotation = Util.QuaternionSafeLookRotation(impactNormal, base.transform.up);
                }
                Transform transform = hitCollider.transform;
                NetworklocalPosition = transform.InverseTransformPoint(base.transform.position);
                NetworklocalRotation = Quaternion.Inverse(transform.rotation) * base.transform.rotation;
                victim = gameObject;
                NetworkhitHurtboxIndex = networkhitHurtboxIndex;
                return true;
            }
            return false;
        }

        private void UpdateSticking()
        {
            bool flag = stuckTransform;
            if (flag)
            {
                base.transform.SetPositionAndRotation(stuckTransform.TransformPoint(localPosition), alignNormals ? (stuckTransform.rotation * localRotation) : base.transform.rotation);
            }
            else
            {
                GameObject gameObject = (NetworkServer.active ? victim : syncVictim);
                if (gameObject)
                {
                    stuckTransform = gameObject.transform;
                    flag = true;
                    if (hitHurtboxIndex >= 0)
                    {
                        stuckBody = stuckTransform.GetComponent<CharacterBody>();
                        if (stuckBody && stuckBody.hurtBoxGroup)
                        {
                            HurtBox hurtBox = stuckBody.hurtBoxGroup.hurtBoxes[hitHurtboxIndex];
                            stuckTransform = (hurtBox ? hurtBox.transform : null);
                        }
                    }
                }
                else if (hitHurtboxIndex == -2 && !NetworkServer.active)
                {
                    flag = true;
                }
            }
            if (!NetworkServer.active)
            {
                return;
            }
            if (rigidbody.isKinematic != flag)
            {
                if (flag)
                {
                    rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rigidbody.isKinematic = true;
                }
                else
                {
                    rigidbody.isKinematic = false;
                    rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                }
            }
            if (!flag)
            {
                NetworkhitHurtboxIndex = -1;
            }
        }

        private void UNetVersion()
        {
        }

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write(syncVictim);
                writer.WritePackedUInt32((uint)hitHurtboxIndex);
                writer.Write(localPosition);
                writer.Write(localRotation);
                return true;
            }
            bool flag = false;
            if ((base.syncVarDirtyBits & (true ? 1u : 0u)) != 0)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(syncVictim);
            }
            if ((base.syncVarDirtyBits & 2u) != 0)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.WritePackedUInt32((uint)hitHurtboxIndex);
            }
            if ((base.syncVarDirtyBits & 4u) != 0)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(localPosition);
            }
            if ((base.syncVarDirtyBits & 8u) != 0)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(localRotation);
            }
            if (!flag)
            {
                writer.WritePackedUInt32(base.syncVarDirtyBits);
            }
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                ___syncVictimNetId = reader.ReadNetworkId();
                hitHurtboxIndex = (sbyte)reader.ReadPackedUInt32();
                localPosition = reader.ReadVector3();
                localRotation = reader.ReadQuaternion();
                return;
            }
            int num = (int)reader.ReadPackedUInt32();
            if (((uint)num & (true ? 1u : 0u)) != 0)
            {
                syncVictim = reader.ReadGameObject();
            }
            if (((uint)num & 2u) != 0)
            {
                hitHurtboxIndex = (sbyte)reader.ReadPackedUInt32();
            }
            if (((uint)num & 4u) != 0)
            {
                localPosition = reader.ReadVector3();
            }
            if (((uint)num & 8u) != 0)
            {
                localRotation = reader.ReadQuaternion();
            }
        }

        public override void PreStartClient()
        {
            if (!___syncVictimNetId.IsEmpty())
            {
                NetworksyncVictim = ClientScene.FindLocalObject(___syncVictimNetId);
            }
        }
    }
}
