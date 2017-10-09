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
            DrawTextColor();
        }

        private void DrawTextColor()
        {
            _label.SetColor(EditorGUILayout.ColorField("Text color", _label.GetColor()));
        }
    }
}