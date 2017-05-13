using System;
using System.Linq;
using System.Reflection;
using mFramework.UI;
using UnityEditor;
using UnityEngine;

namespace mFramework
{
    public sealed class mCore : ITicking
    {
        public static mCore Instance { get; private set; }
        public static bool IsDebug { get; set; }
        public static bool IsEditor { get; private set; }

        private readonly mEngine _engine;

        private mCore()
        {
            _engine = new GameObject("mFrameworkCore").AddComponent<mEngine>();
            
            Instance = this;

            if (Application.isEditor)
                InejctEditor();

            Log("[mFramework] init");
        }

        public static mCore Init()
        {
            if (Instance != null)
                throw new Exception("mFramework already created");
            return new mCore();
        }

        private delegate bool EditorGetBoolean();

        private Type _editorApplication, _callbackFunctionType;
        private bool _editorIsLocked;
        private EditorGetBoolean _editorIsPlaying;
        private EditorGetBoolean _editorIsCompiling;
        private Assembly _unityEditorAssembly;
        private Delegate _playmodeStateChangedDelegate, _updateEditorDelegate;
        private FieldInfo _playmodeStateChangedField, _updateField;

        private void InejctEditor()
        {
            _unityEditorAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name == "UnityEditor");
            if (_unityEditorAssembly == null)
                return;

            IsDebug = true;
            IsEditor = true;

            _editorApplication = _unityEditorAssembly.GetType("UnityEditor.EditorApplication");
            Log(_editorApplication.FullName);
            Log("[mFramework] InejctEditor");

            var isCompilingProperty = _editorApplication.GetProperty("isCompiling",
                        BindingFlags.Static | BindingFlags.Public);
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
            _playmodeStateChangedDelegate = Delegate.CreateDelegate(_callbackFunctionType, this, "EditorPlaymodeStateChanged", false);
            
            _updateField?
                .SetValue(null, Delegate.Combine((Delegate) _updateField.GetValue(null), _updateEditorDelegate));
            _playmodeStateChangedField?
                .SetValue(null, Delegate.Combine((Delegate)_playmodeStateChangedField.GetValue(null), _playmodeStateChangedDelegate));
        }

        private void EditorUpdate()
        {
            if (_editorIsCompiling() && !_editorIsLocked)
            {
                _editorIsLocked = true;
                _editorApplication.GetMethod("LockReloadAssemblies", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
                Log("[mFramework]: LockReloadAssemblies");
            }

        }

        private void EditorPlaymodeStateChanged()
        {
            if (!_editorIsPlaying())
            {
                _playmodeStateChangedField?
                    .SetValue(null, Delegate.Remove((Delegate)_playmodeStateChangedField.GetValue(null), _playmodeStateChangedDelegate));

                if (_editorIsLocked)
                {
                    _editorApplication.GetMethod("UnlockReloadAssemblies", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
                    var assetDatabaseType = _unityEditorAssembly.GetType("UnityEditor.AssetDatabase");
                    assetDatabaseType.GetMethod("Refresh", new Type[] {}).Invoke(null, null);
                    Log("[mFramework]: UnlockReloadAssemblies");
                }
            }
        }

        public static void Log(string format, params object[] obj)
        {
            if (IsEditor && IsDebug)
                UnityEngine.Debug.Log(string.Format(format, obj));
        }

        public void Tick()
        {
            /*
                if (UnityEditor.EditorApplication.isCompiling && (UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused))
                {
                    Debug.Log("[mFramework] Compiled during play");
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            */

            mUI.Instance?.Tick();
        }

        public void FixedTick()
        {
            mUI.Instance?.FixedTick();
        }

        public void LateTick()
        {
            mUI.Instance?.LateTick();
        }
    }
}
