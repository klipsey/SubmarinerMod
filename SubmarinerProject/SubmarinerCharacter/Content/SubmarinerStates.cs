﻿using SubmarinerMod.Modules.BaseStates;
using SubmarinerMod.SubmarinerCharacter.SkillStates;

namespace SubmarinerMod.SubmarinerCharacter.Content
{
    public static class SubmarinerStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(BaseSubmarinerSkillState));
            Modules.Content.AddEntityState(typeof(MainState));
            Modules.Content.AddEntityState(typeof(BaseSubmarinerState));
            Modules.Content.AddEntityState(typeof(Swing));
            Modules.Content.AddEntityState(typeof(HarpoonShot));
            Modules.Content.AddEntityState(typeof(BackFlip));
            Modules.Content.AddEntityState(typeof(RecoverAnchor));
            Modules.Content.AddEntityState(typeof(AimAnchor));
            Modules.Content.AddEntityState(typeof(BeastRide));
            Modules.Content.AddEntityState(typeof(BeastImpact));
            Modules.Content.AddEntityState(typeof(Rest));
            Modules.Content.AddEntityState(typeof(BaseEmote));
        }
    }
}
