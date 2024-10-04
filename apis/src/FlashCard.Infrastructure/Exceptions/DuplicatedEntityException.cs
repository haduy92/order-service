namespace FlashCard.Infrastructure.Exceptions
{
    /// <summary>
    /// This exception is thrown if an entity excepted to be found but not found.
    /// </summary>
    [Serializable]
    public class DuplicatedEntityException : InfraException
    {
        private const string _errorFormat = "Entity is duplicated. Entity type: {0}, id: {1}";

        /// <summary>
        /// Creates a new <see cref="DuplicatedEntityException"/> object.
        /// </summary>
        public DuplicatedEntityException(Type entityType, object id)
            : base(string.Format(_errorFormat, entityType.FullName, id))
        { }

        /// <summary>
        /// Creates a new <see cref="DuplicatedEntityException"/> object.
        /// </summary>
        public DuplicatedEntityException(Type entityType, object id, Exception innerException)
            : base(string.Format(_errorFormat, entityType.FullName, id), innerException)
        { }

        /// <summary>
        /// Creates a new <see cref="DuplicatedEntityException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public DuplicatedEntityException(string message)
            : base(message)
        { }

        /// <summary>
        /// Creates a new <see cref="DuplicatedEntityException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public DuplicatedEntityException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}