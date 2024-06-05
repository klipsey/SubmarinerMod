﻿using JetBrains.Annotations;
using UnityEngine;
using RoR2.Skills;
using RoR2;

namespace InterrogatorMod.Interrogator.Components
{
    public class SubmarinerSkillDef : SkillDef
    {
        protected class InstanceData : BaseSkillInstanceData
        {
            public SubmarinerTracker tracker;
        }

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new InstanceData
            {
                tracker = skillSlot.GetComponent<SubmarinerTracker>()
            };
        }

        private static bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            if (!(((InstanceData)skillSlot.skillInstanceData).tracker?.GetTrackingTarget()))
            {
                return false;
            }
            return true;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            if (!HasTarget(skillSlot))
            {
                return false;
            }
            return base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            if (base.IsReady(skillSlot))
            {
                return HasTarget(skillSlot);
            }
            return false;
        }
    }
}


