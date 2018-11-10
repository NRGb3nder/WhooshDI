using System;
using System.ComponentModel;

namespace WhooshDI.Syntax
{
    /// <summary>
    /// Interface that hides object methods of its implementations from IntelliSense.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFluentSyntax
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object other);
    }
}