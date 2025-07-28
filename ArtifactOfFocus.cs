using BepInEx;
using R2API;
using R2API.ContentManagement;
using RoR2;
using UnityEngine;

namespace ArtifactOfFocus
{
    [BepInPlugin("com.Basher93.ArtifactOfFocus", "Artifact of Focus", "1.0.0")]
    [BepInDependency(R2API.R2API.PluginGUID)]
    public class ArtifactOfFocus : BaseUnityPlugin
    {
        public static ArtifactDef FocusArtifactDef;

        public void Awake()
        {
            // Initialize config using the instance Config property
            ConfigManager.Init(Config);

            // Load asset icons (make sure your Assets class exists and works)
            Assets.PopulateAssets();

            // Create Artifact definition
            FocusArtifactDef = ScriptableObject.CreateInstance<ArtifactDef>();
            FocusArtifactDef.cachedName = "ArtifactOfFocus";
            FocusArtifactDef.nameToken = "ARTIFACTOFFOCUS_NAME";
            FocusArtifactDef.descriptionToken = "ARTIFACTOFFOCUS_DESC";
            FocusArtifactDef.smallIconSelectedSprite = Assets.iconSelected;
            FocusArtifactDef.smallIconDeselectedSprite = Assets.iconDeselected;

            // Register artifact with R2API
            ContentAddition.AddArtifactDef(FocusArtifactDef);

            // Add localization strings
            LanguageAPI.Add("ARTIFACTOFFOCUS_NAME", "Artifact of Focus");
            LanguageAPI.Add("ARTIFACTOFFOCUS_DESC", "Fewer chests and shrines. All drops include the one same random item, chosen at run start.");

            // Initialize the artifact logic (your class that handles in-game effects)
            FocusArtifactLogic.Init();
        }
    }
}