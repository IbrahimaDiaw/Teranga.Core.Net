namespace Teranga.Core.Exceptions
{
    public class TerangaException : Exception
    {
        public TerangaException(string message) : base(message) { }
        public TerangaException(string message, Exception inner) : base(message, inner) { }
    }
}
