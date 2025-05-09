using BepInEx.Configuration;
using SubmarinerMod.Modules;
using UnityEngine;

namespace SubmarinerMod.SubmarinerCharacter.Content
{
    public static class SubmarinerConfig
    {
        public static ConfigEntry<bool> enableFunnyMode;

        public static ConfigEntry<float> maxHealth;
        public static ConfigEntry<float> healthRegen;
        public static ConfigEntry<float> armor;
        public static ConfigEntry<float> shield;

        public static ConfigEntry<int> jumpCount;

        public static ConfigEntry<float> damage;
        public static ConfigEntry<float> attackSpeed;
        public static ConfigEntry<float> crit;

        public static ConfigEntry<float> moveSpeed;
        public static ConfigEntry<float> acceleration;
        public static ConfigEntry<float> jumpPower;

        public static ConfigEntry<bool> autoCalculateLevelStats;

        public static ConfigEntry<float> healthGrowth;
        public static ConfigEntry<float> regenGrowth;
        public static ConfigEntry<float> armorGrowth;
        public static ConfigEntry<float> shieldGrowth;

        public static ConfigEntry<float> damageGrowth;
        public static ConfigEntry<float> attackSpeedGrowth;
        public static ConfigEntry<float> critGrowth;

        public static ConfigEntry<float> moveSpeedGrowth;
        public static ConfigEntry<float> jumpPowerGrowth;

        public static ConfigEntry<float> swingDamageCoefficient;
        public static ConfigEntry<float> harpoonDamageCoefficient;
        public static ConfigEntry<float> mineDamageCoefficient;
        public static ConfigEntry<float> beastDamageCoefficient;
        public static ConfigEntry<float> anchorDamageCoefficient;

        public static ConfigEntry<KeyboardShortcut> restKey;

        public static void Init()
        {
            string section = "Stats - 01";
            string section2 = "QOL - 02";
            string section3 = "Extra - 02";

            damage = Config.BindAndOptions(section, "Change Base Damage Value", 12f);

            maxHealth = Config.BindAndOptions(section, "Change Max Health Value", 130f);
            healthRegen = Config.BindAndOptions(section, "Change Health Regen Value", 1.5f);
            armor = Config.BindAndOptions(section, "Change Armor Value", 20f);
            shield = Config.BindAndOptions(section, "Change Shield Value", 0f);

            jumpCount = Config.BindAndOptions(section, "Change Jump Count", 1);

            attackSpeed = Config.BindAndOptions(section, "Change Attack Speed Value", 1f);
            crit = Config.BindAndOptions(section, "Change Crit Value", 1f);

            moveSpeed = Config.BindAndOptions(section, "Change Move Speed Value", 7f);
            acceleration = Config.BindAndOptions(section, "Change Acceleration Value", 80f);
            jumpPower = Config.BindAndOptions(section, "Change Jump Power Value", 15f);

            autoCalculateLevelStats = Config.BindAndOptions(section, "Auto Calculate Level Stats", true);

            healthGrowth = Config.BindAndOptions(section, "Change Health Growth Value", 0.3f);
            regenGrowth = Config.BindAndOptions(section, "Change Regen Growth Value", 0.2f);
            armorGrowth = Config.BindAndOptions(section, "Change Armor Growth Value", 0f);
            shieldGrowth = Config.BindAndOptions(section, "Change Shield Growth Value", 0f);

            damageGrowth = Config.BindAndOptions(section, "Change Damage Growth Value", 0.2f);
            attackSpeedGrowth = Config.BindAndOptions(section, "Change Attack Speed Growth Value", 0f);
            critGrowth = Config.BindAndOptions(section, "Change Crit Growth Value", 0f);

            moveSpeedGrowth = Config.BindAndOptions(section, "Change Move Speed Growth Value", 0f);
            jumpPowerGrowth = Config.BindAndOptions(section, "Change Jump Power Growth Value", 0f);

            swingDamageCoefficient = Config.BindAndOptions(section, "AL811 Anchoring Device Damage Coefficient", 4.5f);
            harpoonDamageCoefficient = Config.BindAndOptions(section, "Reel-In Damage Coefficient", 3f);
            mineDamageCoefficient = Config.BindAndOptions(section, "Explosive Urchin Damage Coefficient", 7f);
            beastDamageCoefficient = Config.BindAndOptions(section, "Cults Best Friend Damage Coefficient", 7f);
            anchorDamageCoefficient = Config.BindAndOptions(section, "Drop the Anchor Damage Coefficient", 12f);

            enableFunnyMode = Config.BindAndOptions(
                section3,
                "Enable Mini Beast",
                false,
                "Enable Mini Beast.", true);

            restKey = Config.BindAndOptions(section3, "Rest Emote", new KeyboardShortcut(KeyCode.Alpha1), "Key used to Rest");
        }
    }
}
