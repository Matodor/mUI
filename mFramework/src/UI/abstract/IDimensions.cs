namespace mFramework.UI
{
    public interface IDimensions
    {
        /// <summary>
        /// In scaled world units
        /// </summary>
        float Height { get; }

        /// <summary>
        /// In scaled world units
        /// </summary>
        float Width { get; }

        /// <summary>
        /// In non scale world units
        /// </summary>
        float UnscaledWidth { get; }

        /// <summary>
        /// In non scale world units
        /// </summary>
        float UnscaledHeight { get; }
    }
}