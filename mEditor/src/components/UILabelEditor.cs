using mFramework.UI;
using UnityEditor;

namespace mFramework
{
    [CustomEditor(typeof(UILabel), true)]
    public class UILabelEditor : UIObjectEditor
    {
        private UILabel _label;

        protected override void Awake()
        {
            base.Awake();
            _label = (UILabel) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _label.SetColor(EditorGUILayout.ColorField("Text color", _label.GetColor()));
            _label.SetFontSize(EditorGUILayout.IntSlider("Text size", _label.Size, 1, 128));
        }

        private void DrawTextColor()
        {
            _label.SetColor(EditorGUILayout.ColorField("Text color", _label.GetColor()));
        }
    }
}