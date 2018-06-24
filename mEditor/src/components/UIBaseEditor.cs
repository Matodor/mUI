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

        [DrawGizmo(GizmoType.Selected)]
        //[DrawGizmo(GizmoType.InSelectionHierarchy)]
        private static void DrawGizmo(UIObject source, GizmoType gizmoType)
        {
            DrawUnscaledRect(source);
            DrawLocalRect(source);
            DrawGlobalRect(source);
        }

        private static void DrawGlobalRect(UIObject source)
        {
            DrawTestRect(source.UIRect(UIRectType.GLOBAL), Color.red);
            Gizmos.DrawWireSphere(source.CenterPosition(), 0.1f);
            Gizmos.DrawWireSphere(source.GlobalAnchorPosition(UIObject.PivotByAnchor(source.Anchor)), 0.1f);
        }

        private static void DrawLocalRect(UIObject source)
        {
            DrawTestRect(source.UIRect(UIRectType.LOCAL), Color.yellow);
            Gizmos.DrawWireSphere(source.CenterLocalPosition(), 0.1f);
            Gizmos.DrawWireSphere(source.LocalAnchorPosition(UIObject.PivotByAnchor(source.Anchor)), 0.1f);
        }

        private static void DrawUnscaledRect(UIObject source)
        {
            DrawTestRect(source.UIRect(UIRectType.UNSCALED), Color.blue);
        }

        private static void DrawTestRect(UIRect rect, Color color) 
        {
            Gizmos.color = color;
            //.DrawWireSphere(rect.MiddleCenter, 0.1f);
            //Gizmos.DrawWireCube(rect.AnchorPosition, new Vector3(0.1f, 0.1f, 0.1f));

            Gizmos.DrawLine(rect.UpperLeft, rect.UpperRight);
            Gizmos.DrawLine(rect.UpperRight, rect.LowerRight);
            Gizmos.DrawLine(rect.LowerRight, rect.LowerLeft);
            Gizmos.DrawLine(rect.LowerLeft, rect.UpperLeft);
        }

        public override void OnInspectorGUI()
        {
            //EditorGUILayout.Vector2Field("RR", _object.Transform.localPosition);
            
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

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Vector2Field("GlobalScale", _object.GlobalScale);
            EditorGUI.EndDisabledGroup();

            {
                EditorGUI.BeginChangeCheck();
                var padding = new Vector4(
                    _object.Padding.Top,
                    _object.Padding.Right,
                    _object.Padding.Bottom,
                    _object.Padding.Left
                );
                padding = EditorGUILayout.Vector4Field("Padding", padding);
                if (EditorGUI.EndChangeCheck())
                {
                    _object.Padding = new UIPadding()
                    {
                        Top = padding.x,
                        Right = padding.y,
                        Bottom = padding.z,
                        Left = padding.w,
                    };
                }
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

            {
                EditorGUI.BeginChangeCheck();
                var active = EditorGUILayout.Toggle("IsActive", _object.IsActive);
                if (EditorGUI.EndChangeCheck())
                {
                    if (active)
                        _object.Enable();
                    else
                        _object.Disable();
                }
            }

            {
                EditorGUI.BeginChangeCheck();
                var show = EditorGUILayout.Toggle("IsShowing", _object.IsShowing);
                if (EditorGUI.EndChangeCheck())
                {
                    if (show)
                        _object.Show();
                    else
                        _object.Hide();
                }
            }

            if (_object is IUIClickable clickable)
            {
                EditorGUI.BeginChangeCheck();
                var ignoreByHandler = EditorGUILayout.Toggle("IgnoreByHandler", clickable.IgnoreByHandler);
                if (EditorGUI.EndChangeCheck())
                    clickable.IgnoreByHandler = ignoreByHandler;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LongField("GUID", (long) _object.GUID);

            EditorGUILayout.Vector2Field("Width Height", new Vector2(_object.Width, _object.Height));
            EditorGUILayout.Vector2Field("LocalWidth LocalHeight", new Vector2(_object.LocalWidth, _object.LocalHeight));
            EditorGUILayout.Vector2Field("UnscaledWidth UnscaledHeight", new Vector2(_object.UnscaledWidth, _object.UnscaledHeight));
            EditorGUILayout.Vector2Field("SizeX SizeY", new Vector2(_object.SizeX, _object.SizeY));

            //EditorGUILayout.Vector2Field("CenterOffset", _object.CenterOffset);
            EditorGUILayout.Vector2Field("UnscaledCenterOffset", _object.UnscaledCenterOffset);
            EditorGUILayout.IntField("Sorting order", _object.SortingOrder);
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
                        var easingType = (EasingFunctions) EditorGUILayout.EnumPopup("EasingType", animation.EasingType, GUIStyle.none);
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
                $"{vector3.x.ToString("F4").Replace(",", ".")}f," +
                $"{vector3.y.ToString("F4").Replace(",", ".")}f" +
            ")";
        }
    }
}