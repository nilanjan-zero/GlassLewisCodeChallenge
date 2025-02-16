namespace Company.Domain.Exceptions.Database
{
    public class DuplicateEntityException : Exception
    {
        public DuplicateEntityException()
        {
        }

        public DuplicateEntityException(string message)
            : base(message)
        {
        }

        public DuplicateEntityException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
