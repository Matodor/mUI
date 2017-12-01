using System;
using System.Collections.Generic;
using System.Linq;
using mFramework.UI;
using SimpleJSON;

namespace mFramework.Analytics
{
    internal class ScreenSession
    {
        public static Type[] ViewsTypes;
        public static readonly Dictionary<ulong, ScreenSession> ScreenSessions;

        public readonly JSONObject Session;
        public readonly DateTime AttachTime;
        public readonly ScreenSession Parent;
        public readonly IView AttachedView;

        private readonly Dictionary<ulong, ScreenSession> _childs;
        private bool _lastVisible;
        private DateTime _lastHideAt;
        private TimeSpan _totalHide;

        static ScreenSession()
        {
            ScreenSessions = new Dictionary<ulong, ScreenSession>();
        }

        public ScreenSession(IView view, ScreenSession parent)
        {
            Parent = parent;
            AttachTime = DateTime.Now;
            ScreenSessions.Add(view.GUID, this);

            mCore.Log($"Attach ScreenSession: {view.GetType().Name}");

            _childs = new Dictionary<ulong, ScreenSession>();
            AttachedView = view;
            _lastVisible = view.IsShowing;
            _totalHide = TimeSpan.Zero;

            Session = new JSONObject
            {
                ["view_name"] = view.GetType().Name,
                ["hidden_times"] = 0,
                ["hidden_time"] = 0,
                ["screen_lifetime"] = 0,
                ["events"] = new JSONArray(),
                ["childs"] = new JSONArray()
            };

            if (!_lastVisible)
            {
                _lastHideAt = DateTime.Now;
                Session["hidden_times"] = 1;
            }

            parent?.Session["childs"].AsArray.Add(Session);

            view.BeforeDestroy += ViewOnBeforeDestroy;
            view.ChildObjectAdded += ObjOnChildObjectAdded;
            view.VisibleChanged += ViewOnVisibleChanged;
            view.Childs.ForEach(RecursivelySetup);
        }

        public void Event(string key, JSONNode node)
        {
            Session["events"].AsArray.Add(new JSONObject
            {
                ["key"] = key,
                ["payload"] = node
            });
        }

        public void Event(string key, string payload)
        {
            Session["events"].AsArray.Add(new JSONObject
            {
                ["key"] = key,
                ["payload"] = payload
            });
        }

        private void RemoveChild(ulong key)
        {
            _childs.Remove(key);
        }

        private void ViewOnVisibleChanged(IUIObject sender)
        {
            if (!_lastVisible && sender.IsShowing)
            {
                _totalHide += DateTime.Now - _lastHideAt;
            }
            else if (_lastVisible && !sender.IsShowing)
            {
                Session["hidden_times"]++;
                _lastHideAt = DateTime.Now;
            }

            _lastVisible = sender.IsShowing;
        }

        ~ScreenSession()
        {
            mCore.Log("~ScreenSession");
        }

        public IEnumerable<ScreenSession> DeepChild()
        {
            foreach (var a in _childs.Values)
            {
                foreach (var b in a.DeepChild())
                    yield return b;
                yield return a;
            }
            yield return this;
        }

        public void Update()
        {
            if (!_lastVisible && !AttachedView.IsShowing)
            {
                _totalHide += DateTime.Now - _lastHideAt;
            }

            Session["screen_lifetime"] = (int)(DateTime.Now - AttachTime).TotalSeconds;
            Session["hidden_time"] = (int)_totalHide.TotalSeconds;
        }

        private void ViewOnBeforeDestroy(IUIObject sender)
        {
            Parent?.RemoveChild(AttachedView.GUID);

            sender.BeforeDestroy -= ViewOnBeforeDestroy;
            sender.ChildObjectAdded -= ObjOnChildObjectAdded;
            sender.VisibleChanged -= ViewOnVisibleChanged;

            Update();
        }

        private void ObjOnChildObjectAdded(IUIObject sender, IUIObject addedObj)
        {
            RecursivelySetup(addedObj);
        }

        private void RecursivelySetup(IUIObject obj)
        {
            if (obj is IView view && 
                ViewsTypes != null &&
                ViewsTypes.Contains(obj.GetType()))
            {
                _childs.Add(obj.GUID, new ScreenSession(view, this));
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
                if (Session["clicks"][obj.tag] != null)
                {
                    Session["clicks"][obj.tag]++;
                }
                else
                {
                    Session["clicks"][obj.tag] = 1;
                }
            }

            return true;
        }
    }
}