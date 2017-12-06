namespace mFramework.UI
{
    public interface IView : IUIObject
    {
        ushort? StencilId { get; }

        float RelativeX(float t);
        float RelativeY(float t);
    }
}