using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public delegate object LateBoundFieldGet(object target);
    public delegate void LateBoundFieldSet(object target, object value);

    public sealed class mCore
    {
        public static mCore Instance
        {
            get { return _instance ?? (_instance = new mCore()); }
        }

        public static bool IsDebug { get; set; }
        public static bool IsEditor { get; private set; }
        public static event Action OnApplicationQuitEvent;

        private static mCore _instance;
        private readonly Dictionary<Type, CachedFieldsInfo> _fieldDictionary;
        private readonly UnidirectionalList<RepeatAction> _repeatsActions;

        private delegate bool EditorGetBoolean();

        private Type _editorApplication, _callbackFunctionType;
        private bool _editorIsLocked;
        private EditorGetBoolean _editorIsPlaying;
        private EditorGetBoolean _editorIsCompiling;
        private Assembly _unityEditorAssembly;
        private Delegate _playmodeStateChangedDelegate, _updateEditorDelegate;
        private FieldInfo _playmodeStateChangedField, _updateField;
        
        private mCore()
        {
            _repeatsActions = UnidirectionalList<RepeatAction>.Create();
            _fieldDictionary = new Dictionary<Type, CachedFieldsInfo>();
            
            var engine = new GameObject("mFramework").AddComponent<mEngine>();
            engine.transform.position = new Vector3(0, 0, 9999);

            if (Application.isEditor)
                InejctEditor();

            Log("[mFramework] init");
        }
         
        ~mCore()
        {
            _editorIsLocked = false;
        }

        internal void Init()
        {
            
        }

        internal bool RemoveRepeatAction(RepeatAction repeatAction)
        {
            return _repeatsActions.Remove(repeatAction);
        }

        internal void AddRepeatAction(RepeatAction repeatAction)
        {
            _repeatsActions.Add(repeatAction);
        }

        internal static void OnApplicationQuit()
        {
            OnApplicationQuitEvent?.Invoke();
            Log("[mCore] OnApplicationQuit");
        }

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

        public static LateBoundFieldGet CreateFieldGetter(FieldInfo field)
        {
            var method = new DynamicMethod("Get" + field.Name, typeof(object), new[] { typeof(object) }, field.DeclaringType, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, field.DeclaringType); // Cast to source type
            gen.Emit(OpCodes.Ldfld, field);

            if (field.FieldType.IsValueType)
                gen.Emit(OpCodes.Box, field.FieldType);

            gen.Emit(OpCodes.Ret);

            var callback = (LateBoundFieldGet)method.CreateDelegate(typeof(LateBoundFieldGet));
            return callback;
        }

        public static LateBoundFieldSet CreateFieldSetter(FieldInfo field)
        {
            var method = new DynamicMethod("Set" + field.Name, null, new[] { typeof(object), typeof(object) }, field.DeclaringType, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0); // Load target to stack
            gen.Emit(OpCodes.Castclass, field.DeclaringType); // Cast target to source type
            gen.Emit(OpCodes.Ldarg_1); // Load value to stack
            gen.Emit(OpCodes.Unbox_Any, field.FieldType); // Unbox the value to its proper value type
            gen.Emit(OpCodes.Stfld, field); // Set the value to the input field
            gen.Emit(OpCodes.Ret);
             
            var callback = (LateBoundFieldSet)method.CreateDelegate(typeof(LateBoundFieldSet));
            return callback;
        }

        public static CachedFieldsInfo GetCachedFields(Type type)
        {
            CachedFieldsInfo cachedFieldsInfo;
            if (Instance._fieldDictionary.TryGetValue(type, out cachedFieldsInfo) == false)
            {
                var fields = type.GetFields();
                cachedFieldsInfo = new CachedFieldsInfo(fields.Length);
                for (int i = 0; i < fields.Length; i++)
                {
                    cachedFieldsInfo.CachedFields[i] = new CachedFieldInfo(
                        fields[i], CreateFieldSetter(fields[i]), CreateFieldGetter(fields[i]));
                }
                Instance._fieldDictionary.Add(type, cachedFieldsInfo);
            }
            return cachedFieldsInfo;
        }

        internal void Tick()
        {
            /*
                if (UnityEditor.EditorApplication.isCompiling && (UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused))
                {
                    Debug.Log("[mFramework] Compiled during play");
                    UnityEditor.EditorApplication.isPlaying = false;
                }
            */
            _repeatsActions.ForEach(ForEachRepeatAction);
            if (mUI.Instance != null) mUI.Instance.Tick();
        }

        private void ForEachRepeatAction(RepeatAction action)
        {
            action.Tick();
        }

        internal void FixedTick()
        {
            if (mUI.Instance != null) mUI.Instance.FixedTick();
        }

        internal void LateTick()
        {
            if (mUI.Instance != null) mUI.Instance.LateTick();
        }
    }
}
