using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public static class UIToggleGroupHelper
    {
        public static UIToggleGroup CreateToggleGroup(this BaseView view, string objName = "ToggleGroup")
        {
            return new UIToggleGroup(view).SetName(objName);
        }
    }

    public class UIToggleGroup : UIObject
    {
        public override float Width { get { return 0; } }
        public override float Height { get { return 0; } }

        public event Action<UIToggle, UIToggle> OnChangeActiveToogle;

        private readonly List<UIToggle> _toggles;
        private UIToggle _currenToggle;

        public UIToggleGroup(BaseView view) : base(view)
        {
            _toggles = new List<UIToggle>();
            _currenToggle = null;

            OnChangeActiveState += OnChangeState;
        }

        private void OnChangeState(UIObject uiObject, bool oldState, bool newState)
        {
            for (int i = 0; i < _toggles.Count; i++)
                _toggles[i].Active = newState;
        }

        public UIToggle AddToggle(Sprite disabled, Sprite hoverSprite = null, Sprite enabled = null)
        {
            var toggle = new UIToggle(ParentView, disabled, hoverSprite, enabled)
                .SetName("Toggle [" + _toggles.Count + "]")
                .Enable(OnEnableToggle)
                .Disable(OnDisableToggle);

            toggle.SortingOrder(Renderer.sortingOrder);
            toggle.Transform.parent = Transform;
            toggle.Transform.localPosition = Vector3.zero;
            _toggles.Add(toggle);
            return toggle;
        }

        private void OnDisableToggle(UIObject sender, object obj)
        {
            var toggle = (UIToggle) sender;
            _currenToggle = null;
        }

        private void OnEnableToggle(UIObject sender, object obj)
        {
            var toggle = (UIToggle) sender;
            if (_currenToggle != null)
            {
                if (_currenToggle != toggle)
                {
                    _currenToggle.SetDisabled();
                }
            }
            OnChangeActiveToogle?.Invoke(_currenToggle, toggle);
            _currenToggle = toggle;
        }
    }
}
