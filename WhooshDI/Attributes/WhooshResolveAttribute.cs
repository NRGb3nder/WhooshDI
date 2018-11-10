using System;

namespace WhooshDI.Attributes
{
    /// <summary>
    /// Attribute that is used to mark a property for dependency injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class WhooshResolveAttribute : Attribute
    {
    }
}