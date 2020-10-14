using UnityEditor;
using UnityEngine;

namespace uzSurfaceMapper.Utils.Terrains.Editor
{
    using Data;

    [CustomEditor(typeof(UpdatableData), true)]
    public class UpdatableDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var data = (UpdatableData)target;

            if (GUILayout.Button("Update"))
            {
                data.NotifyOfUpdatedValues();
                EditorUtility.SetDirty(target);
            }
        }
    }
}