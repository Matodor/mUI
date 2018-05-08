﻿using System.Collections.Generic;

namespace mFramework.UI
{
    internal static class UIClickablesHandler
    {
        private static readonly UnidirectionalList<IUIClickable> _clickables;
        private static readonly UnidirectionalList<IUIDragable> _dragables;

        static UIClickablesHandler()
        {
            _clickables = UnidirectionalList<IUIClickable>.Create();
            _dragables = UnidirectionalList<IUIDragable>.Create();
        }

        public static void AddDragable(IUIDragable dragable)
        {
            dragable.BeforeDestroy += sender => _dragables.Remove(dragable);
            _dragables.Add(dragable);
        }

        public static void AddClickable(IUIClickable clickable)
        {
            clickable.BeforeDestroy += sender => _clickables.Remove(clickable);
            _clickables.Add(clickable);
        }

        public static void MouseDrag(MouseEvent e)
        {
            var worldPos = mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);

            foreach (var dragable in _dragables)
            {
                if (!dragable.IsActive)
                    continue;

                dragable.MouseDrag(worldPos);
            }
        }

        public static void MouseUp(MouseEvent e)
        {
            var worldPos = mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);

            foreach (var clickable in _clickables)
            {
                if (!clickable.IsActive)
                    continue;

                clickable.MouseUp(worldPos);
            }
        }

        public static void MouseDown(MouseEvent e)
        {
            var worldPos = mUI.UICamera.ScreenToWorldPoint(e.MouseScreenPos);

            foreach (var clickable in _clickables)
            {
                if (!clickable.IsActive)
                    continue;

                if (clickable.AreaChecker.InAreaShape(clickable, worldPos))
                    clickable.MouseDown(worldPos);
            }
        }
    }
}