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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _label.SetText(EditorGUILayout.TextField("Text", _label.Text));
            _label.SetColor(EditorGUILayout.ColorField("Text color", _label.GetColor()));
            _label.SetFontSize(EditorGUILayout.IntSlider("Text size", _label.Size, 1, 128));

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}