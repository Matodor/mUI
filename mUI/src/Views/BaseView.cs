using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.Views
{
    public abstract class BaseView
    {
        public abstract void Create();

        public BaseView ParentView { get; set; }
        public GameObject GameObject { get { return _viewObject; } }
        public Transform Transform { get { return _viewTransform; } }

        private List<PartialView> _childViews;
        private List<UIObject> _childObjects;
        private GameObject _viewObject;
        private Transform _viewTransform;

        private float _viewWidth;
        private float _viewHeight;

        public void AddChildObject(UIObject obj)
        {
            _childObjects.Add(obj);    
        }

        public void AddChildView(PartialView view)
        {
            _childViews.Add(view);    
        }

        private void InitBase(Transform parent)
        {
            _viewObject = new GameObject("view");
            _viewObject.transform.parent = parent;
            _viewTransform = _viewObject.transform;
            _childViews = new List<PartialView>();
            _childObjects = new List<UIObject>();

            Debug.Log("Create view");

            Create();
        }

        public void SetWidth(float width)
        {
            _viewWidth = width;
        }

        public void SetHeight(float height)
        {
            _viewHeight = height;
        }
    }
}
