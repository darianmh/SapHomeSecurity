using SapSecurity.Model;
using SapSecurity.ViewModel;

namespace SapSecurity.Infrastructure.Repositories;

public interface IBaseRepository<T, in TId> where T : BaseEntity<TId>
{

    Task<List<T>> GetAllAsync();
    Task<PaginatedViewModel<T>> GetAllAsync(int pageSize, int pageIndex);
    Task InsertAsync(T entity);
    Task<T?> GetByIdAsync(TId id);
    void Update(T entity);
    Task SaveChangeAsync();
}
public interface IBaseRepository<T> : IBaseRepository<T, int> where T : BaseEntity
{
}