using Microsoft.EntityFrameworkCore;
using SapSecurity.Model;
using SapSecurity.Data;

namespace SapSecurity.Infrastructure.Repositories;

public class ZoneRepository : BaseRepository<Zone>, IZoneRepository
{

    #region Fields



    #endregion
    #region Methods


    public Task<List<Zone>> GetUserZonesAsync(string userId)
    {
        return DbSet
            .Include(x => x.SensorDetails)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor

    public ZoneRepository(ApplicationContext context) : base(context)
    {
    }

    #endregion


}