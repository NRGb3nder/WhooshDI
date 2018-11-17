using System;

namespace WhooshDI
{
    /// <summary>
    /// <see cref="IWhooshContainer"/> with scope support.
    /// </summary>
    public interface IWhooshScope : IWhooshContainer, IDisposable
    {
        /// <summary>
        /// Creates a new dependency resolution scope.
        /// </summary>
        /// <returns>A nested scope.</returns>
        IWhooshScope BeginScope();
    }
}