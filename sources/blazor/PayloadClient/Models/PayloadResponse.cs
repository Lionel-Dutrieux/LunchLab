namespace PayloadClient.Models;

public class PayloadResponse<T>
{
    public T? Doc { get; set; }
    public IEnumerable<T>? Docs { get; set; }
    public int? TotalDocs { get; set; }
    public int? Limit { get; set; }
    public int? TotalPages { get; set; }
    public int? Page { get; set; }
    public int? PrevPage { get; set; }
    public int? NextPage { get; set; }
    public bool? HasPrevPage { get; set; }
    public bool? HasNextPage { get; set; }
}