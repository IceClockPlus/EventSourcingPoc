namespace EventSourcingPoc.API.Contracts
{
    public record Result
    {
        public bool IsSuccess { get; init; }
        public Error? Error { get; init; }

        protected Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error);

        public static implicit operator Result(Error error) => Failure(error);
    }

    public record Result<T> : Result
    {
        public T? Value { get; init; }
        private Result(T value): base(true, null) => Value = value;
        private Result(Error error): base(false, error) => Value = default;

        public static implicit operator Result<T>(T value) => new(value);
        public static implicit operator Result<T>(Error error) => new(error);
    }

    public record Error
    {
        public string Message { get; init; }
        public ErrorType Type { get; init; }
        public Dictionary<string, string>? Details { get; init; }

        public Error(string message, ErrorType type, Dictionary<string, string>? details = null)
        {
            Message = message;
            Type = type;
            Details = details;
        }
    }

    public enum ErrorType
    {
        NotFound,
        ValidationError,
        Unauthorized,
        Forbidden,
        Conflict,
        InternalServerError
    }

}