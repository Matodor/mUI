using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Animations;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp
{
    public enum UIRendererType
    {
        SPRITE_RENDERER = 0,
        MESH_RENDERER = 1
    }

    public abstract class UIObject
    {
        public abstract float PureWidth { get; }
        public abstract float PureHeight { get; }

        public float Width { get { return PureWidth*Transform.lossyScale.x; } }
        public float Height { get { return PureHeight*Transform.lossyScale.y; } }

        public float Top { get { return Transform.position.y + Height/2; } }
        public float Bottom { get { return Transform.position.y - Height/2; } }
        public float Left { get { return Transform.position.x - Width/2; } }
        public float Right { get { return Transform.position.x + Width/2; } }

        public UIObject Parent { get; set; }
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Renderer Renderer { get; set; }
        public List<mUIAnimation> Animations { get; }
        public List<UIObject> Childs { get; }

        public bool Active
        {
            get
            {
                return _active && GameObject.activeInHierarchy;
            }
            set
            {
                OnChangeActiveState?.Invoke(this, _active, value);
                _active = value;
            }
        }

        /******************** Events ********************/
        public event Action<UIObject, Vector3> OnSetPositionEvent;
        public event Action<UIObject, Vector3> OnTranslateEvent;
        public event Action<UIObject, Vector3> OnScaleEvent;
        public event Action<UIObject, Vector3> OnRotateEvent;
        public event Action<UIObject, bool, bool> OnChangeActiveState;
        public event Action<UIObject> OnTick;
        public event Action<UIObject> OnFixedTick;
        public event Action<UIObject> OnDestroy;
        /************************************************/
        
        private bool _active; 
        
        protected UIObject(UIObject parent, UIRendererType type = UIRendererType.SPRITE_RENDERER)
        {
            _active = true;
            Animations = new List<mUIAnimation>();
            Childs = new List<UIObject>();

            GameObject = new GameObject("UIObject");
            Transform = GameObject.transform;

            if (type == UIRendererType.SPRITE_RENDERER)
                Renderer = GameObject.AddComponent<SpriteRenderer>();
            else
                Renderer = GameObject.AddComponent<MeshRenderer>();
             
            if (parent != null)
            {
                ChangeParent(parent);
            }
            else
            { 
                Parent = null;
                Transform.parent = mUI.ViewsGameObject.transform;
                Transform.localPosition = Vector3.zero;
            }
             
            mUI.OnTick += Tick;
            mUI.OnFixedTick += FixedTick;
        }

        ~UIObject()
        {
            mUI.Log("Destroy: {0}", ToString());
        }

        public void Show()
        {
            GameObject.SetActive(true);
            Active = true;
        }

        public void Hide()
        {
            GameObject.SetActive(false);
            Active = false;
        }

        public void ChangeParent(UIObject newParent)
        {
            Parent?.Childs.Remove(this);
            Parent = newParent;
            Parent.Childs.Add(this);
            Transform.parent = Parent.Transform;
            Transform.localPosition = Vector3.zero;
            this.SortingOrder(1);
        }

        public void Position(float x, float y, float z)
        {
            var pos = new Vector3(x, y, z);
            Transform.position = pos;
            OnSetPositionEvent?.Invoke(this, pos);
        }

        public void Translate(float x, float y, float z, Space space)
        {
            Transform.Translate(x, y, z, space);
            OnTranslateEvent?.Invoke(this, new Vector3(x, y, z));
        }

        public void Scale(float x, float y, float z)
        {
            Transform.localScale = new Vector3(x, y, z);
            OnScaleEvent?.Invoke(this, Transform.localScale);
        }

        public void Rotate(float x, float y, float z)
        {
            Transform.eulerAngles = new Vector3(x, y, z);
            OnRotateEvent?.Invoke(this, Transform.eulerAngles);
        }

        public void Destroy()
        {
            mUI.OnTick -= Tick;
            mUI.OnFixedTick -= FixedTick;

            Parent?.Childs.Remove(this);
            Parent = null;

            for (int i = Animations.Count - 1; i >= 0; i--)
                Animations[i].Remove();

            for (int i = Childs.Count - 1; i >= 0; i--)
                Childs[i].Destroy();

            OnDestroy?.Invoke(this);
            UnityEngine.Object.Destroy(GameObject);
        }

        private void FixedTick()
        {
            OnFixedTick?.Invoke(this);
        }

        private void Tick()
        {
            OnTick?.Invoke(this);
        }
    }
}
