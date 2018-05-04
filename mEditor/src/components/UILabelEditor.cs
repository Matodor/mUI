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

            _label.SetColor(EditorGUILayout.ColorField("Text color", _label.GetColor()));
            _label.SetText(EditorGUILayout.TextField("Text", _label.Text));
            _label.SetFontSize(EditorGUILayout.IntSlider("Text size", _label.Size, 1, 128));

            _label.SetLetterSpacing(EditorGUILayout.FloatField("Letter spacing", _label.LetterSpacing));
            _label.SetWordSpacing(EditorGUILayout.FloatField("Word spacing", _label.WordSpacing));
            _label.SetLinesSpacing(EditorGUILayout.FloatField("Lines spacing", _label.LinesSpacing));

            _label.SetTextAnchor((TextAnchor)EditorGUILayout.EnumPopup("Anchor", _label.TextAnchor, GUIStyle.none));
            _label.SetTextAlignment((TextAlignment)EditorGUILayout.EnumPopup("Alignment", _label.TextAlignment, GUIStyle.none));
        }
    }
}