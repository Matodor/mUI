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

        public UIClickableObj[] GetAllAsArray()
        {
            return _list.ToArray();
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
        public event Action<mUIMouseEvent>      OnMouseDownEvent = i => { };
        public event Action<mUIMouseEvent>      OnMouseUpEvent   = i => { };
        public event Action<mUIMouseEvent>      OnMouseDragEvent = i => { };
        public event Action<mUIKeyboardEvent>   OnKeyUpEvent     = i => { };
        public event Action<mUIKeyboardEvent>   OnKeyDownEvent   = i => { };

        public UIClickableObjList UIClickableObjList { get; }

        private IInputGetEvents _getEventsController;
        private Action<IInputBase> _onGUI, _onUpdate;

        public mUIInputDefault()
        {
            UIClickableObjList = new UIClickableObjList();

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

        private void CheckClickable(ref UIClickableObj current, UIClickableObj next)
        {
            if (current == null)
                current = next;
            else
            {
                if (current.Renderer.sortingOrder < next.Renderer.sortingOrder)
                    current = next;
            }
        }

        private IEnumerable<UIClickableObj> ClickableObject(mUIMouseEvent mouseEvent)
        {
            return UIClickableObjList.Get(GetClickableObj(mouseEvent));
        } 

        private Func<UIClickableObj, bool> GetClickableObj(mUIMouseEvent mouseEvent)
        {
            return obj => obj.Active && obj.CanClick(mouseEvent.MouseScreenPos);
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
                        Action<mUIMouseEvent> events = e => { };
                              
                        foreach (var clickable in ClickableObject(mouseEvent))
                        {
                            if (clickable.IgnoreSortingOrder)
                                events += clickable.OnUIMouseDown;
                            else
                                CheckClickable(ref current, clickable);
                        }

                        if (current != null)
                            events += current.OnUIMouseDown;
                        events += OnMouseDownEvent;
                        events(mouseEvent);
                    } break;
                    case mUIMouseEventType.MouseUp:
                        foreach (var clickable in UIClickableObjList.GetAllAsArray())
                            clickable.OnUIMouseUp(mouseEvent);
                        OnMouseUpEvent(mouseEvent);
                        break;
                    case mUIMouseEventType.MouseDrag:
                    {
                        UIClickableObj current = null;
                        Action<mUIMouseEvent> events = e => { };

                        foreach (var clickable in ClickableObject(mouseEvent))
                        {
                            if (clickable.IgnoreSortingOrder)
                                events += clickable.OnUIMouseDrag;
                            else
                                CheckClickable(ref current, clickable);
                        }

                        if (current != null)
                            events += current.OnUIMouseDrag;
                        events += OnMouseDragEvent;
                        events(mouseEvent);
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
