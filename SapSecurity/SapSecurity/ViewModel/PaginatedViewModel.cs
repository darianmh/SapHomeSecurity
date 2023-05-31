namespace SapSecurity.ViewModel;

public class PaginatedViewModel<T>
{
    public List<T> Data { get; set; }
    public long Count { get; set; }
}