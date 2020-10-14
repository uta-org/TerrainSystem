using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains.Data
{
    [CreateAssetMenu]
    public class MeshSettings : UpdatableData
    {
        public const int NumSupportedLoDs = 5;
        public const int NumSupportedChunkSizes = 9;
        public const int NumSupportedFlatshadedChunkSizes = 3;
        public static readonly int[] SupportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

        [Range(0, NumSupportedChunkSizes - 1)] public int chunkSizeIndex;

        [Range(0, NumSupportedFlatshadedChunkSizes - 1)]
        public int flatshadedChunkSizeIndex;

        public float meshScale = 2.5f;
        public bool useFlatShading;

        // num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh, but used for calculating normals
        public int NumVertsPerLine => SupportedChunkSizes[useFlatShading ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;

        public float MeshWorldSize => (NumVertsPerLine - 3) * meshScale;
    }
}