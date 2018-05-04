using UnityEngine;

namespace mFramework.UI
{
    public interface IUIObject : IGlobalUniqueIdentifier, IDimensions
    {
        UnidirectionalList<UIAnimation> Animations { get; }
        UnidirectionalList<IUIObject> Childs { get; }

        bool IsActive { get; }
        bool IsShowing { get; }
        UIObject Parent { get; }
        IView ParentView { get; }

        float Rotation { get; set; }
        UIRect Rect { get; }
        UIAnchor Anchor { get; set; }
        UIPadding Padding { get; set; }

        Vector3 LocalPosition { get; set; }
        Vector3 Position { get; set; }
        Vector3 Scale { get; set; }
        Vector3 GlobalScale { get; }
        Vector2 CenterOffset { get; }

        event UIEventHandler<IUIObject> ActiveChanged;
        event UIEventHandler<IUIObject, UIAnimation> AnimationAdded;
        event UIEventHandler<IUIObject> BeforeDestroy;
        event UIEventHandler<IUIObject, IUIObject> ChildObjectAdded;
        event UIEventHandler<IUIObject, IUIObject> ChildObjectRemoved;
        event UIEventHandler<IUIObject> SortingOrderChanged;
        event UIEventHandler<IUIObject> VisibleChanged;

        T Animation<T>(UIAnimationSettings settings) where T : UIAnimation;
        T Component<T>(UIComponentSettings settings) where T : UIComponent;

        void RemoveAnimations();
        void RemoveAnimations<T>() where T : UIAnimation;
        void Destroy();
        void DestroyChilds();

        IUIObject Disable();
        IUIObject Enable();

        IUIObject SetName(string newName);
        IUIObject Show();
        IUIObject Hide();

        int LocalSortingOrder();
        int SortingOrder();
        IUIObject SortingOrder(int sortingOrder);

        void Tick();
        void FixedTick();
        void LateTick();
    }
}