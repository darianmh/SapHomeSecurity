namespace SapSecurity.Model;

public class BaseEntity<TId>
{
    public TId Id { get; set; }
}
public class BaseEntity : BaseEntity<int>
{
}