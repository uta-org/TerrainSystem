//using System.Diagnostics;
//using UnityEngine;
//using Debug = UnityEngine.Debug;

using System;
using UnityEngine;
using uzSurfaceMapper.Utils.Terrains.Data;
using uzSurfaceMapper.Utils.Terrains.Structs;

namespace uzSurfaceMapper.Utils.Terrains
{
    //using Structs;
    //using Data;

    public static class HeightMapGenerator
    {
        // TODO: Not needed on Demo.
        //public const bool UseDebug = false;

        public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings,
            Vector2 sampleCentre)
        {
            if (!settings.generate)
                return new HeightMap(GetEmptyData(width, height), 0, 0);

            throw new NotImplementedException();
        }

        //public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre)
        //{
        //    Stopwatch watch;

        //    if (UseDebug)
        //        watch = Stopwatch.StartNew();

        //    var values = !settings.generate ? GetEmptyData(width, height) :
        //        Noise.GetHeightMap(sampleCentre, width, height, settings.noiseSettings);
        //    var heightCurveThreadsafe = new AnimationCurve(settings.heightCurve.keys);

        //    var minValue = float.MaxValue;
        //    var maxValue = float.MinValue;

        //    if (settings.generate)
        //        for (var i = 0; i < width; i++)
        //            for (var j = 0; j < height; j++)
        //            {
        //                values[i, j] *= heightCurveThreadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

        //                if (values[i, j] > maxValue)
        //                    maxValue = values[i, j];
        //                if (values[i, j] < minValue)
        //                    minValue = values[i, j];
        //            }

        //    if (UseDebug)
        //    {
        //        watch.Stop();
        //        Debug.Log($"Heightmaps generated in {watch.ElapsedMilliseconds} ms!");
        //    }

        //    return new HeightMap(values, minValue, maxValue);
        //}

        private static float[,] GetEmptyData(int width, int height)
        {
            var data = new float[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    data[i, j] = 0;
                }
            }

            return data;
        }
    }
}