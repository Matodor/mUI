using System;
using UnityEngine;

namespace mFramework.UI.Layouts
{
    public class FlexboxLayoutSettings : UILayoutSettings
    {
        public virtual FlexboxDirection Direction { get; set; } = FlexboxDirection.COLUMN;
        public virtual float MarginBetween { get; set; } = 0f;
    }

    public enum FlexboxDirection
    {
        ROW = 0,
        ROW_REVERSE,
        COLUMN,
        COLUMN_REVERSE
    }

    public class FlexboxLayout : UILayout
    {
        public FlexboxDirection Direction { get; private set; }
        public float MarginBetween { get; private set; }

        protected override void CreateInterface(object[] @params)
        {

        }

        protected override void OnChildObjectAdded(IUIObject sender, IUIObject child)
        {
            if (Childs.Count == 1)
            {
                UnscaledHeight = child.UnscaledHeight;
                UnscaledWidth = child.UnscaledWidth;

                switch (Direction)
                {
                    case FlexboxDirection.ROW:
                        child.Position(this.Position(UIAnchor.MiddleCenter), UIAnchor.MiddleLeft);
                        UnscaledCenterOffset = new Vector2(+child.Width / 2, 0);
                        break;
                    case FlexboxDirection.ROW_REVERSE:
                        child.Position(this.Position(UIAnchor.MiddleCenter), UIAnchor.MiddleRight);
                        UnscaledCenterOffset = new Vector2(-child.Width / 2, 0);
                        break;
                    case FlexboxDirection.COLUMN:
                        child.Position(this.Position(UIAnchor.MiddleCenter), UIAnchor.UpperCenter);
                        UnscaledCenterOffset = new Vector2(0, -child.Height / 2);
                        break;
                    case FlexboxDirection.COLUMN_REVERSE:
                        child.Position(this.Position(UIAnchor.MiddleCenter), UIAnchor.LowerCenter);
                        UnscaledCenterOffset = new Vector2(0, +child.Height / 2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return;
            }

            var prev = Childs.LastItem.Prev.Value;
            switch (Direction)
            {
                case FlexboxDirection.ROW:
                    var right = prev.Position(UIAnchor.MiddleRight) 
                        + new Vector3(MarginBetween * GlobalScale.x, 0);
                    child.Position(right, UIAnchor.MiddleLeft);
                    break;

                case FlexboxDirection.ROW_REVERSE:
                    var left = prev.Position(UIAnchor.MiddleLeft) 
                        - new Vector3(MarginBetween * GlobalScale.x, 0);
                    child.Position(left, UIAnchor.MiddleRight);
                    break;

                case FlexboxDirection.COLUMN:
                    var bottom = prev.Position(UIAnchor.LowerCenter) 
                        - new Vector3(0, MarginBetween * GlobalScale.y);
                    child.Position(bottom, UIAnchor.UpperCenter);
                    break;

                case FlexboxDirection.COLUMN_REVERSE:
                    var top = prev.Position(UIAnchor.UpperCenter)
                        + new Vector3(0, MarginBetween * GlobalScale.y);
                    child.Position(top, UIAnchor.LowerCenter);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            var first = Childs.FirstItem.Value.Rect;
            var last = Childs.LastItem.Value.Rect;

            if (Direction == FlexboxDirection.ROW || Direction == FlexboxDirection.ROW_REVERSE)
            {
                UnscaledWidth += MarginBetween * GlobalScale.x + child.UnscaledWidth;

                if (UnscaledHeight < child.UnscaledHeight)
                    UnscaledHeight = child.UnscaledHeight;

                var w = Vector2.Distance(first.Left, last.Right);
                UnscaledCenterOffset = new Vector2(
                    Direction == FlexboxDirection.ROW
                        ? +UnscaledWidth / 2
                        : -UnscaledWidth / 2, 
                    0
                );
            }
            else
            {
                UnscaledHeight += MarginBetween * GlobalScale.y + child.UnscaledHeight;

                if (UnscaledWidth < child.UnscaledWidth)
                    UnscaledWidth = child.UnscaledWidth;

                var h = Vector2.Distance(first.Top, last.Bottom);
                UnscaledCenterOffset = new Vector2(
                    0, 
                    Direction == FlexboxDirection.COLUMN
                        ? -UnscaledHeight / 2
                        : +UnscaledHeight / 2
                );
            }
        }

        protected override void ApplySettings(UIViewSettings settings, IView parent)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is FlexboxLayoutSettings layoutSettings))
                throw new ArgumentException("FlexboxLayout: The given settings is not FlexboxLayoutSettings");

            base.ApplySettings(settings, parent);

            Direction = layoutSettings.Direction;
            MarginBetween = layoutSettings.MarginBetween;
            UnscaledWidth = 0f;
            UnscaledHeight = 0f;
        }
    }
}