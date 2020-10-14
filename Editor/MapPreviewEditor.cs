using UnityEditor;
using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains.Editor
{
    [CustomEditor(typeof(MapPreview))]
    public class MapPreviewEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var mapPreview = (MapPreview)target;

            if (DrawDefaultInspector())
                if (mapPreview.autoUpdate)
                    mapPreview.DrawMapInEditor();

            if (GUILayout.Button("Generate"))
                mapPreview.DrawMapInEditor();
        }
    }
}