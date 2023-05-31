using Microsoft.EntityFrameworkCore;
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Model;
using SapSecurity.ViewModel;
using SapSecurity.Data;

namespace SapSecurity.Infrastructure.Repositories;

public class SensorGroupRepository : BaseRepository<SensorGroup>, ISensorGroupRepository
{

    #region Fields



    #endregion
    #region Methods

    public override async Task<List<SensorGroup>> GetAllAsync()
        => await DbSet
            .Include(x=>x.SensorDetails)
            .ThenInclude(x=>x.Zone)
            .Include(x => x.SensorDetails)
            .ToListAsync();

    public override async Task<PaginatedViewModel<SensorGroup>> GetAllAsync(int pageSize, int pageIndex)
        => await DbSet
            .Include(x => x.SensorDetails)
            .ThenInclude(x => x.Zone)
            .Include(x => x.SensorDetails)
            .PaginateAsync(pageSize, pageIndex);


    #endregion
    #region Utilities



    #endregion
    #region Ctor


    public SensorGroupRepository(ApplicationContext context) : base(context)
    {
    }

    #endregion

}