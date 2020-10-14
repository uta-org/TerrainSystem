using System;
using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains
{
    using Data;

    [Serializable]
    public struct LodInfo
    {
        [Range(0, MeshSettings.NumSupportedLoDs - 1)]
        public int lod;

        public float visibleDstThreshold;

        public float SqrVisibleDstThreshold => visibleDstThreshold * visibleDstThreshold;
    }
}