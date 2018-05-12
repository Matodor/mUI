using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework
{
    [CustomEditor(typeof(UILabel))]
    public class UILabelEditor : UIBaseEditor
    {
        private UILabel _label;

        public override void Awake()
        {
            base.Awake();
            _label = target as UILabel;
        }

        protected override void OnHeaderGUI()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            {
                EditorGUI.BeginChangeCheck();
                var color = EditorGUILayout.ColorField("Text color", _label.Color);
                if (EditorGUI.EndChangeCheck())
                    _label.Color = color;
            }

            {
                EditorGUI.BeginChangeCheck();
                var text = EditorGUILayout.TextField("Text", _label.Text);
                if (EditorGUI.EndChangeCheck())
                    _label.Text = text;
            }

            {
                EditorGUI.BeginChangeCheck();
                var style = _label.TextStyle;
                style.Size = EditorGUILayout.IntSlider("Text size", style.Size, 1, 128);
                style.LetterSpacing = EditorGUILayout.FloatField("Letter spacing", style.LetterSpacing);
                style.WordSpacing = EditorGUILayout.FloatField("Word spacing", style.WordSpacing);
                style.LinesSpacing = EditorGUILayout.FloatField("Lines spacing", style.LinesSpacing);
                style.TextAlignment = (TextAlignment) EditorGUILayout
                    .EnumPopup("Alignment", style.TextAlignment, GUIStyle.none);
                
                if (EditorGUI.EndChangeCheck())
                    _label.TextStyle = style;
            }
        }
    }
}