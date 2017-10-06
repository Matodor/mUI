using System;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework
{
    public class BehaviourObject : MonoBehaviour
    {
        private Editor _editor;

        public void Awake()
        {
            //mCore.Log(gameObject.name);
            //Selection.selectionChanged
            //_editor = Editor.CreateEditor(gameObject.transform, typeof(UILabelEditor));
        }

        public void OnGUI()
        {
            //_editor.OnInspectorGUI();
        }
    }

    public sealed class mEditor
    {
        public static mEditor Instance { get; }

        static mEditor()
        {
            if (Instance == null)
                Instance = new mEditor();
        }

        public mEditor()
        {
            mUI.UIObjectCreated += ObjectCreated;
            mCore.Log("mEditor created");

            EditorApplication.update += Update;
            Selection.selectionChanged += SelectionChanged;
        }

        private void SelectionChanged()
        {
            mCore.Log("s = ", Selection.activeContext.GetType().FullName);
        }

        private void Update()
        {
        }

        private void ObjectCreated(UIObject uiObject)
        {
            if (uiObject is UILabel)
            {
                var target = uiObject.GameObject.AddComponent<BehaviourObject>();
                //UILabelEditor.
                //new UILabelEditor().target = t;
                // Editor.CreateEditor(t, typeof(UILabelEditor));
            }
        }

        public static void Init()
        {
            
        }
    }
}
