using System.Runtime.Serialization;

namespace EnglishClass.Common.Result
{
    [Serializable]
    public record Result<T> : ISerializable
    {
        public bool IsSuccess { get; private init; }
        public bool IsFailure => !IsSuccess;
        public string? ErrorMessage { get; private init; } = default;
        public T? Value { get; private init; }

        private Result(){}
        public static Result<T> Success(T value) => new() {IsSuccess = true, Value = value};
        public static Result<T> Failure(string format, params object?[] args) => new()
        {
            IsSuccess = false, ErrorMessage = string.Format(format, args)
        };

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsFailure", IsFailure);
            info.AddValue("IsSuccess", IsSuccess);
            if (IsFailure)
            {
                info.AddValue("Error", ErrorMessage);
            }

            if (IsSuccess)
            {
                info.AddValue("Value", Value);
            }
        }
    }
}