using System;
using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains
{
    [Serializable]
    public class NoiseSettings
    {
        public float lacunarity = 2;
        public Noise.NormalizeMode normalizeMode;

        public int octaves = 6;
        public Vector2 offset;

        [Range(0, 1)] public float persistance = .6f;

        public float scale = 50;

        public int seed;

        public void ValidateValues()
        {
            scale = Mathf.Max(scale, 0.01f);
            octaves = Mathf.Max(octaves, 1);
            lacunarity = Mathf.Max(lacunarity, 1);
            persistance = Mathf.Clamp01(persistance);
        }
    }
}