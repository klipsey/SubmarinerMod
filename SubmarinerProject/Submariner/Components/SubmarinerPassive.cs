using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SubmarinerMod.Submariner.Components
{
    public class SubmarinerPassive : MonoBehaviour
    {
        public SkillDef SubmarinerPassiveSkillDef;

        public GenericSkill passiveSkillSlot;

        public bool isJump
        {
            get
            {
                if (SubmarinerPassiveSkillDef && passiveSkillSlot)
                {
                    return passiveSkillSlot.skillDef == SubmarinerPassiveSkillDef;
                }

                return false;
            }
        }
    }
}