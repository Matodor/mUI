using System;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework
{
    public static class mEditor
    {
        private static bool _isLock;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            EditorApplication.update += EditorUpdate;
            mCore.ApplicationQuit += OnApplicationQuit;
            mUI.UIObjectCreated += OnUiObjectCreated;
            Debug.Log("[mEditor] InitializeOnLoad");

            _isLock = false;
        }

        private static void OnUiObjectCreated(UIObject uiObject)
        {
            EditorUtility.SetDirty(uiObject.gameObject);
        }

        private static void OnApplicationQuit()
        {
            EditorApplication.update -= EditorUpdate;
            mCore.ApplicationQuit -= OnApplicationQuit;
            mUI.UIObjectCreated -= OnUiObjectCreated;
            Debug.Log("[mEditor] OnApplicationQuit");

            if (_isLock)
                UnlockReload();
        }

        private static void EditorUpdate()
        {
            if (!_isLock && EditorApplication.isCompiling)
            {
                LockReload();
            }
        }

        [MenuItem("mFramework/LockReload")]
        private static void LockReload()
        {
            _isLock = true;
            EditorApplication.LockReloadAssemblies();
            Debug.Log("[mFramework]: LockReloadAssemblies");
        }

        [MenuItem("mFramework/UnlockReload")]
        private static void UnlockReload()
        {
            _isLock = false;
            EditorApplication.UnlockReloadAssemblies();
            AssetDatabase.Refresh();
            Debug.Log("[mFramework]: UnlockReloadAssemblies");
        }
    }
}
