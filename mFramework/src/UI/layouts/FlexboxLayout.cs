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
                UnscaledHeight = 0f;
            else
                UnscaledWidth = 0f;
        }

        protected override void OnChildAdded(IUIObject sender, IUIObject child)
        {
            if (Direction == FlexboxDirection.COLUMN || Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                if (Childs.Count > 1)
                    UnscaledHeight += MarginBetween + child.Height;
                else
                    UnscaledHeight += child.Height;

                UnscaledCenterOffset = new Vector2(
                    0,
                    Direction == FlexboxDirection.COLUMN
                        ? -UnscaledHeight / 2
                        : +UnscaledHeight / 2
                );
            }
            else
            {
                if (Childs.Count > 1)
                    UnscaledWidth += MarginBetween + child.Width;
                else
                    UnscaledWidth += child.Width;

                UnscaledCenterOffset = new Vector2(
                    Direction == FlexboxDirection.ROW
                        ? +UnscaledWidth / 2
                        : -UnscaledWidth / 2,
                    0
                );
            }

            Vector2 localOffset;
            if (Childs.Count > 1)
            {
                switch (Direction)
                {
                    case FlexboxDirection.ROW:
                        localOffset = Childs.LastItem.Prev.Value.Position(UIAnchor.MiddleRight, Space.Self);
                        localOffset.x += MarginBetween + child.Width / 2f;
                        localOffset.y = 0f;
                        break;
                    case FlexboxDirection.ROW_REVERSE:
                        localOffset = Childs.LastItem.Prev.Value.Position(UIAnchor.MiddleLeft, Space.Self);
                        localOffset.x -= MarginBetween + child.Width / 2f;
                        localOffset.y = 0f;
                        break;
                    case FlexboxDirection.COLUMN:
                        localOffset = Childs.LastItem.Prev.Value.Position(UIAnchor.LowerCenter, Space.Self);
                        localOffset.y -= MarginBetween + child.Height / 2f;
                        localOffset.x = 0f;
                        break;
                    case FlexboxDirection.COLUMN_REVERSE:
                        localOffset = Childs.LastItem.Prev.Value.Position(UIAnchor.UpperCenter, Space.Self);
                        localOffset.y += MarginBetween + child.Height / 2f;
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
                        localOffset.x += Padding.Left;
                        break;
                    case FlexboxDirection.ROW_REVERSE:
                        localOffset.x -= Padding.Right;
                        break;
                    case FlexboxDirection.COLUMN:
                        localOffset.y -= Padding.Top;
                        break;
                    case FlexboxDirection.COLUMN_REVERSE:
                        localOffset.y += Padding.Bottom;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (Direction == FlexboxDirection.COLUMN || Direction == FlexboxDirection.COLUMN_REVERSE)
            {
                switch (AlignItems)
                {
                    case FlexboxAlignItems.FLEX_START:
                        localOffset.x = localOffset.x - UnscaledWidth / 2f + child.Width / 2f;
                        break;
                    case FlexboxAlignItems.FLEX_END:
                        localOffset.x = localOffset.x + UnscaledWidth / 2f - child.Width / 2f;
                        break;
                    case FlexboxAlignItems.STRETCH:
                        child.Scale(UnscaledWidth / child.Width, child.Scale.y);
                        break;
                }
            }
            else
            {
                switch (AlignItems)
                {
                    case FlexboxAlignItems.FLEX_START:
                        localOffset.y = localOffset.y - UnscaledHeight / 2f + child.Height / 2f;
                        break;
                    case FlexboxAlignItems.FLEX_END:
                        localOffset.y = localOffset.y + UnscaledHeight / 2f - child.Height / 2f;
                        break;
                    case FlexboxAlignItems.STRETCH:
                        child.Scale(child.Scale.x, UnscaledHeight / child.Height);
                        break;
                }
            }

            child.Position(localOffset, UIAnchor.MiddleCenter, Space.Self);
        }
    }
}