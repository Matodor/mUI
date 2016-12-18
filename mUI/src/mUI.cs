using System;
using System.Collections.Generic;
using mUIApp.Input;
using mUIApp.Other;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp
{
    public static class mUI
    {
        public static bool Debug { get; set; }
        public static ISpriteRepository Sprites { get; set; }
        public static IInputBase UIInput { get { return _engineInstance.UIInput; } }
        public static IKeyStorage KeyStorage { get; set; }
        public static GameObject GameObject { get { return _engineGameObject; } }
        public static GameObject ViewsGameObject { get { return _uiViewsGameObject; } }
        public static mUICamera UICamera { get; }
        public static string DefaultFont { get; set; }

        public const string DefaultFontName = "mUIDefault";

        public static event Action OnTick;
        public static event Action OnFixedTick;

        private static readonly mUIEngine _engineInstance;
        private static readonly GameObject _engineGameObject;
        private static readonly GameObject _uiViewsGameObject;
        //private static readonly List<View> _uiViews;
        private static readonly Dictionary<string, mUIFont> _uiFonts; 

        static mUI()
        {
            if (_engineInstance == null)
            {
                _engineGameObject = new GameObject("mUI");
                _uiViewsGameObject = new GameObject("Views");

                _engineInstance = _engineGameObject.AddComponent<mUIEngine>();
                _uiViewsGameObject.transform.parent = _engineGameObject.transform;
                _uiFonts = new Dictionary<string, mUIFont>();

                UICamera = new mUICamera(ViewsGameObject);
                Init();
            } 
        }

        public static ActionRepeat ActionRepeat(float repeatTime, Action<object> action, object data = null)
        {
            return new ActionRepeat(repeatTime, action, data);
        }

        public static ActionTimer ActionTimer(float time, Action<object> action, object data = null)
        {
            return new ActionTimer(time, action, data);
        }

        public static void Tick()
        {
            OnTick();
        }

        public static mUIFont GetFont(string name)
        {
            if (_uiFonts.ContainsKey(name))
                return _uiFonts[name];
            return null;
        }

        public static bool LoadFont(string name)
        {
            var font = new mUIFont(name);
            if (font.Loaded)
            {
                _uiFonts.Add(name, font);
                return true;
            }
            return false;
        }

        public static void LateTick()
        {
        }

        public static void FixedTick()
        {
            OnFixedTick();
        }
        
        private static void Init()
        {
            OnTick += () => { };
            OnFixedTick += () => { };
            DefaultFont = DefaultFontName;
            KeyStorage = new mUIKeyStorage();
            Sprites = new mUIDefaultSpriteRepository();
        }

        public static void Log(string format, params object[] obj)
        {
            UnityEngine.Debug.Log(string.Format(format, obj));
        }

        public static void Log(object obj)
        {
            UnityEngine.Debug.Log(obj);
        }

        public static Vector2 GetSpriteOffset(Bounds bounds)
        {
            var pivot = GetSpritePivot(bounds);
            return new Vector2(
                (-pivot.x + 0.5f) * bounds.size.x,
                (-pivot.y + 0.5f) * bounds.size.y
            );
        }

        public static Vector2 GetSpritePivot(Bounds bounds)
        {
            return new Vector2(
                -bounds.center.x / bounds.extents.x / 2 + 0.5f, 
                -bounds.center.y / bounds.extents.y / 2 + 0.5f
            );
        }

        public static bool TriangleContainsPoint(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            if ((P.x - A.x) * (A.y - B.y) - (P.y - A.y) * (A.x - B.x) >= 0)
                if ((P.x - B.x) * (B.y - C.y) - (P.y - B.y) * (B.x - C.x) >= 0)
                    if ((P.x - C.x) * (C.y - A.y) - (P.y - C.y) * (C.x - A.x) >= 0)
                        return true;
            return false;
        }

        public static int Сlamp(int val, int min, int max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }

        public static double Сlamp(double val, double min, double max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }

        public static float Сlamp(float val, float min, float max)
        {
            if (val < min)
                return min;
            if (val > max)
                return max;
            return val;
        }
    }
}
