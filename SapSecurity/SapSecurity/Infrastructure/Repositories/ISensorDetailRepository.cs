using SapSecurity.Model;
using SapSecurity.ViewModel;

namespace SapSecurity.Infrastructure.Repositories;

public interface ISensorDetailRepository : IBaseRepository<SensorDetail>
{
    Task<SensorDetail?> GetByIdentifier(string identifier);
    Task<List<SensorDetail>> GetAllSensors(int zoneId);
    Task<List<SensorDetail>> GetAllSensors(string userId);
    Task<List<SensorDetail>> GetDeActiveSensors(string userId);
}