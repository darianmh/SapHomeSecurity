using SapSecurity.Model;

namespace SapSecurity.Infrastructure.Repositories;

public interface IZoneRepository : IBaseRepository<Zone>
{
    Task<List<Zone>> GetUserZonesAsync(string userId);
}