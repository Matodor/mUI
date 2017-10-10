using System;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework
{
    public class UIObjectEditor : Editor
    {
        private UIAnimation _currentSelected;

        protected virtual void Awake()
        {
            //EditorApplication.update += UpdateEditor;
        }

        /*
        private void UpdateEditor()
        {
            if (Event.current != null && Event.current.type == EventType.MouseUp)
            {
                if (_currentSelected != null)
                    _currentSelected.State = UIAnimationState.PLAYING;
            }
        }

        protected void OnDestroy()
        {
            EditorApplication.update -= UpdateEditor;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            if (EditorGUILayout.Foldout(true, "Animations"))
            {
                var animations = ((UIObject) target).Animations;
                for (int i = 0; i < animations.Count; i++)
                {
                    if (EditorGUILayout.BeginFadeGroup(1f))
                    {
                        var t = EditorGUILayout.Slider(animations[i].CurrentTime, 0f, 1f);
                        var lastId = GUIUtility.GetControlID(FocusType.Passive) - 1;

                        if (GUIUtility.hotControl == lastId)
                        {
                            if (_currentSelected == null)
                            {
                                _currentSelected = animations[i];
                                _currentSelected.State = UIAnimationState.STOPPED;
                            }

                            animations[i].SetAnimationPos(t);
                            animations[i].Animate();
                        }
                        else
                        {
                            if (_currentSelected != null)
                            {
                                _currentSelected.State = UIAnimationState.PLAYING;
                                _currentSelected = null;
                            }
                        }

                    }
                    EditorGUILayout.EndFadeGroup();
                }
            }
        }*/
    }
}