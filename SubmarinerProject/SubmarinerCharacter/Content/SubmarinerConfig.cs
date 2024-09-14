using BepInEx.Configuration;
using SubmarinerMod.Modules;
using UnityEngine;

namespace SubmarinerMod.SubmarinerCharacter.Content
{
    public static class SubmarinerConfig
    {
        public static ConfigEntry<bool> enableFunnyMode;

        public static ConfigEntry<KeyboardShortcut> restKey;
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

            restKey = Config.BindAndOptions("02 - Keybinds", "Rest Emote", new KeyboardShortcut(KeyCode.Alpha1), "Key used to Rest");
        }
    }
}
