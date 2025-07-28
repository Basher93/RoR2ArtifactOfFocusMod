using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace ArtifactOfFocus
{
    public static class ConfigManager
    {
        public static ConfigEntry<float> ChestSpawnMultiplier;
        public static ConfigEntry<float> TerminalSpawnMultiplier;
        public static ConfigEntry<float> ShrineSpawnMultiplier;

        public static ConfigEntry<float> WhiteChance;
        public static ConfigEntry<float> GreenChance;
        public static ConfigEntry<float> RedChance;
        public static ConfigEntry<float> YellowChance;
        public static ConfigEntry<float> BlueChance;

        public static void Init(ConfigFile config)
        {
            // Multipliers
            ChestSpawnMultiplier = config.Bind("SpawnRates", "ChestSpawnMultiplier", 0.5f, "Multiplier for chest spawns (0 to 1). Default is 0.5");
            TerminalSpawnMultiplier = config.Bind("SpawnRates", "TerminalSpawnMultiplier", 0.5f, "Multiplier for terminal spawns (0 to 1). Default is 0.5");
            ShrineSpawnMultiplier = config.Bind("SpawnRates", "ShrineSpawnMultiplier", 0.75f, "Multiplier for chance shrine spawns (0 to 1). Default is 0.75");

            // Rarity chances (must total 100)
            WhiteChance = config.Bind("RarityChances", "WhiteItemChance", 73f, "Chance for white tier items to be selected (Total must be 100)");
            GreenChance = config.Bind("RarityChances", "GreenItemChance", 24f, "Chance for green tier items to be selected (Total must be 100)");
            RedChance = config.Bind("RarityChances", "RedItemChance", 1f, "Chance for red tier items to be selected (Total must be 100)");
            YellowChance = config.Bind("RarityChances", "YellowItemChance", 1f, "Chance for yellow tier items to be selected (Total must be 100)");
            BlueChance = config.Bind("RarityChances", "BlueItemChance", 1f, "Chance for blue (lunar) tier items to be selected (Total must be 100)");

            ModSettingsManager.SetModDescription("Artifact of Focus - Halved chest spawns, but on every item pickup the run's focused item is also dropped!");

            ModSettingsManager.AddOption(new StepSliderOption(ChestSpawnMultiplier, new StepSliderConfig { min = 0f, max = 1f, increment = 0.05f }));
            ModSettingsManager.AddOption(new StepSliderOption(TerminalSpawnMultiplier, new StepSliderConfig { min = 0f, max = 1f, increment = 0.05f }));
            ModSettingsManager.AddOption(new StepSliderOption(ShrineSpawnMultiplier, new StepSliderConfig { min = 0f, max = 1f, increment = 0.05f }));

            ModSettingsManager.AddOption(new StepSliderOption(WhiteChance, new StepSliderConfig { min = 0f, max = 100f, increment = 1f }));
            ModSettingsManager.AddOption(new StepSliderOption(GreenChance, new StepSliderConfig { min = 0f, max = 100f, increment = 1f }));
            ModSettingsManager.AddOption(new StepSliderOption(RedChance, new StepSliderConfig { min = 0f, max = 100f, increment = 1f }));
            ModSettingsManager.AddOption(new StepSliderOption(YellowChance, new StepSliderConfig { min = 0f, max = 100f, increment = 1f }));
            ModSettingsManager.AddOption(new StepSliderOption(BlueChance, new StepSliderConfig { min = 0f, max = 100f, increment = 1f }));
        }
    }
}