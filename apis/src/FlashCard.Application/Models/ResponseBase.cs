namespace FlashCard.Application.Models;

public abstract record ResponseBase<T> where T : class
{
    public bool Succeeded { get; set; }
    public IDictionary<string, string>? Errors { get; set; }
    public T? Data { get; set; }
}


public abstract record ResponseBase
{
    public bool Succeeded { get; set; }
    public IDictionary<string, string>? Errors { get; set; }
}
