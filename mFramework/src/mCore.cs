using System;
using System.Collections.Generic;
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

        public static bool IsEditor { get; private set; }
        public static bool IsDebug { get; set; }
        public static event Action OnApplicationQuitEvent;

        private static mCore _instance;
        private readonly Dictionary<Type, CachedFieldsInfo> _fieldDictionary;
        private readonly UnidirectionalList<RepeatAction> _repeatsActions;
        private readonly EditorExtension _editorExtension;

        private mCore()
        {
            _repeatsActions = UnidirectionalList<RepeatAction>.Create();
            _fieldDictionary = new Dictionary<Type, CachedFieldsInfo>();
            
            var engine = new GameObject("mFramework").AddComponent<mEngine>();
            engine.transform.position = new Vector3(0, 0, 9999);

            if (Application.isEditor)
            {
                _editorExtension = new EditorExtension();
                IsDebug = true;
                IsEditor = true;
            }

            Log("[mFramework] init");
        }
         
        ~mCore()
        {
            Log("~mCore");
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
            Instance._editorExtension?.Detach();
            Log("[mCore] OnApplicationQuit");
        }

        public static void Log(string format, params object[] @params)
        {
            if (IsEditor && IsDebug)
                UnityEngine.Debug.Log(string.Format(format, @params));
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
