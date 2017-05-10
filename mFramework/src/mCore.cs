using System;
using System.Linq;
using UnityEngine;

namespace mFramework
{
    public sealed class mCore : ITicking
    {
        public static mCore Instance { get; private set; }
        public static bool IsDebug { get; set; }

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

        private void InejctEditor()
        {
            var unityEditorAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name == "UnityEditor");
            if (unityEditorAssembly == null)
            {
                return;
            }

            IsDebug = true;
            Log("[mFramework] InejctEditor");
        }

        public static void Log(string format, params object[] obj)
        {
            if (IsDebug)
                UnityEngine.Debug.Log(string.Format(format, obj));
        }

        public void Tick()
        {
            UI.Instance?.Tick();
        }

        public void FixedTick()
        {
            UI.Instance?.FixedTick();
        }

        public void LateTick()
        {
            UI.Instance?.LateTick();
        }
    }
}
