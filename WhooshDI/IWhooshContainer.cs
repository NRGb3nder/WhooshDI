using System;

namespace WhooshDI
{
    /// <summary>
    /// Dependency injection container that supports named dependencies.
    /// </summary>
    public interface IWhooshContainer : IDisposable
    {
        /// <summary>
        /// Resolves dependency of a type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A type of dependency</typeparam>
        /// <returns>Resolved dependency.</returns>
        T Resolve<T>();
        
        /// <summary>
        /// Resolves dependency of a type <typeparamref name="T"/>
        /// </summary>
        /// <param name="name">A name of dependency implementation</param>
        /// <typeparam name="T">A type of dependency</typeparam>
        /// <returns>Resolved dependency.</returns>
        T Resolve<T>(object name);
    }
}