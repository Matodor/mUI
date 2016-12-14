using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Animations;
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
        public float Bottom { get { return Transform.position.y + Height/2; } }
        public float Left { get { return Transform.position.x - Width/2; } }
        public float Right { get { return Transform.position.x + Width/2; } }

        public UIObject Parent { get; set; }
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public Renderer Renderer { get; set; }

        public int SortingOrder
        {
            get
            {
                return _sortingOrder;
            }
            set
            {
                _sortingOrder = value;
            }
        }

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
        private int _sortingOrder;
        private readonly List<UIObject> _childsObjects;
        private readonly List<mUIAnimation> _animations; 

        protected UIObject(UIObject parent, UIRendererType type = UIRendererType.SPRITE_RENDERER)
        {
            _animations = new List<mUIAnimation>();
            _childsObjects = new List<UIObject>();

            GameObject = new GameObject("UIObject");
            Transform = GameObject.transform;

            if (type == UIRendererType.SPRITE_RENDERER)
                Renderer = GameObject.AddComponent<SpriteRenderer>();
            else
                Renderer = GameObject.AddComponent<MeshRenderer>();

            if (parent != null)
            {
                Parent = parent;
                Transform.parent = parent.Transform;
                Transform.localPosition = Vector3.zero;
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

        public void Position(float x, float y, float z)
        {
            var pos = new Vector3(x, y, z);
            Transform.position = pos;
            OnSetPositionEvent?.Invoke(this, pos);
        }

        public void Translate(float x, float y, float z, Space space = Space.World)
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

        public void RemoveAnimation(mUIAnimation anim)
        {
            _animations.Remove(anim);
        }

        public void AddAnimation(mUIAnimation anim)
        {
            _animations.Add(anim);
        }

        public void AddChild(UIObject child)
        {
            _childsObjects.Add(child);            
        }

        public void Destroy()
        {
            // destroy animations
        }

        private void FixedTick()
        {

        }

        private void Tick()
        {
            
        }
    }
}
