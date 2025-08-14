namespace Blog_API.Exceptions
{
    public class DatabaseOperationException : Exception
    {
        public DatabaseOperationException() : base() { }
        public DatabaseOperationException(string message) : base(message) { }
        public DatabaseOperationException(string message, Exception inner) : base(message, inner) { }

    }
}
