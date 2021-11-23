using NestedScroll.ScrollElement;
using UnityEditor;
using UnityEngine;

namespace NestedScroll.Editor
{
    [CustomEditor(typeof(SnapScrolling))]
    public class SnapScrollingEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("UpdateScroll"))
            {
                SnapScrolling t = (SnapScrolling) target;
                t.UpdateScroll();
            }
        }
    }
}
