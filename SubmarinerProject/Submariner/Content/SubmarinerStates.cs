using SubmarinerMod.Modules.BaseStates;
using SubmarinerMod.Submariner.SkillStates;

namespace SubmarinerMod.Submariner.Content
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
            Modules.Content.AddEntityState(typeof(AnchorBaseState));
            Modules.Content.AddEntityState(typeof(AnchorWaitForStick));
            Modules.Content.AddEntityState(typeof(AnchorTetherBehaviour));
            Modules.Content.AddEntityState(typeof(RecoverAnchor));
            Modules.Content.AddEntityState(typeof(AimAnchor));
        }
    }
}
