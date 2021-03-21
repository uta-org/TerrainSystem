using System;
using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains
{
    using Structs;
    using Data;

    internal class LodMesh
    {
        public bool HasMesh;
        public bool HasRequestedMesh;
        private readonly int _lod;

        public Mesh Mesh;

        public LodMesh(int lod)
        {
            this._lod = lod;
        }

        public event Action UpdateCallback;

        private void OnMeshDataReceived(object meshDataObject)
        {
            Mesh = ((MeshData)meshDataObject).CreateMesh();
            HasMesh = true;

            UpdateCallback?.Invoke();
        }

        public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
        {
            HasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.Values, meshSettings, _lod),
                OnMeshDataReceived);
        }
    }
}