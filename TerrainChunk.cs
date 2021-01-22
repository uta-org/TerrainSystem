using System;
using System.IO;
using System.Linq;
using System.Text;
using uzSurfaceMapper.Extensions;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using uzSurfaceMapper.Model;
using uzSurfaceMapper.Core.Generators;
using uzSurfaceMapper.Extensions.Demo;
using uzSurfaceMapper.Utils.Generators;

namespace uzSurfaceMapper.Utils.Terrains
{
    using Structs;
    using Data;

    public class TerrainChunk
    {
        private const float ColliderGenerationDistanceThreshold = 5;
        private Bounds _bounds;
        private readonly int _colliderLodIndex;
        public Vector2 Coord;

        private readonly LodInfo[] _detailLevels;
        private bool _hasSetCollider;

        private HeightMap _heightMap;
        private bool _heightMapReceived;

        private readonly HeightMapSettings _heightMapSettings;
        private readonly LodMesh[] _lodMeshes;
        private readonly float _maxViewDst;
        private readonly MeshCollider _meshCollider;
        private readonly MeshFilter _meshFilter;

        private readonly GameObject _meshObject;

        private readonly MeshRenderer _meshRenderer;
        private readonly MeshSettings _meshSettings;
        private int _previousLodIndex = -1;
        private readonly Vector2 _sampleCentre;
        //private readonly Transform _viewer;

        public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings,
            LodInfo[] detailLevels, int colliderLodIndex, Transform parent, Material material)
        {
            this.Coord = coord;
            this._detailLevels = detailLevels;
            this._colliderLodIndex = colliderLodIndex;
            this._heightMapSettings = heightMapSettings;
            this._meshSettings = meshSettings;
            //this._viewer = viewer;

            _sampleCentre = coord * meshSettings.MeshWorldSize / meshSettings.meshScale;
            var position = coord * meshSettings.MeshWorldSize;
            _bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);

            _meshObject = new GameObject($"Terrain Chunk {coord.ToRoundedString("x")}");
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();
            _meshRenderer.material = material;

            _meshObject.transform.position = new Vector3(position.x, 0, position.y);
            _meshObject.transform.parent = parent;
            SetVisible(false);

            _lodMeshes = new LodMesh[detailLevels.Length];
            for (var i = 0; i < detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LodMesh(detailLevels[i].lod);
                _lodMeshes[i].UpdateCallback += UpdateTerrainChunk;
                if (i == colliderLodIndex)
                    _lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
            }

            _maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;

            TerrainGenerator.Instance.OnTerrainUpdated += OnTerrainUpdated;
        }

        private void OnTerrainUpdated()
        {
            var str = Builder.ToString();
            if (string.IsNullOrEmpty(str)) return;
            var path = Path.Combine(Environment.CurrentDirectory, "chunks.txt");
            File.WriteAllText(path, str);
            Debug.Log($"writed succesfully at '{path}'!");
            //Debug.Log(str);
            Builder.Clear();
        }

        private Vector2 ViewerPosition => new Vector2(FirstPersonController.Pos.x, FirstPersonController.Pos.z);
        // FirstPersonController.Pos;
        // new Vector2(_viewer.position.x, _viewer.position.z);

        public event Action<TerrainChunk, bool> OnVisibilityChanged;

        private static StringBuilder Builder { get; } = new StringBuilder();

        public void Load()
        {
            //if (!_heightMapSettings.generate) return;

            // TODO
            //ThreadedDataRequester.RequestData(
            //    () => HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine,
            //        _heightMapSettings, _sampleCentre), OnHeightMapReceived);

            // Generate buildings
            var position = Coord * _meshSettings.MeshWorldSize;
            var conv = CityGenerator.SConv;
            var scaledVector = conv.GetScaledVector(position);
            var c = City.GetChunk((int)scaledVector.x, (int)scaledVector.y, position.x, position.y, out var debugStr);
            //Debug.Log($"[{_meshObject.transform.name}] {position} -> {scaledVector}\nGenerating {c.listOfIndexBuildings.Count} builds!\n\n[{c}]\n{debugStr}"); // DON'T DELETE

            if (c.listOfIndexBuildings != null)
                try
                {
                    BuildingGeneratorUtil.CreateBuildings(c);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

            if (c.roadPoints != null)
                try
                {
                    // ReSharper disable once UnusedVariable

                    Builder.AppendLine($"starting to load chunk: {ToString()}");
                    var roads = c.roadPoints.AsEnumerable().CreateRoad(Builder).ToList();
                    RoadGeneratorUtil.Roads.Add(c, roads);

                    //var builder = new StringBuilder($"{road.name}\n{new string('=', 30)}\n\n");
                    //road.spline.nodes.ForEach(spline =>
                    //{
                    //    builder.AppendLine($"{spline.pos} -> {CityGenerator.SConv.GetRealPositionForTexture(spline.pos)}");
                    //});
                    //Debug.Log(builder.ToString());
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }

            //MonoSingleton<MapGenerator>.Instance.StartCoroutine(BuildingGeneratorUtil.CreateBuildings(c));
        }

        private void OnHeightMapReceived(object heightMapObject)
        {
            _heightMap = (HeightMap)heightMapObject;
            _heightMapReceived = true;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (_heightMapReceived)
            {
                var viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));

                var wasVisible = IsVisible();
                var visible = viewerDstFromNearestEdge <= _maxViewDst;

                if (visible)
                {
                    var lodIndex = 0;

                    for (var i = 0; i < _detailLevels.Length - 1; i++)
                        if (viewerDstFromNearestEdge > _detailLevels[i].visibleDstThreshold)
                            lodIndex = i + 1;
                        else
                            break;

                    if (lodIndex != _previousLodIndex)
                    {
                        var lodMesh = _lodMeshes[lodIndex];
                        if (lodMesh.HasMesh)
                        {
                            _previousLodIndex = lodIndex;
                            _meshFilter.mesh = lodMesh.Mesh;
                        }
                        else if (!lodMesh.HasRequestedMesh)
                        {
                            lodMesh.RequestMesh(_heightMap, _meshSettings);
                        }
                    }
                }

                if (wasVisible != visible)
                {
                    SetVisible(visible);
                    OnVisibilityChanged?.Invoke(this, visible);
                }
            }
        }

        public void UpdateCollisionMesh()
        {
            if (!_hasSetCollider)
            {
                var sqrDstFromViewerToEdge = _bounds.SqrDistance(ViewerPosition);

                if (sqrDstFromViewerToEdge < _detailLevels[_colliderLodIndex].SqrVisibleDstThreshold)
                    if (!_lodMeshes[_colliderLodIndex].HasRequestedMesh)
                        _lodMeshes[_colliderLodIndex].RequestMesh(_heightMap, _meshSettings);

                if (sqrDstFromViewerToEdge < ColliderGenerationDistanceThreshold * ColliderGenerationDistanceThreshold)
                    if (_lodMeshes[_colliderLodIndex].HasMesh)
                    {
                        _meshCollider.sharedMesh = _lodMeshes[_colliderLodIndex].Mesh;
                        _hasSetCollider = true;
                    }
            }
        }

        public void SetVisible(bool visible)
        {
            _meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return _meshObject.activeSelf;
        }

        public override string ToString()
        {
            return $"({_bounds.min.x:F2}, {_bounds.min.y:F2}, {_bounds.max.x:F2}, {_bounds.max.y:F2})";
        }
    }
}