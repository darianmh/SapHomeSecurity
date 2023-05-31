using Microsoft.EntityFrameworkCore;
using SapSecurity.Model;
using SapSecurity.Data;

namespace SapSecurity.Infrastructure.Repositories;

public class SensorDetailRepository : BaseRepository<SensorDetail>, ISensorDetailRepository
{
    #region Fields



    #endregion
    #region Methods

    public override Task<SensorDetail?> GetByIdAsync(int id)
    {
        return DbSet.Include(x => x.SensorGroup)
            .Include(x => x.Zone)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<SensorDetail?> GetByIdentifier(string identifier)
        => await DbSet.FirstOrDefaultAsync(x => x.Identifier == identifier);

    public async Task<List<SensorDetail>> GetAllSensors(int zoneId)
    {
        return await DbSet.Where(x => x.ZoneId == zoneId)
            .Include(x => x.Zone)
            .Include(x => x.SensorGroup)
          .ToListAsync();
    }

    public async Task<List<SensorDetail>> GetAllSensors(string userId)
    {
        return await DbSet
            .Include(x => x.Zone)
            .Include(x => x.SensorGroup)
            .Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<List<SensorDetail>> GetDeActiveSensors(string userId)
    {
        var date = DateTime.UtcNow.AddSeconds(-20);
        return await DbSet
            .Include(x => x.SensorGroup)
            .Where(x => x.UserId == userId
                        &&
                        (x.SensorLogs == null
                         || x.SensorLogs.Count(sl => sl.DateTimeUtc >= date) == 0))
            .ToListAsync();
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor


    public SensorDetailRepository(ApplicationContext context) : base(context)
    {
    }

    #endregion

}