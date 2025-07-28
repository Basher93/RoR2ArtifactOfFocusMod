using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactOfFocus
{
    public static class FocusArtifactLogic
    {
        private static readonly System.Reflection.FieldInfo originObjectField = typeof(PickupDropletController).GetField("originObject", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        public static PickupIndex selectedPickupIndex = PickupIndex.none;

        private static readonly HashSet<PickupDropletController> dropletsHandled = new HashSet<PickupDropletController>();
        private static readonly Dictionary<CharacterMaster, int> previousCounts = new Dictionary<CharacterMaster, int>();

        private static bool itemRolled = false;

        public static void Init()
        {
            Run.onRunStartGlobal += OnRunStart;
            On.RoR2.SceneDirector.Start += OnSceneDirectorStart;
            On.RoR2.CharacterMaster.OnInventoryChanged += OnInventoryChanged;
            On.RoR2.PickupDropletController.Start += OnPickupDropletControllerStart;
        }

        private static void OnRunStart(Run run)
        {
            previousCounts.Clear();
            dropletsHandled.Clear();
            selectedPickupIndex = PickupIndex.none;
            itemRolled = false;

            EnsureItemRolled();  // Roll item at run start
        }

        private static void EnsureItemRolled()
        {
            if (itemRolled) return;

            if (Run.instance == null
                || Run.instance.availableTier1DropList == null
                || Run.instance.availableTier1DropList.Count == 0)
            {
                Debug.LogWarning("[ArtifactOfFocus] Run or drop list not ready yet.");
                return;
            }

            selectedPickupIndex = RollItemFromConfig();

            if (selectedPickupIndex != PickupIndex.none)
            {
                var def = PickupCatalog.GetPickupDef(selectedPickupIndex);
                if (def != null && def.itemIndex != ItemIndex.None)
                {
                    string itemName = Language.GetString(ItemCatalog.GetItemDef(def.itemIndex).nameToken);
                    Chat.AddMessage($"Artifact of Focus: All items this run will include: {itemName}");
                    Debug.Log($"[ArtifactOfFocus] Selected focused item: {itemName}");
                }
                else
                {
                    Debug.LogError("[ArtifactOfFocus] Invalid item selected.");
                    selectedPickupIndex = PickupIndex.none;
                }
            }
            else
            {
                Debug.LogWarning("[ArtifactOfFocus] No valid item selected.");
            }

            itemRolled = true;
        }

        private static PickupIndex RollItemFromConfig()
        {
            var weightedTiers = new List<Tuple<ItemTier, int>>()
{
            new Tuple<ItemTier, int>(ItemTier.Tier1, (int)ConfigManager.WhiteChance.Value),
            new Tuple<ItemTier, int>(ItemTier.Tier2, (int)ConfigManager.GreenChance.Value),
            new Tuple<ItemTier, int>(ItemTier.Tier3, (int)ConfigManager.RedChance.Value),
            new Tuple<ItemTier, int>(ItemTier.Boss, (int)ConfigManager.YellowChance.Value),
            new Tuple<ItemTier, int>(ItemTier.Lunar, (int)ConfigManager.BlueChance.Value),
};

            int totalWeight = 0;
            foreach (var pair in weightedTiers)
                totalWeight += pair.Item2;
            if (totalWeight == 0) return PickupIndex.none;

            int roll = UnityEngine.Random.Range(0, totalWeight);
            int current = 0;

            foreach (var pair in weightedTiers)
            {
                current += pair.Item2;
                if (roll < current)
                {
                    ItemTier tier = pair.Item1;

                    List<PickupIndex> dropList = Run.instance.availableTier1DropList;
                    if (tier == ItemTier.Tier2) dropList = Run.instance.availableTier2DropList;
                    else if (tier == ItemTier.Tier3) dropList = Run.instance.availableTier3DropList;
                    else if (tier == ItemTier.Boss) dropList = Run.instance.availableBossDropList;
                    else if (tier == ItemTier.Lunar) dropList = Run.instance.availableLunarItemDropList;

                    var filtered = dropList.Where(p =>
                    {
                        var def = PickupCatalog.GetPickupDef(p);
                        return def != null && def.itemIndex != ItemIndex.None && !ItemCatalog.GetItemDef(def.itemIndex).IsUtility();
                    }).ToList();

                    if (filtered.Count > 0)
                        return filtered[UnityEngine.Random.Range(0, filtered.Count)];
                    else
                        return PickupIndex.none;
                }
            }

            return PickupIndex.none;
        }

        private static void OnSceneDirectorStart(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);

            if (!RunArtifactManager.instance?.IsArtifactEnabled(ArtifactOfFocus.FocusArtifactDef) ?? true) return;

            float multiplier = Mathf.Clamp01(ConfigManager.ChestSpawnMultiplier.Value);
            foreach (var pi in UnityEngine.Object.FindObjectsOfType<PurchaseInteraction>())
            {
                if (pi.costType == CostTypeIndex.Money && UnityEngine.Random.value > multiplier)
                {
                    UnityEngine.Object.Destroy(pi.gameObject);
                }
            }
        }

        private static void OnPickupDropletControllerStart(On.RoR2.PickupDropletController.orig_Start orig, PickupDropletController self)
        {
            orig(self);

            if (!NetworkServer.active) return;
            if (!RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfFocus.FocusArtifactDef)) return;

            if (selectedPickupIndex == PickupIndex.none)
            {
                Debug.LogWarning("[ArtifactOfFocus] Focused item not rolled yet.");
                EnsureItemRolled();
                if (selectedPickupIndex == PickupIndex.none) return;
            }

            if (self.pickupIndex == PickupIndex.none) return;
            if (self.pickupIndex == selectedPickupIndex) return;

            //  Prevent duplication when the drop came from a printer
            var pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);
            if (pickupDef != null && pickupDef.itemIndex != ItemIndex.None)
            {
                // Check if the drop is replacing another item — typical for 3D printers
                var itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
                if (itemDef != null && itemDef.ContainsTag(ItemTag.Scrap))
                {
                    Debug.Log("[ArtifactOfFocus] Skipping scrap-related drop (likely printer).");
                    return;
                }
            }

            // Prevent duplicate spawns
            if (dropletsHandled.Contains(self)) return;
            dropletsHandled.Add(self);

            // Drop the focus item next to the original
            Vector3 position = self.transform.position;
            Vector3 velocity = Vector3.zero;

            PickupDropletController.CreatePickupDroplet(
                selectedPickupIndex,
                position,
                velocity
            );

            string itemName = Language.GetString(ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(selectedPickupIndex).itemIndex).nameToken);
            Chat.AddMessage($"<color=#b4d455>{itemName}</color> granted by the Artifact of Focus!");
            // Disabled the following line for release, adds a chat line everytime the focus item drops:
            // Debug.Log($"[ArtifactOfFocus] Dropped focused item: {itemName}");
        }

        private static readonly Dictionary<CharacterMaster, int> previousTotalCounts = new Dictionary<CharacterMaster, int>();
        private static void OnInventoryChanged(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster master)
        {
            orig(master);

            if (!NetworkServer.active) return;
            if (!RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfFocus.FocusArtifactDef)) return;
            if (selectedPickupIndex == PickupIndex.none) return;

            var inventory = master.inventory;
            if (inventory == null) return;

            int totalItemsExcludingFocus = 0;
            foreach (ItemIndex itemIndex in Enum.GetValues(typeof(ItemIndex)))
            {
                if (itemIndex == ItemIndex.None) continue;
                if (itemIndex == PickupCatalog.GetPickupDef(selectedPickupIndex).itemIndex) continue;
                totalItemsExcludingFocus += inventory.GetItemCount(itemIndex);
            }

            previousTotalCounts.TryGetValue(master, out int previousCount);

            int gained = totalItemsExcludingFocus - previousCount;

            if (gained > 0)
            {
                inventory.GiveItem(PickupCatalog.GetPickupDef(selectedPickupIndex).itemIndex, gained);

                string itemName = Language.GetString(ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(selectedPickupIndex).itemIndex).nameToken);
                Chat.AddMessage($"<color=#b4d455>{itemName}</color> granted by the Artifact of Focus x{gained}!");
            }

            previousTotalCounts[master] = totalItemsExcludingFocus;
        }
    }

    public static class ItemDefExtensions
    {
        public static bool IsUtility(this ItemDef def)
        {
            return def.tier == ItemTier.Lunar && (def.tags.Contains(ItemTag.Utility) || def.name.ToLower().Contains("lunarutility"));
        }
    }
}