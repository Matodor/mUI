using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace mFramework
{
    internal class EditorExtension
    {
        private delegate bool EditorGetBoolean();

        private bool _isLockReloadAssemblies;

        private Type _editorApplication;
        private Type _callbackFunctionType;

        private EditorGetBoolean _editorIsPlaying;
        private EditorGetBoolean _editorIsCompiling;
        private Assembly _unityEditorAssembly;

        private Delegate _updateEditorDelegate;

        private FieldInfo _playmodeStateChangedField;
        private FieldInfo _updateField;

        public EditorExtension()
        {
            Attach();
        }

        private void Attach()
        {
            _unityEditorAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name == "UnityEditor");

            if (_unityEditorAssembly == null)
                return;

            _editorApplication = _unityEditorAssembly.GetType("UnityEditor.EditorApplication");
            mCore.Log(_editorApplication.FullName);
            mCore.Log("[mFramework] EditorExtension attach");

            var isCompilingProperty = _editorApplication.GetProperty("isCompiling", BindingFlags.Static | BindingFlags.Public);
            var isPlayingProperty = _editorApplication.GetProperty("isPlaying", BindingFlags.Static | BindingFlags.Public);

            _editorIsCompiling =
                (EditorGetBoolean)
                Delegate.CreateDelegate(typeof(EditorGetBoolean), isCompilingProperty.GetGetMethod());

            _editorIsPlaying =
                (EditorGetBoolean)
                Delegate.CreateDelegate(typeof(EditorGetBoolean), isPlayingProperty.GetGetMethod());

            _playmodeStateChangedField = _editorApplication.GetField("playmodeStateChanged", BindingFlags.Static | BindingFlags.Public);
            _updateField = _editorApplication.GetField("update", BindingFlags.Static | BindingFlags.Public);

            _callbackFunctionType = _editorApplication.GetNestedType("CallbackFunction", BindingFlags.Public);
            _updateEditorDelegate = Delegate.CreateDelegate(_callbackFunctionType, this, "EditorUpdate", false);

            _updateField?
                .SetValue(null, Delegate.Combine((Delegate)_updateField.GetValue(null), _updateEditorDelegate));
        }

        private void EditorUpdate()
        {
            if (_editorIsCompiling() && !_isLockReloadAssemblies)
                LockReloadAssemblies();
        } 

        private void LockReloadAssemblies()
        {
            _isLockReloadAssemblies = true;
            _editorApplication.GetMethod("LockReloadAssemblies", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
            mCore.Log("[mFramework]: LockReloadAssemblies");
        }

        private void UnlockReloadAssemblies()
        {
            _isLockReloadAssemblies = false;
            _editorApplication.GetMethod("UnlockReloadAssemblies", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
            var assetDatabaseType = _unityEditorAssembly.GetType("UnityEditor.AssetDatabase");
            assetDatabaseType.GetMethod("Refresh", new Type[] { }).Invoke(null, null);
            mCore.Log("[mFramework]: UnlockReloadAssemblies");
        }

        public void Detach()
        {
            _updateField?.SetValue(null, Delegate.Remove((Delegate)_updateField.GetValue(null), _updateEditorDelegate));
            if (_isLockReloadAssemblies)
                UnlockReloadAssemblies();
        }
    }
}
