﻿using System.Linq;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using InterrogatorMod.Interrogator;
using InterrogatorMod.Interrogator.Content;

namespace InterrogatorMod.Interrogator.Components

{
    [RequireComponent(typeof(TeamComponent))]
    [RequireComponent(typeof(CharacterBody))]
    [RequireComponent(typeof(InputBankTest))]
    public class SubmarinerTracker : MonoBehaviour
    {
        public float maxTrackingDistance = 40f;

        public float maxTrackingAngle = 20f;

        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;

        private CharacterBody characterBody;

        private TeamComponent teamComponent;

        private InputBankTest inputBank;

        private float trackerUpdateStopwatch;

        private Indicator indicator;

        private bool onCooldown;

        private readonly BullseyeSearch search = new BullseyeSearch();

        private void Awake()
        {
            indicator = new Indicator(base.gameObject, LegacyResourcesAPI.Load<GameObject>("Prefabs/HuntressTrackingIndicator"));
        }

        private void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            inputBank = GetComponent<InputBankTest>();
            teamComponent = GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            if (trackingTarget != null)
            {
                if (!onCooldown)
                {
                    return trackingTarget;
                }
                else
                {
                    return null;
                }
            }
            else return trackingTarget;
        }
        private void OnEnable()
        {
            indicator.active = true;
        }

        private void OnDisable()
        {
            indicator.active = false;
        }

        private void FixedUpdate()
        {
            trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (trackerUpdateStopwatch >= 1f / trackerUpdateFrequency)
            {
                trackerUpdateStopwatch -= 1f / trackerUpdateFrequency;
                _ = trackingTarget;
                Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                SearchForTarget(aimRay);
                if (trackingTarget != null && characterBody.skillLocator.special.skillNameToken != SubmarinerSurvivor.INTERROGATOR_PREFIX + "SPECIAL_SCEPTER_CONVICT_NAME")
                {
                    onCooldown = !trackingTarget.healthComponent.body.HasBuff(SubmarinerBuffs.interrogatorGuiltyDebuff);
                }
                if (onCooldown)
                {
                    indicator.targetTransform = null;
                }
                else
                {
                    indicator.targetTransform = trackingTarget ? trackingTarget.transform : null;
                }
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            search.teamMaskFilter = TeamMask.GetUnprotectedTeams(teamComponent.teamIndex);
            search.filterByLoS = true;
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = maxTrackingDistance;
            search.maxAngleFilter = maxTrackingAngle;
            search.RefreshCandidates();
            search.FilterOutGameObject(gameObject);
            trackingTarget = search.GetResults().FirstOrDefault();
        }
    }
}

