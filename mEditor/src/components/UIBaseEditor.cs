using mFramework.UI;
using UnityEditor;

namespace mFramework
{
    [CustomEditor(typeof(UIObject), true)]
    public class UIBaseEditor : Editor
    {
        private UIObject _object;

        public virtual void Awake()
        {
            _object = target as UIObject;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Toggle("IsActive", _object.IsActive);
            EditorGUILayout.Toggle("IsShowing", _object.IsShowing);

            EditorGUILayout.IntField("Sorting order", _object.SortingOrder());
            var s = EditorGUILayout.IntField("Local sorting order", _object.LocalSortingOrder());
            _object.SortingOrder(s);
        }
    }
}