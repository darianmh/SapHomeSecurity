using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Model;
using SapSecurity.ViewModel;
using SapSecurity.Data;

namespace SapSecurity.Infrastructure.Repositories;

public class BaseRepository<T, TId> : IBaseRepository<T, TId> where T : BaseEntity<TId>
{

    #region Fields

    protected readonly ApplicationContext _context;
    protected DbConnection DbConnection => _context.Database.GetDbConnection();
    protected DbSet<T> DbSet => _context.Set<T>();

    #endregion
    #region Methods
    public virtual async Task<List<T>> GetAllAsync()
        => await DbSet.OrderByDescending(x=>x.Id).ToListAsync();

    public virtual async Task<PaginatedViewModel<T>> GetAllAsync(int pageSize, int pageIndex)
        => await DbSet.OrderByDescending(x => x.Id).PaginateAsync(pageSize, pageIndex);

    public virtual async Task InsertAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public virtual async Task<T?> GetByIdAsync(TId id)
        => await DbSet.FirstOrDefaultAsync(x => x.Id!.Equals(id));

    public void Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    public async Task SaveChangeAsync() => await _context.SaveChangesAsync();

    #endregion
    #region Utilities


    #endregion
    #region Ctor

    public BaseRepository(ApplicationContext context)
    {
        _context = context;
    }


    #endregion
}
public class BaseRepository<T> : BaseRepository<T, int>, IBaseRepository<T> where T : BaseEntity
{

    #region Fields



    #endregion
    #region Methods



    #endregion
    #region Utilities



    #endregion
    #region Ctor

    public BaseRepository(ApplicationContext context) : base(context)
    {
    }

    #endregion

}