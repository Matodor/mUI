namespace mFramework.UI
{
    public delegate void UIEventHandler<in TSender>(TSender sender) where TSender : IUIObject;
    public delegate void UIEventHandler<in TSender, in T>(TSender sender, T arg1) where TSender : IUIObject;
    public delegate void UIEventHandler<in TSender, in T1, in T2>(TSender sender, T1 arg1, T2 arg2) where TSender : IUIObject;
    public delegate void UIEventHandler<in TSender, in T1, in T2, in T3>(TSender sender, T1 arg1, T2 arg2, T3 arg3) where TSender : IUIObject;
}
