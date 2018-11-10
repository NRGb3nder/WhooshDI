using System;

namespace WhooshDI.Exceptions
{
    /// <summary>
    /// Exception that is thrown when circular dependency is detected (and circular dependency considered invalid).
    /// </summary>
    public class CircularDependencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException"/> class.
        /// </summary>
        public CircularDependencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException"/> class.
        /// </summary>
        /// <param name="message">A message to provide with exception</param>
        public CircularDependencyException(string message) : base(message)
        {
        }
    }
}