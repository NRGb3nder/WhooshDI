using System;
using System.Collections.Generic;
using System.Linq;

namespace WhooshDI.Exceptions
{
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException()
        {
        }

        public CircularDependencyException(string message) : base(message)
        {
        }
    }
}