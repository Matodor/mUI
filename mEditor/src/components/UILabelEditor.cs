using System.Linq;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework
{
    [CustomEditor(typeof(UILabel))]
    public class UILabelEditor : UIBaseEditor
    {
        private UILabel _label;
        private bool _showFormattings;

        public override void Awake()
        {
            base.Awake();
            _label = target as UILabel;
            _showFormattings = false;
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
                EditorGUILayout.LabelField("Text");
                var text = EditorGUILayout.TextArea(_label.Text);
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
                style.TextAlignment = (TextAlignment) EditorGUILayout.EnumPopup("Alignment", 
                    style.TextAlignment, GUIStyle.none);
                style.FontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font style",
                    style.FontStyle, GUIStyle.none);

                if (EditorGUI.EndChangeCheck())
                    _label.TextStyle = style;
            }

            EditorGUILayout.Space();
            _showFormattings = EditorGUILayout.Foldout(_showFormattings, "TextFormattings");
            if (_showFormattings)
            {
                foreach (var kvp in _label.TextFormatting.ToArray())
                {
                    EditorGUILayout.LabelField($"{kvp.Key}");
                    EditorGUI.indentLevel += 1;

                    EditorGUI.BeginChangeCheck();
                    var formatting = kvp.Value;

                    formatting.Color = EditorGUILayout.ColorField("Color", 
                        formatting.Color ?? _label.Color);

                    formatting.FontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font style", 
                        formatting.FontStyle ?? _label.TextStyle.FontStyle, GUIStyle.none);

                    formatting.LetterSpacing = EditorGUILayout.FloatField("Letter spacing", 
                        formatting.LetterSpacing ?? _label.TextStyle.LetterSpacing);

                    formatting.Size = EditorGUILayout.IntSlider("Text size", 
                        formatting.Size ?? _label.TextStyle.Size, 1, 128);

                    formatting.WordSpacing = EditorGUILayout.FloatField("Word spacing", 
                        formatting.WordSpacing ?? _label.TextStyle.WordSpacing);

                    if (EditorGUI.EndChangeCheck())
                        _label[kvp.Key] = formatting;

                    EditorGUI.indentLevel -= 1;
                    EditorGUILayout.Space();
                }
            }
        }
    }
}