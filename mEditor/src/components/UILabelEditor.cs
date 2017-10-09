using mFramework.UI;
using UnityEditor;

namespace mFramework
{
    [CustomEditor(typeof(UILabel), true)]
    public class UILabelEditor : Editor
    {
        private UILabel _label;

        private void Awake()
        {
            _label = (UILabel) target;
        }

        public override void OnInspectorGUI()
        {
            DrawTextColor();
        }

        private void DrawTextColor()
        {
            _label.SetColor(EditorGUILayout.ColorField("Text color", _label.GetColor()));
        }
    }
}