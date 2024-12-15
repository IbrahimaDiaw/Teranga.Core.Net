namespace Teranga.Core.Exceptions
{
    /// <summary>
    /// The exception of the application
    /// </summary>
    public class TerangaException : Exception
    {
        /// <summary>
        /// The exception of the application
        /// <param name="message"></param>
        /// </summary>
        public TerangaException(string message) : base(message) { }
        /// <summary>
        /// The exception of the application
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// </summary>
        public TerangaException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// The exception of the application
        /// </summary>
        public TerangaException() : base(){ }
    }
}
