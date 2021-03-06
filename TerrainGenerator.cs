﻿using System;
using System.Collections;
using System.Collections.Generic;
using uzSurfaceMapper.Core;
using uzSurfaceMapper.Core.Generators;
using uzSurfaceMapper.Extensions;
using UnityEngine;
using UnityEngine.Core;
using UnityEngine.Serialization;
using UnityStandardAssets.Characters.FirstPerson;
using uzSurfaceMapper.Core.Workers;

namespace uzSurfaceMapper.Utils.Terrains
{
    using Data;
    using uzSurfaceMapper.Extensions.Demo;

    public class TerrainGenerator : MonoSingleton<TerrainGenerator>
    {
        public bool m_GenerateHeightmaps = true;

        private const float ViewerMoveThresholdForChunkUpdate = 25f;

        private const float SqrViewerMoveThresholdForChunkUpdate =
            ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;

        private int _chunksVisibleInViewDst;

        [FormerlySerializedAs("colliderLODIndex")] public int colliderLodIndex;
        public LodInfo[] detailLevels;
        public HeightMapSettings heightMapSettings;
        public Material mapMaterial;

        public MeshSettings meshSettings;

        public bool saveTextureAtStart;

        private float _meshWorldSize;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        public TextureData textureSettings;

        //public Transform viewer;

        private Vector2 _viewerPosition;
        private Vector2 _viewerPositionOld;
        private readonly List<TerrainChunk> _visibleTerrainChunks = new List<TerrainChunk>();

        private bool _firstUpdate;

        public event Action OnTerrainUpdated = delegate { };

        private void Start()
        {
            heightMapSettings.generate = m_GenerateHeightmaps;

            textureSettings.ApplyToMaterial(mapMaterial);
            textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);

            var maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
            _meshWorldSize = meshSettings.MeshWorldSize;
            _chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / _meshWorldSize);

            StartCoroutine(WaitUntilGeneratorIsReady());
        }

        private void Update()
        {
            if (!MapGenerator.IsReady)
                return;

            _viewerPosition = new Vector2(FirstPersonController.Pos.x, FirstPersonController.Pos.z);
            // FirstPersonController.Pos;
            // new Vector2(viewer.position.x, viewer.position.z);

            if (_viewerPosition != _viewerPositionOld)
                foreach (var chunk in _visibleTerrainChunks)
                    chunk.UpdateCollisionMesh();

            if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > SqrViewerMoveThresholdForChunkUpdate)
            {
                _viewerPositionOld = _viewerPosition;
                UpdateVisibleChunks();
            }
        }

        private IEnumerator WaitUntilGeneratorIsReady()
        {
            Debug.Log("Waiting for generator to end.");

            // First, execute registered workers
            foreach (var worker in TextureWorkerBase.WorkersCollection)
            {
                yield return new WaitUntil(() => worker.IsReady);

                if (worker == null)
                {
                    Debug.LogWarning("Null worker!");
                    continue;
                }

                if (worker.CurrentColors == null)
                {
                    Debug.LogWarning($"Null CurrentColors from worker ({worker.GetType().FullName})!");
                    continue;
                }

                if (saveTextureAtStart)
                    lock (worker.CurrentColors)
                        worker.SaveTexture(worker.CurrentColors, false);
            }

            Debug.Log($"Finished workers. Waiting for MapGenerator: {MapGenerator.IsReadyLog}");

            yield return new WaitUntil(() => MapGenerator.IsReady);

            Debug.Log("Updating visible chunks.");
            UpdateVisibleChunks();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            //if (VoronoiTextureWorker.UseGizmoColors)
            //    VoronoiTextureWorker.UpdateColorArray();

            yield return new WaitUntil(() => transform.childCount > 1);

            Transform transform1;
            Debug.Log($"Active chunks: {(transform1 = transform).GetActiveChildCount()} || Child Count: {transform1.childCount}");

            yield return new WaitUntil(() => _visibleTerrainChunks.Count > 0);

            MapGenerator.Instance.UnfreezePlayer();

            //foreach (var position in VoronoiTextureWorker.UnrelatedPositions)
            //{
            //    var nearestTransform = transform1.FindClosestTarget(position);
            //    Debug.Log($"Nearest object to '{position}' is '{nearestTransform.name}'!");
            //}
        }

        private void UpdateVisibleChunks()
        {
            var alreadyUpdatedChunkCoords = new HashSet<Vector2>();
            for (var i = _visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedChunkCoords.Add(_visibleTerrainChunks[i].Coord);
                _visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            Debug.Log($"Visible chunks: {alreadyUpdatedChunkCoords.Count}");

            var currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _meshWorldSize);
            var currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _meshWorldSize);

            Debug.Log($"Iterating offset (x/z) from {-_chunksVisibleInViewDst} to {_chunksVisibleInViewDst} ({_chunksVisibleInViewDst * 4})");

            for (var yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++)
                for (var xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++)
                {
                    var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                    if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                    {
                        if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        {
                            //Debug.Log($"[{viewedChunkCoord.ToRoundedString()}] Updating already loaded chunk.");
                            _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                        }
                        else
                        {
                            //Debug.Log($"[{viewedChunkCoord}] Loading chunk.");

                            var newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels,
                                colliderLodIndex, transform, mapMaterial);
                            _terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                            newChunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
                            newChunk.Load();
                        }
                    }
                }

            OnTerrainUpdated();
        }

        private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
        {
            //Debug.Log($"Changed visibility from '{chunk}' to {(isVisible ? "visible" : "hidden")}");
            if (isVisible)
                _visibleTerrainChunks.Add(chunk);
            else
                _visibleTerrainChunks.Remove(chunk);
        }
    }
}