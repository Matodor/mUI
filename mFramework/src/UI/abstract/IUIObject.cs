using UnityEngine;

namespace mFramework.UI
{
    public interface IUIObject : IGlobalUniqueIdentifier
    {
        UnidirectionalList<UIAnimation> Animations { get; }
        UnidirectionalList<IUIObject> Childs { get; }

        bool IsActive { get; }
        bool IsShowing { get; }
        UIObject Parent { get; }
        IView ParentView { get; }

        GameObject gameObject { get; }
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
        void RemoveAnimations();
        void Destroy();

        IUIObject Disable();
        IUIObject Enable();

        UIRect GetRect();

        float UnscaledHeight();
        float UnscaledWidth();
        float GetHeight();
        float GetWidth();

        Vector2 GlobalScale();
        Vector2 LocalScale();

        Vector2 LocalTranslatedY(float y);
        Vector2 LocalTranslatedX(float x);
        Vector2 LocalTranslated(Vector2 vec);
        Vector2 LocalTranslated(float x, float y);

        IUIObject LocalPosX(float x);
        IUIObject LocalPosY(float y);
        IUIObject LocalPos(float x, float y);
        IUIObject LocalPos(Vector2 position);
        Vector2 LocalPos();

        Vector2 TranslatedY(float y);
        Vector2 TranslatedX(float x);
        Vector2 Translated(Vector2 vec);
        Vector2 Translated(float x, float y);

        IUIObject PosX(float x);
        IUIObject PosY(float y);
        IUIObject Pos(float x, float y);
        IUIObject Pos(Vector2 position);
        Vector2 Pos();

        IUIObject LocalRotate(float x, float y, float z);
        IUIObject Rotate(float angle);
        IUIObject Rotate(float x, float y, float z);
        float Rotation();

        IUIObject Scale(float v);
        IUIObject Scale(float x, float y);
        IUIObject Scale(Vector2 scale);

        IUIObject SetName(string newName);
        IUIObject Show();
        IUIObject Hide();

        int LocalSortingOrder();
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