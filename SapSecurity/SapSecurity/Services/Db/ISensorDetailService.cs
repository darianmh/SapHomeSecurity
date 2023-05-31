using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public interface ISensorDetailService
{
    Task<string?> GetUserBySensor(int sensorId);
    /// <summary>
    /// find by unique key
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Task<SensorDetail?> GetByIdentifierAsync(string identifier);
    /// <summary>
    /// find by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<SensorDetail?> GetByIdAsync(int id);

    /// <summary>
    /// set sensor as active ot de active
    /// </summary>
    /// <param name="sensorId">sensor id</param>
    /// <param name="sensorState">0 is de active and 1 is active</param>
    /// <returns></returns>
    Task<bool> SetSensorState(int sensorId, int sensorState);

    Task<List<SensorViewModel>> GetAllSensors(int zoneId);
    Task<SensorViewModel?> GetSensorViewModel(SensorDetail sensorDetail);
    Task<SensorViewModel?> GetByIdViewModelAsync(int id);


    Task<int> GetSensPercent(SensorDetail sensor);
    int GetSensPercent(SensorDetail sensor, double? lastValue);

    /// <summary>
    /// find sensors with no log in last seconds
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<SensorDetail>> GetDeActiveSensors(string userId);
}