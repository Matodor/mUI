using System;
using UnityEngine;

namespace mFramework.UI.Layouts
{
    public class FlexboxLayoutProps : UILayoutProps
    {
        public virtual FlexboxDirection Direction { get; set; } = FlexboxDirection.COLUMN;
        public virtual FlexboxAlignItems AlignItems { get; set; } = FlexboxAlignItems.CENTER;
        public virtual float MarginBetween { get; set; } = 0f;
    }

    public enum FlexboxAlignItems
    {
        FLEX_START = 0,
        FLEX_END,
        CENTER,
        STRETCH,
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
        public FlexboxAlignItems AlignItems { get; private set; }
        public float MarginBetween { get; private set; }

        protected override void CreateInterface(object[] @params)
        {

        }

        protected override void ApplyProps(UIViewProps props, IView parent)
        {
            if (props == null)
                throw new ArgumentNullException(nameof(props));

            if (!(props is FlexboxLayoutProps layoutSettings))
                throw new ArgumentException("FlexboxLayout: The given settings is not FlexboxLayoutSettings");

            base.ApplyProps(props, parent);

            AlignItems = layoutSettings.AlignItems;
            Direction = layoutSettings.Direction;
            MarginBetween = layoutSettings.MarginBetween;

            if (Direction == FlexboxDirection.COLUMN || Direction == FlexboxDirection.COLUMN_REVERSE)
                SizeY = 0f;
            else
                SizeX = 0f;
        }

        protected override void OnChildAdded(IUIObject sender, IUIObject child)
        {
            var marginBetween = MarginBetween;

            if (Direction == FlexboxDirection.COLUMN || Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                if (Childs.Count > 1)
                    SizeY += marginBetween * GlobalScale.y + child.UnscaledHeight;
                else
                    SizeY += child.UnscaledHeight;

                UnscaledCenterOffset = new Vector2(
                    0,
                    Direction == FlexboxDirection.COLUMN
                        ? -SizeY / 2
                        : +SizeY / 2
                );
            }
            else
            {
                if (Childs.Count > 1)
                    SizeX += marginBetween * GlobalScale.x + child.UnscaledWidth;
                else
                    SizeX += child.UnscaledWidth;

                UnscaledCenterOffset = new Vector2(
                    Direction == FlexboxDirection.ROW
                        ? +SizeX / 2
                        : -SizeX / 2,
                    0
                );
            }

            Vector2 localOffset;
            if (Childs.Count > 1)
            {
                var prevChild = Childs.LastItem.Prev.Value;
                var w = marginBetween + child.UnscaledWidth / 2f;
                var h = marginBetween + child.UnscaledHeight / 2f;
                
                switch (Direction)
                {
                    case FlexboxDirection.ROW:
                        localOffset = prevChild.Position(UIAnchor.MiddleRight, Space.Self);
                        localOffset.x += w;
                        localOffset.y = 0f;
                        break;

                    case FlexboxDirection.ROW_REVERSE:
                        localOffset = prevChild.Position(UIAnchor.MiddleLeft, Space.Self);
                        localOffset.x -= w;
                        localOffset.y = 0f;
                        break;

                    case FlexboxDirection.COLUMN:
                        localOffset = prevChild.Position(UIAnchor.LowerCenter, Space.Self);
                        localOffset.y -= h;
                        localOffset.x = 0f;
                        break;

                    case FlexboxDirection.COLUMN_REVERSE:
                        localOffset = prevChild.Position(UIAnchor.UpperCenter, Space.Self);
                        localOffset.y += h;
                        localOffset.x = 0f;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                localOffset = UnscaledCenterOffset;

                switch (Direction)
                {
                    case FlexboxDirection.ROW:
                        localOffset.x += Padding.Left * Scale.x;
                        break;
                    case FlexboxDirection.ROW_REVERSE:
                        localOffset.x -= Padding.Right * Scale.x;
                        break;
                    case FlexboxDirection.COLUMN:
                        localOffset.y -= Padding.Top * Scale.y;
                        break;
                    case FlexboxDirection.COLUMN_REVERSE:
                        localOffset.y += Padding.Bottom * Scale.y;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            /* TODO FIX
            if (Direction == FlexboxDirection.COLUMN || Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                switch (AlignItems)
                {
                    case FlexboxAlignItems.FLEX_START:
                        localOffset.x = localOffset.x - SizeX / 2f + child.Width / 2f;
                        break;
                    case FlexboxAlignItems.FLEX_END:
                        localOffset.x = localOffset.x + SizeX / 2f - child.Width / 2f;
                        break;
                    case FlexboxAlignItems.STRETCH:
                        child.Scale(SizeX / child.Width, child.Scale.y);
                        break;
                }
            }
            else
            {
                switch (AlignItems)
                {
                    case FlexboxAlignItems.FLEX_START:
                        localOffset.y = localOffset.y - SizeY / 2f + child.Height / 2f;
                        break;
                    case FlexboxAlignItems.FLEX_END:
                        localOffset.y = localOffset.y + SizeY / 2f - child.Height / 2f;
                        break;
                    case FlexboxAlignItems.STRETCH:
                        child.Scale(child.Scale.x, SizeY / child.Height);
                        break;
                }
            }*/

            child.Position(localOffset, UIAnchor.MiddleCenter, Space.Self);
        }
    }
}