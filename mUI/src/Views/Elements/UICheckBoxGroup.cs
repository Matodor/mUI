using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public static class UICheckBoxGroupHelper
    {
        public static UICheckBoxGroup CreateCheckBoxGroup(this BaseView view, string objName = "CheckBoxGroup")
        {
            return new UICheckBoxGroup(view).SetName(objName);
        }
    }

    public class UICheckBoxGroup : UIObject
    {
        public override float Width { get { return 0; } }
        public override float Height { get { return 0; } }

        private readonly List<UICheckBox> _checkBoxs; 

        public UICheckBoxGroup(BaseView view) : base(view)
        {
            _checkBoxs = new List<UICheckBox>();
        }

        public UICheckBox AddCheckBox(Sprite uncheck, Sprite hoverSprite = null, Sprite @checked = null)
        {
            var checkBox = new UICheckBox(ParentView, uncheck, hoverSprite, @checked)
                .SetName("CheckBox [" + _checkBoxs.Count + "]")
                .SortingOrder(SortingOrder + 1)
                .Check(OnCheck)
                .UnCheck(OnUnCheck);

            checkBox.Transform.parent = Transform;
            _checkBoxs.Add(checkBox);
            return checkBox;
        }

        private void OnUnCheck(UIObject uiObject, object obj)
        {
            var checkBox = (UICheckBox) uiObject;
        }

        private void OnCheck(UIObject uiObject, object obj)
        {
            var checkBox = (UICheckBox) uiObject;
        }
    }
}
