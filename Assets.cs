using BepInEx;
using System.IO;
using UnityEngine;

namespace ArtifactOfFocus
{
    public static class Assets
    {
        public static Sprite iconSelected;
        public static Sprite iconDeselected;

        public static void PopulateAssets()
        {
            // Adjusted folder name to match your actual folder structure with spaces
            string basePath = Path.GetDirectoryName(typeof(ArtifactOfFocus).Assembly.Location);

            Debug.Log($"[ArtifactOfFocus] Loading assets from: {basePath}");

            iconSelected = LoadSprite(Path.Combine(basePath, "IconSelected.png"));
            iconDeselected = LoadSprite(Path.Combine(basePath, "IconDeselected.png"));
        }

        private static Sprite LoadSprite(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"[ArtifactOfFocus] Asset not found at path: {path}");
                return null;
            }

            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            tex.LoadImage(data);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}