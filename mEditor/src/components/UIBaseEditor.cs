using mFramework.UI;
using UnityEditor;
using UnityEngine;

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
            var active = EditorGUILayout.Toggle("IsActive", _object.IsActive);
            if (active != _object.IsActive)
            {
                if (active)
                    _object.Enable();
                else
                    _object.Disable();
            }

            var show = EditorGUILayout.Toggle("IsShowing", _object.IsShowing);
            if (show != _object.IsShowing)
            {
                if (show)
                    _object.Show();
                else
                    _object.Hide();
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LongField("GUID", (long)_object.GUID);
            EditorGUILayout.IntField("Sorting order", _object.SortingOrder());
            EditorGUI.EndDisabledGroup();

            var localSorting = EditorGUILayout.IntField("Local sorting order", _object.LocalSortingOrder());
            if (localSorting != _object.LocalSortingOrder())
            {
                _object.SortingOrder(localSorting);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}