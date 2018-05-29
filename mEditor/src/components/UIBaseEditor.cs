using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework.UI
{
    [CustomEditor(typeof(UIObject), true)]
    public class UIBaseEditor : Editor
    {
        private UIObject _object;
        private bool _showAnimations;

        public virtual void Awake()
        {
            _showAnimations = false;
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

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(rect.Center, 0.1f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(rect.Anchor, new Vector3(0.1f, 0.1f, 0.1f));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Vector2Field("RR", _object.Transform.localPosition);
            
            {
                EditorGUI.BeginChangeCheck();
                var pos = EditorGUILayout.Vector2Field("Position", _object.Position);
                if (EditorGUI.EndChangeCheck())
                    _object.Position = pos;
            }
            
            {
                EditorGUI.BeginChangeCheck();
                var pos = EditorGUILayout.Vector2Field("Local position", _object.LocalPosition);
                if (EditorGUI.EndChangeCheck())
                    _object.LocalPosition = pos;
            }

            {
                EditorGUI.BeginChangeCheck();
                var scale = EditorGUILayout.Vector2Field("Scale", _object.Scale);
                if (EditorGUI.EndChangeCheck())
                    _object.Scale = scale;
            }
            EditorGUILayout.Space();
            {
                EditorGUI.BeginChangeCheck();
                var rotation = EditorGUILayout.FloatField("Rotation", _object.Rotation);
                if (EditorGUI.EndChangeCheck())
                    _object.Rotation = rotation;
            }

            {
                EditorGUI.BeginChangeCheck();
                var rotation = EditorGUILayout.FloatField("Local Rotation", _object.LocalRotation);
                if (EditorGUI.EndChangeCheck())
                    _object.LocalRotation = rotation;
            }
            EditorGUILayout.Space();
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
            EditorGUILayout.LongField("GUID", (long) _object.GUID);
            EditorGUILayout.IntField("Sorting order", _object.SortingOrder);

            EditorGUILayout.Vector2Field("CenterOffset", _object.CenterOffset);
            EditorGUILayout.Vector2Field("UnscaledCenterOffset", _object.UnscaledCenterOffset);

            //EditorGUILayout.Vector3Field("AnchorLocalPosition", _object.AnchorLocalPosition(_object.AnchorPivot, out _));
            //EditorGUILayout.Vector3Field("AnchorPosition", _object.AnchorPosition(_object.AnchorPivot, out _));
            EditorGUI.EndDisabledGroup();

            {
                EditorGUI.BeginChangeCheck();
                var sorting = EditorGUILayout.IntField("Local sorting order", _object.LocalSortingOrder);
                if (EditorGUI.EndChangeCheck())
                    _object.SortingOrder = sorting;
            }

            {
                EditorGUI.BeginChangeCheck();
                var anchor = (UIAnchor) EditorGUILayout.EnumPopup("Anchor", _object.Anchor, GUIStyle.none);
                if (EditorGUI.EndChangeCheck())
                    _object.Anchor = anchor;
            }

            if (GUILayout.Button("Copy Position"))
                GUIUtility.systemCopyBuffer = Vector2(_object.Position);
            if (GUILayout.Button("Copy LocalPos"))
                GUIUtility.systemCopyBuffer = Vector2(_object.LocalPosition);

            _showAnimations = EditorGUILayout.Foldout(_showAnimations, "Animations");
            if (_showAnimations)
            {
                foreach (var animation in _object.Animations)
                {
                    EditorGUILayout.LabelField(animation.ToString());
                    EditorGUI.indentLevel += 1;
                    {
                        EditorGUI.BeginChangeCheck();
                        var easingType = (EasingType)EditorGUILayout.EnumPopup("EasingType", animation.EasingType, GUIStyle.none);
                        if (EditorGUI.EndChangeCheck())
                            animation.EasingType = easingType;
                    }
                    {
                        EditorGUI.BeginChangeCheck();
                        var duration = EditorGUILayout.FloatField("Duration", animation.Duration);
                        if (EditorGUI.EndChangeCheck())
                            animation.Duration = duration;
                    }
                    EditorGUI.indentLevel -= 1;
                    EditorGUILayout.Space();
                }
            }
        }

        private static string Vector2(Vector3 vector3)
        {
            return "new Vector2(" +
                $"{vector3.x.ToString("F").Replace(",", ".")}f," +
                $"{vector3.y.ToString("F").Replace(",", ".")}f" +
            ")";
        }
    }
}