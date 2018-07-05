using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using UnityEngine;

namespace mFramework
{
    public delegate object LateBoundFieldGet(object target);

    public delegate void LateBoundFieldSet(object target, object value);

    public sealed class mCore : MonoBehaviour
    {
        internal static mCore Instance { get; private set; }

        public static event Action ApplicationQuit;
        public static event Action<bool> ApplicationPaused;

        public static bool IsEditor { get; private set; }
        public static bool IsDebug { get; set; }
        
        private Dictionary<Type, CachedFieldsInfo> _fieldDictionary;
        private UnidirectionalList<RepeatAction> _repeatsActions;
        private UnidirectionalList<TimerAction> _timerActions;
        private bool _destroyed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (Instance != null)
                return;
            
            var instance = new GameObject("mCore").AddComponent<mCore>();
            instance.gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInHierarchy;
            DontDestroyOnLoad(instance);
        }

        private void Awake()
        {
            if (Instance != null)
                throw new Exception("mCore instance already created");

            Instance = this;
            _repeatsActions = UnidirectionalList<RepeatAction>.Create();
            _timerActions = UnidirectionalList<TimerAction>.Create();
            _fieldDictionary = new Dictionary<Type, CachedFieldsInfo>();

            if (Application.isEditor)
            {
                IsDebug = true;
                IsEditor = true;

                var cultureDefinition = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
                cultureDefinition.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = cultureDefinition;
            }

            Debug.Log("[mCore] init");
        }

        private void OnDestroy()
        {
            if (_destroyed)
                return;

            Debug.Log("[mCore] OnDestroy");
            _destroyed = true;
            _fieldDictionary.Clear();
            _repeatsActions.Clear();
            _timerActions.Clear();
            Instance = null;
        }

        private void OnApplicationPause(bool paused)
        {
            Debug.Log($"[mCore] OnApplicationPause ({paused})");
            ApplicationPaused?.Invoke(paused);
        }

        private void OnApplicationQuit()
        {
            Debug.Log("[mCore] OnApplicationQuit");
            ApplicationQuit?.Invoke();

            OnDestroy();
            Destroy(gameObject);
            ApplicationQuit = null;
            ApplicationPaused = null;
        }

        private void Update()
        {
            EventsController.Update();

            _repeatsActions.ForEach(ForEachRepeatAction);
            _timerActions.ForEach(ForEachTimerAction);
        }
        
        internal static bool RemoveTimerAction(TimerAction timerAction)
        {
            return Instance != null && Instance._timerActions.Remove(timerAction);
        }

        internal static void AddTimerAction(TimerAction timerAction)
        {
            Instance?._timerActions.Add(timerAction);
        }

        internal static bool RemoveRepeatAction(RepeatAction repeatAction)
        {
            return Instance != null && Instance._repeatsActions.Remove(repeatAction);
        }

        internal static void AddRepeatAction(RepeatAction repeatAction)
        {
            Instance?._repeatsActions.Add(repeatAction);
        }

        private static void ForEachTimerAction(TimerAction timerAction)
        {
            timerAction.Tick();
        }

        private static void ForEachRepeatAction(RepeatAction action)
        {
            action.Tick();
        }

        public static LateBoundFieldGet CreateFieldGetter(FieldInfo field)
        {
            var method = new DynamicMethod("Get" + field.Name, typeof(object), new[] {typeof(object)},
                field.DeclaringType, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, field.DeclaringType); // Cast to source type
            gen.Emit(OpCodes.Ldfld, field);

            if (field.FieldType.IsValueType)
                gen.Emit(OpCodes.Box, field.FieldType);

            gen.Emit(OpCodes.Ret);

            var callback = (LateBoundFieldGet) method.CreateDelegate(typeof(LateBoundFieldGet));
            return callback;
        }

        public static LateBoundFieldSet CreateFieldSetter(FieldInfo field)
        {
            var method = new DynamicMethod("Set" + field.Name, null, new[] {typeof(object), typeof(object)},
                field.DeclaringType, true);
            var gen = method.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0); // Load target to stack
            gen.Emit(OpCodes.Castclass, field.DeclaringType); // Cast target to source type
            gen.Emit(OpCodes.Ldarg_1); // Load value to stack
            gen.Emit(OpCodes.Unbox_Any, field.FieldType); // Unbox the value to its proper value type
            gen.Emit(OpCodes.Stfld, field); // Set the value to the input field
            gen.Emit(OpCodes.Ret);

            var callback = (LateBoundFieldSet) method.CreateDelegate(typeof(LateBoundFieldSet));
            return callback;
        }

        public static CachedFieldsInfo GetCachedFields(Type type)
        {
            if (Instance._fieldDictionary.TryGetValue(type, out var cachedFieldsInfo) == false)
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
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
        
        public static void DrawDebugCircle(Vector2 pos, float radius)
        {
            for (int i = 0; i < 360; i++)
            {
                var p1 = mMath.GetRotatedPoint(pos, new Vector2(radius, 0), i);
                var p2 = mMath.GetRotatedPoint(pos, new Vector2(radius, 0), i + 1);
                Debug.DrawLine(p1, p2);
            }
        }

        public static void DrawDebugBox(Vector2 boxPos, Vector2 boxSize)
        {
            Debug.DrawLine(boxPos + new Vector2(-boxSize.x / 2, boxSize.y / 2),
                boxPos + new Vector2(boxSize.x / 2, boxSize.y / 2));
            Debug.DrawLine(boxPos + new Vector2(boxSize.x / 2, boxSize.y / 2),
                boxPos + new Vector2(boxSize.x / 2, -boxSize.y / 2));
            Debug.DrawLine(boxPos + new Vector2(boxSize.x / 2, -boxSize.y / 2),
                boxPos + new Vector2(-boxSize.x / 2, -boxSize.y / 2));
            Debug.DrawLine(boxPos + new Vector2(-boxSize.x / 2, -boxSize.y / 2),
                boxPos + new Vector2(-boxSize.x / 2, boxSize.y / 2));
        }
    }
}
