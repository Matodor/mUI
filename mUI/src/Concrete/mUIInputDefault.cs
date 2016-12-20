using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.Input
{
    public class UIClickableObjList
    {
        private readonly List<UIClickableObj> _list;

        public UIClickableObjList()
        {
            _list = new List<UIClickableObj>();
        }

        public void Add(UIClickableObj obj)
        {
            _list.Add(obj);
        }

        public bool Remove(UIClickableObj obj)
        {
            return _list.Remove(obj);
        }

        public IEnumerable<UIClickableObj> GetAll()
        {
            return _list.AsEnumerable();
        }

        public IEnumerable<UIClickableObj> Get(Func<UIClickableObj, bool> predicat)
        {
            return _list.Where(predicat);
        } 
    }

    public class mUIInputDefault : IInputBase
    {
        public event Action<mUIMouseEvent> OnMouseDownEvent;
        public event Action<mUIMouseEvent> OnMouseUpEvent;
        public event Action<mUIMouseEvent> OnMouseDragEvent;
        public event Action<mUIKeyboardEvent> OnKeyUpEvent;
        public event Action<mUIKeyboardEvent> OnKeyDownEvent;

        public UIClickableObjList UIClickableObjList { get; }

        private IInputGetEvents _getEventsController;
        private Action<IInputBase> _onGUI, _onUpdate;

        public mUIInputDefault()
        {
            UIClickableObjList = new UIClickableObjList();

            OnMouseDownEvent = i => { };
            OnMouseUpEvent = i => { };
            OnMouseDragEvent = i => { };
            OnKeyUpEvent = i => { };
            OnKeyDownEvent = i => { };

            _onGUI = i => { };
            _onUpdate = i => { };

            switch (Application.platform)
            {
                case RuntimePlatform.Android: SetupMobile(); break;
                case RuntimePlatform.IPhonePlayer: SetupMobile(); break;
                case RuntimePlatform.WindowsEditor: SetupDesktop(); break;
                case RuntimePlatform.WindowsPlayer: SetupDesktop(); break;
            }
        }

        private void SetupMobile()
        {
            _getEventsController = new mUIMobileGetEvents();
            _onUpdate = _getEventsController.GetEvents;
        }

        private void SetupDesktop()
        {
            _getEventsController = new mUIDesktopGetEvents();
            _onGUI = _getEventsController.GetEvents;
        }

        public void ParseEvent(mUIEvent @event)
        {
            if (@event.Type == mUIEventType.MOUSE_EVENT)
            {
                mUIMouseEvent mouseEvent = (mUIMouseEvent) @event;
                switch (mouseEvent.MouseEventType)
                {
                    case mUIMouseEventType.NONE:
                        break;
                    case mUIMouseEventType.MouseDown:
                    {
                        UIClickableObj current = null;
                        foreach (var clickable in UIClickableObjList.Get(obj => obj.CanClick(mouseEvent.MouseScreenPos)))
                        {
                            if (clickable.IgnoreSortingOrder)
                                clickable.OnUIMouseDown(mouseEvent);
                            else
                            {
                                if (current == null)
                                    current = clickable;
                                else
                                {
                                    if (!clickable.IgnoreSortingOrder && current.Renderer.sortingOrder < clickable.Renderer.sortingOrder)
                                        current = clickable;
                                } 
                            }
                        }
                        current?.OnUIMouseDown(mouseEvent);
                        OnMouseDownEvent(mouseEvent);
                    } break;
                    case mUIMouseEventType.MouseUp:
                        foreach (var clickable in UIClickableObjList.GetAll())
                            clickable.OnUIMouseUp(mouseEvent);
                        OnMouseUpEvent(mouseEvent);
                        break;
                    case mUIMouseEventType.MouseDrag:
                    {
                        UIClickableObj current = null;
                        foreach (var clickable in UIClickableObjList.Get(obj => obj.CanClick(mouseEvent.MouseScreenPos)))
                        {
                            if (clickable.IgnoreSortingOrder)
                                clickable.OnUIMouseDrag(mouseEvent);
                            else
                            {
                                if (current == null)
                                    current = clickable;
                                else
                                {
                                    if (!clickable.IgnoreSortingOrder && current.Renderer.sortingOrder < clickable.Renderer.sortingOrder)
                                        current = clickable;
                                }
                            }
                        }
                        current?.OnUIMouseDrag(mouseEvent);
                        OnMouseDragEvent(mouseEvent);
                    } break;
                    case mUIMouseEventType.ScrollWheel:
                        break;
                }
            }
            else if (@event.Type == mUIEventType.KEYBOARD_EVENT)
            {
                mUIKeyboardEvent keyboardEvent = (mUIKeyboardEvent) @event;
                switch (keyboardEvent.KeyboardEventType)
                {
                    case mUIKeyboardEventType.KeyUp:
                        OnKeyUpEvent(keyboardEvent);
                        break;
                    case mUIKeyboardEventType.KeyDown:
                        OnKeyDownEvent(keyboardEvent);
                        break;
                }
            }
        }

        public void OnGUI()
        {
            _onGUI(this);
        }

        public void Update()
        {
            _onUpdate(this);
        }
    }
}
