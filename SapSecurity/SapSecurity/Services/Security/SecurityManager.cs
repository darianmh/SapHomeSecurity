using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Connection;
using SapSecurity.Services.Db;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Security;

public class SecurityManager : ISecurityManager
{

    #region Fields

    private readonly ISensorDetailService _sensorDetailService;
    private readonly IUserSocketManager _userSocketManager;
    private readonly IUserSmsManager _userSmsManager;
    private readonly IUserWebSocketManager _userWebSocketManager;
    private readonly IHomeUdpSocketManager _homeUdpSocketManager;
    private readonly IMapManager _mapManager;

    #endregion
    #region Methods

    public async Task MeasureDangerPossibility(string userId)
    {
        var securityStatus = CacheManager.GetUserSecurityActivate(userId);
        if (securityStatus != true)
        {
            CacheManager.SetSecurityStatus(userId, SensorStatus.DeActive);
        }
        else
        {
            var status = IndexManager.GetUserHomeStatus(userId);
            CacheManager.SetSecurityStatus(userId, status);
        }
        await SendStatus(userId);
    }


    public void SoundAlertAsync(string userId, AlertLevel level)
    {
        Console.WriteLine($"Sound alert User: {userId} Alert: {level}");
        _ = SoundAlertUserAsync(userId, level);
        _ = SoundAlertHomeAsync(userId, level);
        _ = SoundAlertMap(userId, level);
    }


    public async Task<int?> SensReceiver(SensorInfoModel sensor, int sensValue)
    {
        if (sensValue != sensor.NeutralValue)
        {
            var weight = sensor.Weight;
            var index = weight;
            if (!sensor.IsDigital)
            {
                if (sensor.NeutralValue != 0)
                    index = Convert.ToInt32((sensor.NeutralValue - sensValue) / sensor.NeutralValue * 100);
            }
            //else if (sensor.WeightPercent != 100)
            //{
            //    //for something like weight sensors
            //    var sensorLastIndex = IndexManager.GetSensorIndex(sensor.SensorId);
            //    index = weight * sensor.WeightPercent / 100;
            //    index += sensorLastIndex;
            //}
            IndexManager.SetIndex(sensor, index, sensValue);
        }
        else
        {
            IndexManager.SetIndex(sensor, 0, sensValue);
        }
        await MeasureDangerPossibility(sensor.UserId);

        return GetSensorResponse(sensor);

    }



    public void RunSecurityTask(string user)
    {
        if (CacheManager.UserSecurityCheck.ContainsKey(user))
        {
            CacheManager.UserSecurityCheck[user] = true;
        }
        else CacheManager.UserSecurityCheck.TryAdd(user, true);
        var thread = new Thread(async () =>
        {
            var c = 0;
            while (CacheManager.UserSecurityCheck.TryGetValue(user, out var check) && check)
            {
                if (c < 6)
                {
                    await Task.Delay(10000);
                    c++;
                    continue;
                }

                c = 0;
                //find de active sensors
                var deActiveSensors = await _sensorDetailService.GetDeActiveSensors(userId: user);
                deActiveSensors.ForEach(x => IndexManager.SetIndex(x, x.Weight, null));
                Console.WriteLine(string.Join(",", IndexManager.Index.Select(x => $"{x.SensorId}: {x.IndexValue}")));
                await MeasureDangerPossibility(user);
            }

        });
        thread.Start();
    }

    public void StopSecurityTask(string user)
    {
        if (CacheManager.UserSecurityCheck.ContainsKey(user))
        {
            CacheManager.UserSecurityCheck[user] = false;
        }
    }

    #endregion
    #region Utilities
    private int? GetSensorResponse(SensorInfoModel sensor)
    {
        var specialMessage = CacheManager.ReadSpecialMessage(sensor.SensorId);
        if (specialMessage != null) return specialMessage;


        //sensors
        if (sensor.GroupType == SensorGroupType.Sensor)
            return null;

        //like spray
        if (sensor.GroupType == SensorGroupType.ZoneReceiver)
        {
            if (!CacheManager.GetUserSecurityActivate(sensor.UserId)) return 0;
            var zoneStatus = IndexManager.GetZoneStatus(sensor.ZoneId, sensor.UserId);
            if (zoneStatus == SensorStatus.Danger || zoneStatus == SensorStatus.Warning)
            {
                //spray ones
                CacheManager.SetSpecialMessage(sensor.SensorId, 0, false);
                return 1;
            }
            if (zoneStatus == SensorStatus.Active || zoneStatus == SensorStatus.DeActive) return 0;
            return null;
        }

        //like other alarms
        if (sensor.GroupType == SensorGroupType.HomeReceiver)
        {
            if (!CacheManager.GetUserSecurityActivate(sensor.UserId)) return 0;
            var homeStatus = IndexManager.GetUserHomeStatus(sensor.UserId);
            if (homeStatus == SensorStatus.Danger || homeStatus == SensorStatus.Warning)
            {
                return 1;
            }
            if (homeStatus == SensorStatus.Active || homeStatus == SensorStatus.DeActive) return 0;
            return null;
        }
        //like critical alarm
        if (sensor.GroupType == SensorGroupType.CriticalHomeReceiver)
        {
            if (!CacheManager.GetUserSecurityActivate(sensor.UserId))
            {
                var sensorStatus = IndexManager.GetSensorStatus(sensor.SensorId, sensor.ZoneId, sensor.UserId);
                if (sensorStatus == SensorStatus.Danger || sensorStatus == SensorStatus.Warning) return 1;
                return 0;
            }
            var homeStatus = IndexManager.GetUserHomeStatus(sensor.UserId);
            if (homeStatus == SensorStatus.Danger || homeStatus == SensorStatus.Warning) return 1;
            if (homeStatus == SensorStatus.Active || homeStatus == SensorStatus.DeActive) return 0;
            return null;
        }

        //like door lock
        if (sensor.GroupType == SensorGroupType.HomeSecurityDepend)
        {
            var homeSecurity = CacheManager.GetUserSecurityActivate(sensor.UserId);
            if (homeSecurity) return 0;
            return 1;
        }

        return null;
    }

    private async Task SendStatus(string userId)
    {
        var isLocked = CacheManager.GetUserLockSendStatus(userId);
        if (isLocked) return;
        CacheManager.SetUserLockSendStatus(userId, true);
        if (CacheManager.HasUserStatusChanged(userId) && (CacheManager.GetUserSecurityActivate(userId)))
        {
            SoundAlertAsync(userId, CacheManager.GetAlertLevel(userId));
        }

        var changedZones = CacheManager.GetChangedZones(userId);
        foreach (var zone in changedZones)
        {
            await _userWebSocketManager.SendMessage($"{zone.ZoneId},{(int)IndexManager.GetZoneStatus(zone.ZoneId, userId)}", SocketMessageType.ZNo, userId);
        }
        var changedSensors = CacheManager.GetChangedSensors(userId);
        foreach (var changedSensor in changedSensors)
        {
            await _userWebSocketManager.SendMessage($"{changedSensor.SensorId},{(int)IndexManager.GetSensorStatus(changedSensor.SensorId, changedSensor.ZoneId, userId)},{_sensorDetailService.GetSensPercent(CacheManager.GetSensorsLastValue(changedSensor.SensorId), changedSensor.IsDigital, changedSensor.NeutralValue)}", SocketMessageType.SNo, userId);
        }
        CacheManager.SetUserLockSendStatus(userId, false);
    }


    private async Task SoundAlertHomeAsync(string userId, AlertLevel level)
    {
        await _homeUdpSocketManager.SoundAlertAsync(userId, level);
    }

    private async Task SoundAlertMap(string userId, AlertLevel level)
    {
        await _mapManager.SoundAlert(userId, level);
    }
    /// <summary>
    /// sound alert for client user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    private async Task SoundAlertUserAsync(string userId, AlertLevel level)
    {
        if (!await SoundAlertUserSocketAsync(userId, level) || level == AlertLevel.High)
        {
            await SoundAlertUserSmsAsync(userId, level);
        }
    }
    /// <summary>
    /// sound alert for client user with socket
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    private async Task<bool> SoundAlertUserSocketAsync(string userId, AlertLevel level)
    {
        return await _userSocketManager.SoundAlertAsync(userId, level);
    }

    /// <summary>
    /// sound alert for client user with sms
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    private async Task SoundAlertUserSmsAsync(string userId, AlertLevel level)
        => await _userSmsManager.SoundAlertAsync(userId, level);



    #endregion
    #region Ctor


    public SecurityManager(ISensorDetailService sensorDetailService, IUserSocketManager userSocketManager, IUserSmsManager userSmsManager, IUserWebSocketManager userWebSocketManager, IHomeUdpSocketManager homeUdpSocketManager, IMapManager mapManager)
    {
        _sensorDetailService = sensorDetailService;
        _userSocketManager = userSocketManager;
        _userSmsManager = userSmsManager;
        _userWebSocketManager = userWebSocketManager;
        _homeUdpSocketManager = homeUdpSocketManager;
        _mapManager = mapManager;
    }


    #endregion
}