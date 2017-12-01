using UnityEngine;

namespace mFramework.UI
{
    public interface IUIObject : IGlobalUniqueIdentifier
    {
        UnidirectionalList<UIAnimation> Animations { get; }
        UnidirectionalList<IUIObject> Childs { get; }
        GameObject gameObject { get; }
        bool IsActive { get; }
        bool IsShowing { get; }
        UIObject Parent { get; }
        string tag { get; set; }
        Transform transform { get; }

        event UIEventHandler<IUIObject> ActiveChanged;
        event UIEventHandler<IUIObject, UIAnimation> AnimationAdded;
        event UIEventHandler<IUIObject> BeforeDestroy;
        event UIEventHandler<IUIObject, IUIObject> ChildObjectAdded;
        event UIEventHandler<IUIObject, IUIObject> ChildObjectRemoved;
        event UIEventHandler<IUIObject> SortingOrderChanged;
        event UIEventHandler<IUIObject> VisibleChanged;

        T Animation<T>(UIAnimationSettings settings) where T : UIAnimation;
        T Component<T>(UIComponentSettings settings) where T : UIComponent;
        void Destroy();
        IUIObject Disable();
        IUIObject Enable();
        float GetHeight();
        UIRect GetRect();
        float GetWidth();
        Vector2 GlobalScale();
        IUIObject Hide();
        Vector2 LocalPosition();
        IUIObject LocalPosition(float x, float y);
        IUIObject LocalPosition(Vector2 position);
        IUIObject LocalRotate(float x, float y, float z);
        Vector2 LocalScale();
        int LocalSortingOrder();
        Vector2 Position();
        IUIObject Position(float x, float y);
        IUIObject Position(Vector2 position);
        IUIObject PositionX(float x);
        IUIObject PositionY(float y);
        void RemoveAnimations();
        IUIObject Rotate(float angle);
        IUIObject Rotate(float x, float y, float z);
        float Rotation();
        IUIObject Scale(float v);
        IUIObject Scale(float x, float y);
        IUIObject Scale(Vector2 scale);
        IUIObject SetName(string newName);
        IUIObject Show();
        int SortingOrder();
        IUIObject SortingOrder(int sortingOrder);
        IUIObject Translate(float x, float y);
        IUIObject Translate(float x, float y, float z);
        IUIObject Translate(Vector2 translatePos);
        void OnSortingOrderChanged();
        void Tick();
        void FixedTick();
        void LateTick();
    }
}