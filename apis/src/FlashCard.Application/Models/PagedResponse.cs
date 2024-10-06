namespace FlashCard.Application.Models;

public abstract record PagedResponse<T> where T : class
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public IEnumerable<T>? Items { get; set; }
}
