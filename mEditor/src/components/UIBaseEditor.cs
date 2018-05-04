using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework
{
    [CustomEditor(typeof(UIObject), true)]
    public class UIBaseEditor : Editor
    {
        private UIObject _object;

        public virtual void Awake()
        {
            _object = target as UIObject;
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(UIObject source, GizmoType gizmoType)
        {
            var rect = source.Rect;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rect.TopLeft, rect.TopRight);
            Gizmos.DrawLine(rect.TopRight, rect.BottomRight);
            Gizmos.DrawLine(rect.BottomRight, rect.BottomLeft);
            Gizmos.DrawLine(rect.BottomLeft, rect.TopLeft);
        }

        public override void OnInspectorGUI()
        {
            {
                EditorGUI.BeginChangeCheck();
                var pos = EditorGUILayout.Vector2Field("Position", _object.Position);
                if (EditorGUI.EndChangeCheck())
                    _object.Position = pos;
            }

            {
                EditorGUI.BeginChangeCheck();
                var scale = EditorGUILayout.Vector2Field("Scale", _object.Scale);
                if (EditorGUI.EndChangeCheck())
                    _object.Scale = scale;
            }

            {
                EditorGUI.BeginChangeCheck();
                var rotation = EditorGUILayout.FloatField("Rotation", _object.Rotation);
                if (EditorGUI.EndChangeCheck())
                    _object.Rotation = rotation;
            }

            var active = EditorGUILayout.Toggle("IsActive", _object.IsActive);
            if (active != _object.IsActive)
            {
                if (active)
                    _object.Enable();
                else
                    _object.Disable();
            }

            var show = EditorGUILayout.Toggle("IsShowing", _object.IsShowing);
            if (show != _object.IsShowing)
            {
                if (show)
                    _object.Show();
                else
                    _object.Hide();
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LongField("GUID", (long)_object.GUID);
            EditorGUILayout.IntField("Sorting order", _object.SortingOrder());
            EditorGUI.EndDisabledGroup();

            var localSorting = EditorGUILayout.IntField("Local sorting order", _object.LocalSortingOrder());
            if (localSorting != _object.LocalSortingOrder())
            {
                _object.SortingOrder(localSorting);
            }

            {
                EditorGUI.BeginChangeCheck();
                var anchor = (UIAnchor) EditorGUILayout.EnumPopup("Anchor", _object.Anchor, GUIStyle.none);

                if (EditorGUI.EndChangeCheck())
                    _object.Anchor = anchor;
            }
        }
    }
}