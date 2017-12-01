namespace mFramework.UI
{
    public interface IView : IUIObject
    {
        ushort? StencilId { get; }
    }
}