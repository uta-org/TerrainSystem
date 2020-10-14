using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains.Data
{
    [CreateAssetMenu]
    public class HeightMapSettings : UpdatableData
    {
        public bool generate;

        public AnimationCurve heightCurve;

        public float heightMultiplier;

        public NoiseSettings noiseSettings;

        public bool useFalloff;

        public float MinHeight => heightMultiplier * heightCurve.Evaluate(0);

        public float MaxHeight => heightMultiplier * heightCurve.Evaluate(1);

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            noiseSettings.ValidateValues();
            base.OnValidate();
        }

#endif
    }
}