using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace uzSurfaceMapper.Utils.Terrains
{
    using Structs;
    using Data;

    public class MapPreview : MonoBehaviour
    {
        public enum DrawMode
        {
            NoiseMap,
            Mesh,
            FalloffMap
        }

        public bool autoUpdate;
        public DrawMode drawMode;

        [FormerlySerializedAs("editorPreviewLOD")]
        [Range(0, MeshSettings.NumSupportedLoDs - 1)]
        public int editorPreviewLod;

        public HeightMapSettings heightMapSettings;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public MeshSettings meshSettings;

        public Material terrainMaterial;
        public TextureData textureData;

        public Renderer textureRender;

        public void DrawMapInEditor()
        {
            throw new NotImplementedException();

            //textureData.ApplyToMaterial(terrainMaterial);
            //textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);
            //var heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.NumVertsPerLine, meshSettings.NumVertsPerLine,
            //    heightMapSettings, Vector2.zero);

            //if (drawMode == DrawMode.NoiseMap)
            //    DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
            //else if (drawMode == DrawMode.Mesh)
            //    DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.Values, meshSettings, editorPreviewLod));
            //else if (drawMode == DrawMode.FalloffMap)
            //    DrawTexture(TextureGenerator.TextureFromHeightMap(
            //        new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.NumVertsPerLine), 0, 1)));
        }

        public void DrawTexture(Texture2D texture)
        {
            textureRender.sharedMaterial.mainTexture = texture;
            textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

            textureRender.gameObject.SetActive(true);
            meshFilter.gameObject.SetActive(false);
        }

        public void DrawMesh(MeshData meshData)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();

            textureRender.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
        }

        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
                DrawMapInEditor();
        }

        private void OnTextureValuesUpdated()
        {
            textureData.ApplyToMaterial(terrainMaterial);
        }

        private void OnValidate()
        {
            if (meshSettings != null)
            {
                meshSettings.OnValuesUpdated -= OnValuesUpdated;
                meshSettings.OnValuesUpdated += OnValuesUpdated;
            }

            if (heightMapSettings != null)
            {
                heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }

            if (textureData != null)
            {
                textureData.OnValuesUpdated -= OnTextureValuesUpdated;
                textureData.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }
    }
}