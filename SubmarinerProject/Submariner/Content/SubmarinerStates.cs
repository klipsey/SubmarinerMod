using InterrogatorMod.Modules.BaseStates;
using InterrogatorMod.Interrogator.SkillStates;

namespace InterrogatorMod.Interrogator.Content
{
    public static class SubmarinerStates
    {
        public static void Init()
        {
            Modules.Content.AddEntityState(typeof(BaseInterrogatorSkillState));
            Modules.Content.AddEntityState(typeof(MainState));
            Modules.Content.AddEntityState(typeof(BaseInterrogatorState));
            Modules.Content.AddEntityState(typeof(Swing));
            Modules.Content.AddEntityState(typeof(ThrowCleaver));
            Modules.Content.AddEntityState(typeof(Falsify));
            Modules.Content.AddEntityState(typeof(Convict));
            Modules.Content.AddEntityState(typeof(ConvictScepter));
        }
    }
}
