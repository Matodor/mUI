﻿using UnityEngine;

namespace mFramework.UI
{
    public interface IUIObject : IGlobalUniqueIdentifier, ISizeable
    {
        UnidirectionalList<UIAnimation> Animations { get; }
        UnidirectionalList<UIObject> Childs { get; }

        /// <summary>
        /// Object is enabled and active in hierarchy
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Object is active in hierarchy
        /// </summary>
        bool IsShowing { get; }

        /// <summary>
        /// Parent UIObject
        /// </summary>
        UIObject Parent { get; }

        IView ParentView { get; }

        /// <summary>
        /// Set the local sorting order, get the sorting order in hierarchy
        /// </summary>
        int SortingOrder { get; set; }
        
        /// <summary>
        /// Get/set local sorting order
        /// </summary>
        int LocalSortingOrder { get; set; }

        float Rotation { get; set; }
        float LocalRotation { get; set; }

        /// <summary>
        /// Return rotated rectangle of UIObject, borders in world unit space
        /// </summary>
        UIRect Rect { get; }
        UIAnchor Anchor { get; set; }
        UIPadding Padding { get; set; }

        Vector3 LocalPosition { get; set; }
        Vector3 Position { get; set; }
        Vector3 Scale { get; set; }
        Vector3 GlobalScale { get; }

        //Vector2 CenterOffset { get; }
        Vector2 UnscaledCenterOffset { get; }

        /// <summary>
        /// Global scaled height with padding
        /// </summary>
        float Height { get; }

        /// <summary>
        /// Global scaled width with padding
        /// </summary>
        float Width { get; }

        /// <summary>
        /// Local scaled height with padding
        /// </summary>
        float LocalHeight { get; }

        /// <summary>
        /// Local scaled width with padding
        /// </summary>
        float LocalWidth { get; }

        /// <summary>
        /// Scaled height with padding
        /// </summary>
        float UnscaledHeight { get; }

        /// <summary>
        /// Scaled width with padding
        /// </summary>
        float UnscaledWidth { get; }
        
        event UIEventHandler<IUIObject> ActiveChanged;
        event UIEventHandler<IUIObject, UIAnimation> AnimationAdded;
        event UIEventHandler<IUIObject> BeforeDestroy;
        event UIEventHandler<IUIObject, IUIObject> ChildAdded;
        event UIEventHandler<IUIObject> SortingOrderChanged;
        event UIEventHandler<IUIObject> VisibleChanged;

        T Animation<T>(UIAnimationSettings settings) where T : UIAnimation, new();
        T Component<T>(UIComponentProps props) where T : UIComponent;

        void RemoveAnimations();
        void RemoveAnimations<T>() where T : UIAnimation;
        void Destroy();
        void DestroyChilds();

        IUIObject Disable();
        IUIObject Enable();

        IUIObject SetName(string newName);
        IUIObject Show();
        IUIObject Hide();

        void Tick();
        void FixedTick();
        void LateTick();
    }
}