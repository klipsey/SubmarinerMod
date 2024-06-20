using BepInEx.Configuration;
using SubmarinerMod.Modules;

namespace SubmarinerMod.Submariner.Content
{
    public static class SubmarinerConfig
    {
        public static ConfigEntry<bool> enableFunnyMode;
        public static void Init()
        {
            string section = "01 - General";
            string section2 = "02 - Stats";

            //add more here or else you're cringe
            enableFunnyMode = Config.BindAndOptions(
                section,
                "Enable Mini Beast",
                false,
                "Enable Mini Beast.", true);
        }
    }
}
