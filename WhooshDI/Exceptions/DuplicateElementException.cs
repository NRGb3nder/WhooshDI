using System;

namespace WhooshDI.Exceptions
{
    /// <summary>
    /// Exception that is thrown when duplicate element is found (and duplication considered invalid).
    /// </summary>
    public class DuplicateElementException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementException"/> class.
        /// </summary>
        public DuplicateElementException()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementException"/> class.
        /// </summary>
        /// <param name="message">A message to provide with exception</param>
        public DuplicateElementException(string message) : base(message)
        {
        }
    }
}