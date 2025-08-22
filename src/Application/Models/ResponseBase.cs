namespace Application.Models;

public abstract record ResponseBase<T> where T : class
{
    public bool Succeeded => Errors == null || !Errors.Any();
    public IEnumerable<string>? Errors { get; set; }
    public T? Data { get; set; }
}

public abstract record ResponseBase
{
    public bool Succeeded => Errors == null || !Errors.Any();
    public IEnumerable<string>? Errors { get; set; }
}

/// <summary>
/// Success response wrapper
/// </summary>
public sealed record SuccessResponse<T> : ResponseBase<T> where T : class
{
    public SuccessResponse() { Errors = null; }
}

/// <summary>
/// Success response for empty responses
/// </summary>
public sealed record SuccessResponse : ResponseBase
{
    public SuccessResponse() { Errors = null; }
}

/// <summary>
/// Error response wrapper
/// </summary>
public sealed record ErrorResponse<T> : ResponseBase<T> where T : class
{
    public ErrorResponse(IEnumerable<string> errors)
    {
        Errors = errors;
        Data = null;
    }

    public ErrorResponse(string error)
    {
        Errors = new[] { error };
        Data = null;
    }
}

/// <summary>
/// Error response for empty responses
/// </summary>
public sealed record ErrorResponse : ResponseBase
{
    public ErrorResponse(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    public ErrorResponse(string error)
    {
        Errors = new[] { error };
    }
}

