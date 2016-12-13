using System;
using System.Collections.Generic;
using mUIApp.Animations;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    [Flags]
    public enum UIVector3Set
    {
        SET_X = 0x0,
        SET_Y = 0x1,
        SET_Z = 0x2,
    }

    public static class UIObjectHelper
    {
        public static T SetParent<T>(this T obj, UIObject parent) where T : UIObject
        {
            return obj.SetParent(parent.Transform);
        }

        public static T SetParent<T>(this T obj, Transform parent) where T : UIObject
        {
            obj.Transform.parent = parent;
            obj.Transform.localPosition = Vector3.zero;
            return obj;
        }

        public static Vector2 GetRelativePos<T>(this T obj, float xRelative, float yRelative) where T : UIGameObject
        {
            return  new Vector3(
                obj.ParentView.LeftAnchor + obj.ParentView.Width * xRelative,
                obj.ParentView.BottomAnchor + obj.ParentView.Height * yRelative,
                obj.Transform.position.z
            );
        }

        public static T RelativePos<T>(this T obj, float xRelative, float yRelative) where T : UIGameObject
        {
            obj.Transform.position = new Vector3(
                obj.ParentView.LeftAnchor + obj.ParentView.Width*xRelative,
                obj.ParentView.BottomAnchor + obj.ParentView.Height*yRelative,
                obj.Transform.position.z
            );
            obj.OnTranslate();
            return obj;
        }

        public static T RelativeTranslate<T>(this T obj, float xRelative, float yRelative = 0, Space space = Space.World) where T : UIGameObject
        {
            obj.Transform.Translate(
                obj.ParentView.Width*xRelative,
                obj.ParentView.Height*yRelative,
                0,
                space
            );
            obj.OnTranslate();
            return obj;
        }

        public static T Translate<T>(this T obj, float x, float y = 0, float z = 0, Space space = Space.World) where T : UIGameObject
        {
            obj.Transform.Translate(x, y, z, space);
            obj.OnTranslate();
            return obj;
        }

        public static Vector2 GetScale<T>(this T obj) where T : UIGameObject
        {
            return obj.Transform.localScale;
        }

        public static Vector2 GetPos<T>(this T obj) where T : UIGameObject
        {
            return obj.Transform.position;
        }

        public static T Position<T>(this T obj, float x, float y) where T : UIGameObject
        {
            obj.Transform.position = new Vector3(x, y, obj.Transform.position.z);
            obj.OnTranslate();
            return obj;
        }

        public static T Position<T>(this T obj, UIObject other, UIVector3Set set) where T : UIGameObject
        {
            Vector3 newPos = obj.Transform.position;
            if ((set & UIVector3Set.SET_X) == UIVector3Set.SET_X)
                newPos.x = other.Transform.position.x;
            if ((set & UIVector3Set.SET_Y) == UIVector3Set.SET_Y)
                newPos.y = other.Transform.position.y;
            if ((set & UIVector3Set.SET_Z) == UIVector3Set.SET_Z)
                newPos.z = other.Transform.position.z;
            obj.Transform.position = newPos;
            obj.OnTranslate();
            return obj;
        }

        public static T Position<T>(this T obj, UIObject other) where T : UIGameObject
        {
            obj.Transform.position = other.Transform.position;
            obj.OnTranslate();
            return obj;
        }

        public static T SortingOrder<T>(this T obj, int sortingOrder) where T : UIObject
        {
            obj.Renderer.sortingOrder = obj.ParentView.SortingOrder + sortingOrder;
            return obj;
        }

        public static T SetName<T>(this T obj, string name) where T : UIGameObject
        {
            obj.GameObject.name = name;
            return obj;
        }

        public static T Scale<T>(this T obj, float x, float y) where T : UIGameObject
        {
            Scale(obj.Transform, x, y);
            obj.OnScale();
            return obj;
        }

        public static T Rotate<T>(this T obj, float x, float y, float z) where T : UIGameObject
        {
            Rotate(obj.Transform, x, y, z);
            return obj;
        }

        public static T Rotate<T>(this T obj, float z) where T : UIGameObject
        {
            Rotate(obj.Transform, obj.Transform.eulerAngles.x, obj.Transform.eulerAngles.y, z);
            return obj;
        }

        public static T Rotate<T>(this T obj, Vector3 rotate) where T : UIGameObject
        {
            Rotate(obj.Transform, rotate.x, rotate.y, rotate.z);
            return obj;
        }

        public static T Scale<T>(this T obj, Vector3 scale) where T : UIGameObject
        {
            Scale(obj.Transform, scale.x, scale.y, scale.z);
            obj.OnScale();
            return obj;
        }

        public static T Active<T>(this T obj, bool active) where T : UIObject
        {
            obj.Active = active;
            return obj;
        }

        public static void Rotate(Transform transform, float x, float y, float z)
        {
            transform.eulerAngles = new Vector3(x, y, z);
        }

        public static void Scale(Transform transform, float x, float y)
        {
            transform.localScale = new Vector3(x, y, transform.localScale.z);
        }

        public static void Scale(Transform transform, float x, float y, float z)
        {
            transform.localScale = new Vector3(x, y, z);
        }
    }

    public abstract class UIObject : UIGameObject
    {
        public abstract float Width { get; }
        public abstract float Height { get; }

        public Renderer Renderer { get; protected set; }
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public BaseView ParentView { get; }
        public List<mUIAnimation> Animations { get; } = new List<mUIAnimation>();

        public void OnTranslate()
        {
            OnTranslateEvent?.Invoke(this);
        }

        public void Destroy()
        {
            ParentView.RemoveChildObject(this);
            UnityEngine.Object.Destroy(GameObject);
        }

        public void OnRotate()
        {
            OnRotateEvent?.Invoke(this);
        }

        public void OnScale()
        {
            OnScaleEvent?.Invoke(this);
        }

        public event Action<UIGameObject> OnTranslateEvent;
        public event Action<UIGameObject> OnScaleEvent;
        public event Action<UIGameObject> OnRotateEvent;

        public event Action<UIObject, bool, bool> OnChangeActiveState;
        public event Action<UIObject> OnTick;

        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                OnChangeActiveState?.Invoke(this, _active, value);
                _active = value;
            }
        }
        
        private bool _active;

        protected UIObject(BaseView view, bool createSpriteRenderer = true)
        {
            _active = true;

            ParentView = view;
            GameObject = new GameObject("UIObject");
            Transform = GameObject.transform;
            Transform.parent = view.Transform;
            Transform.localPosition = Vector3.zero;

            OnTick += (o) => { }; 

            if (createSpriteRenderer)
            {
                Renderer = GameObject.AddComponent<SpriteRenderer>();
                Renderer.sortingOrder = view.SortingOrder;
            }

            view.AddChildObject(this);
        }

        public void Tick()
        {
            for (int i = Animations.Count - 1; i >= 0; i--)
                Animations[i].Tick();
            OnTick?.Invoke(this);   
        }
    }
}