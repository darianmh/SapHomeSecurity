using System.Data.Common;
using SapSecurity.Model;

namespace SapSecurity.Infrastructure.Repositories;

public interface ISensorLogRepository : IBaseRepository<SensorLog>
{
    /// <summary>
    /// get sensor logs from determined time
    /// </summary>
    /// <param name="sensorId">sensor</param>
    /// <param name="dateTime">from date time</param>
    /// <returns></returns>
    Task<List<SensorLog>> GetLastLogsBySensor(int sensorId, DateTime dateTime);

    Task<SensorLog?> GetLastLogAsync(int sensorId);
}