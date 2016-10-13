using mUIApp.Input;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp
{
    public sealed class mUI
    {
        public static bool Debug { get; set; }
        public static ISpriteRepository SpriteRepository { get; set; }
        public static IInputBase UIInput { get { return _engineInstance.UIInput; } }
        public static GameObject GameObject { get { return _engineGameObject; } }
        public static GameObject ViewsGameObject { get { return _uiViewsGameObject; } }
        public static mUICamera UICamera { get; }

        private static readonly mUIEngine _engineInstance;
        private static readonly GameObject _engineGameObject;
        private static readonly GameObject _uiViewsGameObject;
        
        static mUI()
        {
            if (_engineInstance == null)
            {
                _engineGameObject = new GameObject("mUI");
                _engineInstance = _engineGameObject.AddComponent<mUIEngine>();
                _uiViewsGameObject = new GameObject("Views");
                _uiViewsGameObject.transform.parent = _engineGameObject.transform;

                UICamera = new mUICamera();
                Init();
            }
        }

        private static void Init()
        {
            SpriteRepository = new mUIDefaultSpriteRepository();
        }

        public static T CreateView<T>(string viewName = "view") where T : View, new()
        {
            var view = new T();
            ViewHelper.InitViewCallback(view, _uiViewsGameObject.transform);
            view.GameObject.name = viewName;
            return view;    
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
    }
}
