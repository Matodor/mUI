using mFramework.UI;
using UnityEditor;

namespace mFramework
{
    public class UILabelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.IntSlider("test", 50, 0, 100);
        }
    }
}