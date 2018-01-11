using UnityEditor;
using UnityEngine;

namespace mFramework
{
    public static class mEditor
    {
        private static bool _isLock;

        static mEditor()
        {
        
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Attach()
        {
            EditorApplication.update += EditorUpdate;

            mCore.ApplicationQuitEvent += OnApplicationQuitEvent;
            mCore.Log("[mEditor] InitializeOnLoad");

            _isLock = false;
        }

        private static void OnApplicationQuitEvent()
        {
            EditorApplication.update -= EditorUpdate;
            
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

        private static void LockReload()
        {
            _isLock = true;
            EditorApplication.LockReloadAssemblies();
            mCore.Log("[mFramework]: LockReloadAssemblies");
        }

        private static void UnlockReload()
        {
            _isLock = false;
            EditorApplication.UnlockReloadAssemblies();
            AssetDatabase.Refresh();
            mCore.Log("[mFramework]: UnlockReloadAssemblies");
        }
    }
}
