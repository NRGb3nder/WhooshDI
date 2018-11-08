using System;

namespace WhooshDI.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute : Attribute
    {
        public object Key { get; }
        
        public DependencyKeyAttribute(object key) => Key = key;
    }
}