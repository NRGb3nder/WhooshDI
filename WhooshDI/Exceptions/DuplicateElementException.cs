using System;

namespace WhooshDI.Exceptions
{
    public class DuplicateElementException : Exception
    {
        public DuplicateElementException()
        {
        }
        
        public DuplicateElementException(string message) : base(message)
        {
        }
    }
}