namespace mFramework.UI
{
    public interface ISizeable
    {
        /// <summary>
        /// Element bound width in world unscaled units
        /// </summary>
        float SizeX { get; }

        /// <summary>
        /// Element bound height in world unscaled units
        /// </summary>
        float SizeY { get; }
    }
}