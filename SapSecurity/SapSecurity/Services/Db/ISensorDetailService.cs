using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public interface ISensorDetailService
{
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


    Task<int> GetSensPercent(SensorDetail sensor);
    int GetSensPercent(int? lastValue, bool isDigital, double neutralValue);

    /// <summary>
    /// find sensors with no log in last seconds
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<SensorInfoModel>> GetDeActiveSensors(string userId);

    /// <summary>
    /// find sensor by identifier and cache
    /// </summary>
    /// <param name="sensorId"></param>
    /// <returns></returns>
    Task<SensorInfoModel?> GetSensorInfoByIdentifier(string sensorId);

    Task<List<SensorViewModel>> GetAllSensors(string userId);
}