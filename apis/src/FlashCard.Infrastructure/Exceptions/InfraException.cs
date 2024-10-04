namespace FlashCard.Infrastructure.Exceptions
{
    /// <summary>
    /// Base exception type for those are thrown by Domain Layer for Domain specific exceptions.
    /// </summary>
    [Serializable]
    public class InfraException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="InfraException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public InfraException (string message)
            : base (message) { }

        /// <summary>
        /// Creates a new <see cref="InfraException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public InfraException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}