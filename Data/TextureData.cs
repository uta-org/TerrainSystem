using System;
using System.Linq;
using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains.Data
{
    [CreateAssetMenu]
    public class TextureData : UpdatableData
    {
        private const int TextureSize = 512;
        private const TextureFormat TextureFormat = UnityEngine.TextureFormat.RGB565;

        public Layer[] layers;
        private float _savedMaxHeight;

        private float _savedMinHeight;

        public void ApplyToMaterial(Material material)
        {
            material.SetInt("layerCount", layers.Length);
            material.SetColorArray("baseColours", layers.Select(x => x.tint).ToArray());
            material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
            material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
            material.SetFloatArray("baseColourStrength", layers.Select(x => x.tintStrength).ToArray());
            material.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());
            var texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
            material.SetTexture("baseTextures", texturesArray);

            UpdateMeshHeights(material, _savedMinHeight, _savedMaxHeight);
        }

        public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
        {
            _savedMinHeight = minHeight;
            _savedMaxHeight = maxHeight;

            material.SetFloat("minHeight", minHeight);
            material.SetFloat("maxHeight", maxHeight);
        }

        private Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            var textureArray = new Texture2DArray(TextureSize, TextureSize, textures.Length, TextureFormat, true);
            for (var i = 0; i < textures.Length; i++)
                textureArray.SetPixels(textures[i].GetPixels(), i);
            textureArray.Apply();
            return textureArray;
        }

        [Serializable]
        public class Layer
        {
            [Range(0, 1)] public float blendStrength;

            [Range(0, 1)] public float startHeight;

            public Texture2D texture;
            public float textureScale;
            public Color tint;

            [Range(0, 1)] public float tintStrength;
        }
    }
}