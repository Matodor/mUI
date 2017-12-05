using System;
using System.Collections.Generic;
using System.Linq;
using mFramework.UI;
using SimpleJSON;

namespace mFramework.Analytics
{
    /*public class ScreenSession
    {
        internal readonly JSONObject Session;
        internal readonly ScreenSession Parent;

        public readonly DateTime AttachTime;
        public readonly IView AttachedView;

        private readonly Dictionary<ulong, ScreenSession> _childs;
        private bool _lastVisible;
        private bool _updatedBeforeQuit;

        private DateTime _lastHideAt;
        private TimeSpan _totalHide;

        internal ScreenSession(IView view, ScreenSession parent)
        {
            Parent = parent;
            AttachTime = DateTime.Now;
            AttachedView = view;

            mCore.Log($"Attach ScreenSession: {view.GetType().Name}");

            _updatedBeforeQuit = false;
            _childs = new Dictionary<ulong, ScreenSession>();
            _lastVisible = view.IsShowing;
            _totalHide = TimeSpan.Zero;

            Session = new JSONObject
            {
                ["view_name"] = view.GetType().Name,
                ["created_at"] = DateTime.Now.ToString(""),
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

            if (parent != null)
            {
                parent._childs.Add(view.GUID, this);
                parent.Session["childs"].AsArray.Add(Session);

                mCore.Log($"parent = {parent.AttachedView.GetType().Name} child={view.GetType().Name}");
            }

            view.BeforeDestroy += ViewOnBeforeDestroy;
            view.VisibleChanged += ViewOnVisibleChanged;
        }

        public void Event(string key, JSONNode node)
        {
            Session["events"].AsArray.Add(new JSONObject
            {
                ["key"] = key,
                ["payload"] = node
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

        internal IEnumerable<ScreenSession> DeepChild()
        {
            foreach (var a in _childs)
            {
                foreach (var b in a.Value.DeepChild())
                    yield return b;
                yield return a.Value;
            }
            yield return this;
        }

        internal void Update()
        {
            if (_updatedBeforeQuit)
                return;
            ;
            if (!_lastVisible && !AttachedView.IsShowing)
            {
                _totalHide += DateTime.Now - _lastHideAt;
            }

            Session["screen_lifetime"] = (int)(DateTime.Now - AttachTime).TotalSeconds;
            Session["hidden_time"] = (int)_totalHide.TotalSeconds;
        }

        private void ViewOnBeforeDestroy(IUIObject sender)
        {
            foreach (var screenSession in DeepChild())
            {
                screenSession.Update();
                screenSession._updatedBeforeQuit = true;
            }

            Parent?.RemoveChild(AttachedView.GUID);

            sender.BeforeDestroy -= ViewOnBeforeDestroy;
            sender.VisibleChanged -= ViewOnVisibleChanged;
        }

        /*
        private void ObjOnChildObjectAdded(IUIObject sender, IUIObject addedObj)
        {
            RecursivelySetup(addedObj);
        }
        */

        /*
        private void RecursivelySetup(IUIObject obj)
        {
            if (obj is IView view && 
                ViewsTypes != null &&
                ViewsTypes.Contains(view.GetType()))
            {
                if (ChildTypes.TryGetValue(view.GetType(), out var parentType))
                {
                    var parent = ScreenSessions.Values.FirstOrDefault(s => s.AttachedView.GetType().Name == parentType.Name);
                    _childs.Add(view.GUID, new ScreenSession(view, parent));
                }
                else
                {
                    _childs.Add(view.GUID, new ScreenSession(view, this));
                }
            }
            else
            {
                //if (obj is IUIClickable clickable)
                //{
                //    clickable.UIClickable.CanMouseDown += UiClickableOnCanMouseDown;
                //}

                obj.ChildObjectAdded += ObjOnChildObjectAdded;
                obj.Childs.ForEach(RecursivelySetup);
            }
        }
        */

        /*        
        private bool UiClickableOnCanMouseDown(IUIClickable uiClickable, MouseEvent e)
        {
            var obj = uiClickable as UIObject;

            if (obj == null ||!obj.IsActive ||
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
        */
    /*}*/
}