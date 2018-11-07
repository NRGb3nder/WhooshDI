using System;

namespace WhooshDI.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class DependencyKeyAttribute : Attribute
    {
        public object Key { get; }
        
        public DependencyKeyAttribute(object key) => Key = key;
    }
}