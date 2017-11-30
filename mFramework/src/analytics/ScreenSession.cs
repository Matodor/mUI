using System;
using System.Linq;
using mFramework.UI;
using SimpleJSON;

namespace mFramework.Analytics
{
    internal class ScreenSession
    {
        public readonly DateTime AttachTime;
        public static Type[] ViewsTypes;

        private readonly JSONObject _buttonClicks;
        private bool _lastVisible;
        private DateTime _lastHideAt;
        private TimeSpan _totalHide;
        private int _hiddenTimes;

        public ScreenSession(UIObject view)
        {
            AttachTime = DateTime.Now;
            if (!(view is IView))
                return;

            mCore.Log($"Attach ScreenSession: {view.GetType().Name}");

            _lastVisible = view.IsShowing;
            _totalHide = TimeSpan.Zero;
            _buttonClicks = new JSONObject();

            if (!_lastVisible)
            {
                _lastHideAt = DateTime.Now;
                _hiddenTimes = 1;
            }

            view.BeforeDestroy += ViewOnBeforeDestroy;
            view.ChildObjectAdded += ObjOnChildObjectAdded;
            view.VisibleChanged += ViewOnVisibleChanged;
            view.Childs.ForEach(RecursivelySetup);
        }

        private void ViewOnVisibleChanged(UIObject sender)
        {
            if (!_lastVisible && sender.IsShowing)
            {
                _totalHide += DateTime.Now - _lastHideAt;
            }
            else if (_lastVisible && !sender.IsShowing)
            {
                _hiddenTimes++;
                _lastHideAt = DateTime.Now;
            }

            _lastVisible = sender.IsShowing;
        }

        ~ScreenSession()
        {
            mCore.Log("~ScreenSession");
        }

        private void ViewOnBeforeDestroy(UIObject sender)
        {
            sender.BeforeDestroy -= ViewOnBeforeDestroy;
            sender.ChildObjectAdded -= ObjOnChildObjectAdded;
            sender.VisibleChanged -= ViewOnVisibleChanged;

            if (!_lastVisible && !sender.IsShowing)
            {
                _totalHide += DateTime.Now - _lastHideAt;
            }

            var stats = new JSONObject
            {
                ["screen_name"] = sender.GetType().Name,
                ["hidden_times"] = _hiddenTimes,
                ["hidden_time"] = (int)_totalHide.TotalSeconds,
                ["screen_lifetime"] = (int)(DateTime.Now - AttachTime).TotalSeconds,
                ["clicks"] = _buttonClicks,
            };
             
            mCore.Log($"{sender.GetType().Name} stats: {stats.ToString()}");
        }

        private void ObjOnChildObjectAdded(UIObject sender, AddedСhildObjectEventArgs e)
        {
            RecursivelySetup(e.AddedObject);
        }

        private void RecursivelySetup(UIObject obj)
        {
            if (ViewsTypes != null &&
                ViewsTypes.Contains(obj.GetType()))
            {
                var screenSession = new ScreenSession(obj);
            }
            else
            {
                if (obj is IUIClickable clickable)
                {
                    clickable.UIClickable.CanMouseDown += UiClickableOnCanMouseDown;
                }

                obj.ChildObjectAdded += ObjOnChildObjectAdded;
                obj.Childs.ForEach(RecursivelySetup);
            }
        }

        private bool UiClickableOnCanMouseDown(IUIClickable uiClickable, MouseEvent e)
        {
            var obj = uiClickable as UIObject;

            if (obj == null ||
                !obj.IsActive ||
                !uiClickable.UIClickable.Area2D.InArea(mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos)))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(obj.tag))
            {
                if (_buttonClicks[obj.tag] != null)
                {
                    _buttonClicks[obj.tag]["clicks"] = _buttonClicks[obj.tag]["clicks"].AsInt + 1;
                }
                else
                {
                    _buttonClicks[obj.tag] = new JSONObject()
                    {
                        ["clicks"] = 0
                    };
                }
            }

            return true;
        }
    }
}