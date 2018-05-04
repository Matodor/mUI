using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public delegate object LateBoundFieldGet(object target);
    public delegate void LateBoundFieldSet(object target, object value);

    public sealed class mCore
    {
        internal static mBehaviour Behaviour { get; private set; }

        public static bool IsEditor { get; private set; }
        public static bool IsDebug { get; set; }
        public static event Action ApplicationQuitEvent = delegate {};
        public static event Action<bool> ApplicationPaused = delegate {};

        private static Dictionary<Type, CachedFieldsInfo> _fieldDictionary;
        private static UnidirectionalList<RepeatAction> _repeatsActions;
        private static UnidirectionalList<TimerAction> _timerActions;
        private static mCore _instance;

        static mCore()
        {
            if (_instance == null)
                _instance = new mCore();
        }

        ~mCore()
        {
            Log("~mCore");
        }

        private mCore()
        {
            var newCultureDefinition = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            newCultureDefinition.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = newCultureDefinition;

            Behaviour = new GameObject("mFramework").AddComponent<mBehaviour>();
            Behaviour.transform.position = new Vector3(0, 0, 0);

            _repeatsActions = UnidirectionalList<RepeatAction>.Create();
            _timerActions = UnidirectionalList<TimerAction>.Create();
            _fieldDictionary = new Dictionary<Type, CachedFieldsInfo>();
            
            if (Application.isEditor)
            {
                IsDebug = true;
                IsEditor = true;
            }

            Log("[mFramework] init");
        }
         
        internal static bool RemoveTimerAction(TimerAction timerAction)
        {
            return _timerActions.Remove(timerAction);
        }

        internal static void AddTimerAction(TimerAction timerAction)
        {
            _timerActions.Add(timerAction);
        }

        internal static bool RemoveRepeatAction(RepeatAction repeatAction)
        {
            return _repeatsActions.Remove(repeatAction);
        }

        internal static void AddRepeatAction(RepeatAction repeatAction)
        {
            _repeatsActions.Add(repeatAction);
        }

        internal static void OnApplicationQuit()
        {
            ApplicationQuitEvent.Invoke();
            Log("[mCore] OnApplicationQuit");

            _repeatsActions.Clear();
            _timerActions.Clear();
            _instance = null;
        }

        public static void Log(string format, params object[] @params)
        {
            if (IsDebug)
            {
                UnityEngine.Debug.Log(@params.Length > 0 ? string.Format(format, @params) : format);
            }
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

            var callback = (LateBoundFieldGet) method.CreateDelegate(typeof(LateBoundFieldGet));
            return callback;
        }

        public static LateBoundFieldSet CreateFieldSetter(FieldInfo field)
        {
            var method = new DynamicMethod("Set" + field.Name, null, new[] { typeof(object), typeof(object) }, 
                field.DeclaringType, true);
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
            if (_fieldDictionary.TryGetValue(type, out var cachedFieldsInfo) == false)
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                cachedFieldsInfo = new CachedFieldsInfo(fields.Length);
                for (int i = 0; i < fields.Length; i++)
                {
                    cachedFieldsInfo.CachedFields[i] = new CachedFieldInfo(
                        fields[i], CreateFieldSetter(fields[i]), CreateFieldGetter(fields[i]));
                }
                _fieldDictionary.Add(type, cachedFieldsInfo);
            }
            return cachedFieldsInfo;
        }

        private static void ForEachTimerAction(TimerAction timerAction)
        {
            timerAction.Tick();
        }

        private static void ForEachRepeatAction(RepeatAction action)
        {
            action.Tick();
        }

        internal static void Tick()
        {
            _repeatsActions.ForEach(ForEachRepeatAction);
            _timerActions.ForEach(ForEachTimerAction);

            mUI.Tick();
        }

        internal static void FixedTick()
        {
            mUI.FixedTick();
        }

        internal static void LateTick()
        {
            mUI.LateTick();
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
            Debug.DrawLine(boxPos + new Vector2(-boxSize.x / 2, boxSize.y / 2), boxPos + new Vector2(boxSize.x / 2, boxSize.y / 2));
            Debug.DrawLine(boxPos + new Vector2(boxSize.x / 2, boxSize.y / 2), boxPos + new Vector2(boxSize.x / 2, -boxSize.y / 2));
            Debug.DrawLine(boxPos + new Vector2(boxSize.x / 2, -boxSize.y / 2), boxPos + new Vector2(-boxSize.x / 2, -boxSize.y / 2));
            Debug.DrawLine(boxPos + new Vector2(-boxSize.x / 2, -boxSize.y / 2), boxPos + new Vector2(-boxSize.x / 2, boxSize.y / 2));
        }

        internal static void OnApplicationPause(bool pauseState)
        {
            ApplicationPaused(pauseState);
        }
    }
}
