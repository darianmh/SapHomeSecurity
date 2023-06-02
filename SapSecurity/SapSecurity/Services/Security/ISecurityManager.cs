using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Security;

public interface ISecurityManager
{
    /// <summary>
    /// measure possibility of danger according last user logs
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task MeasureDangerPossibility(string userId);

    /// <summary>
    /// send alert for user 
    /// </summary>
    /// <param name="userId">user to be notified</param>
    /// <param name="level"></param>
    /// <returns></returns>
    void SoundAlertAsync(string userId, AlertLevel level);

    /// <summary>
    /// call back for all sensors messages
    /// </summary>
    /// <param name="sensor"></param>
    /// <param name="sensValue"></param>
    /// <returns></returns>
    Task<int?> SensReceiver(SensorInfoModel sensor, int sensValue);
    /// <summary>
    /// start task for measurement
    /// </summary>
    /// <param name="user"></param>
    void RunSecurityTask(string user);
    /// <summary>
    /// stop task for measurement
    /// </summary>
    /// <param name="user"></param>
    void StopSecurityTask(string user);

}