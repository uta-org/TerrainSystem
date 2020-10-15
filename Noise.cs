//using System;
//using System.Threading.Tasks;
//using uzSurfaceMapper.Core;
//using uzSurfaceMapper.Core.Data;
//using uzSurfaceMapper.Core.Enums;
//using uzSurfaceMapper.Core.Generators;
//using uzSurfaceMapper.Extensions;
//using UnityEngine;
//using uzSurfaceMapper.Core.Workers;
//using MyColor = uzSurfaceMapper.Model.Color;
//using static uzSurfaceMapper.Core.Generators.TerrainGenerator;
//using Random = System.Random;

namespace uzSurfaceMapper.Utils.Terrains
{
    public static class Noise
    {
        // TODO
        public enum NormalizeMode
        {
            Local,
            Global
        }

        //public static float[,] GetHeightMap(Vector2 position, int width, int height, NoiseSettings settings, float detailScale = 1f, bool useParallel = false)
        //{
        //    if (CityGenerator.SConv == null)
        //        throw new Exception("SceneConversion is not ready yet!");

        //    var map = new float[width, height];

        //    var halfWidth = width / 2f;
        //    var halfHeight = height / 2f;

        //    if (useParallel)
        //        Parallel.For(
        //            0,
        //            width * height,
        //            ParallelWorker.Options,
        //            index => DoIterations(position, width, height, index, halfWidth, halfHeight, map));
        //    else
        //        for (int i = 0; i < width * height; i++)
        //            DoIterations(position, width, height, i, halfWidth, halfHeight, map);

        //    return map;
        //}

        //private static void DoIterations(Vector2 position, int width, int height, int index, float halfWidth, float halfHeight,
        //    float[,] map)
        //{
        //    int x = index % width;
        //    int z = index / width;

        //    //var mapWidth = MapGenerator.mapWidth;
        //    //var mapHeight = MapGenerator.mapHeight;

        //    //int tx = (int)(x - halfWidth + position.x);
        //    //int tz = (int)(z - halfHeight + position.y);

        //    //int lx = CityGenerator.SConv.GetRealPositionForTexture(tx, true).InvertY(mapHeight);
        //    //int lz = CityGenerator.SConv.GetRealPositionForTexture(tz, false);

        //    //var data = VoronoiTextureWorker.VoronoiTexture[F.Pn(lx, lz, mapWidth)].GetGroundTypeData();

        //    //if (data == null)
        //    //{
        //    //    VoronoiTextureWorker.VoronoiTexture[F.Pn(lx, lz, mapWidth)] = MyColor.red;
        //    //}
        //    //else if (!data.IsAllowed && data.GroundType != GroundType.Water &&
        //    //         data.Behaviour != GroundBehaviour.Road)
        //    //{
        //    //    VoronoiTextureWorker.DoVoronoiChange(lx, lz, mapWidth, mapHeight);
        //    //}

        //    // generate the height for current vertex
        //    Vector2 vertexPosition = position + new Vector2(x - halfWidth, height - (z - halfHeight) - 1);
        //    //vertexPosition = CityGenerator.SConv.GetRealPositionForTexture(vertexPosition);

        //    map[x, z] =
        //        GetHeightUsingColors(vertexPosition.x, vertexPosition.y);

        //    // TODO: This doesn't work as expected
        //    ////vertexPosition = CityGenerator.SConv.GetRealPositionForTexture(vertexPosition);

        //    ////map[x, y] =
        //    ////    GetHeight(vertexPosition.x, vertexPosition.y, true);
        //}
    }
}