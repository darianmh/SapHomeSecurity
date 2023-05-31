using SapSecurity.Model;

namespace SapSecurity.Services.Db;

public interface ISensorLogService
{
    Task LogAsync(double status, int sensorDetailId);

    Task SaveChangesAsync();
    Task<SensorLog?> GetLastLogAsync(int sensorId);
}