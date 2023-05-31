
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Connection;
using SapSecurity.Services.Db;

namespace SapSecurity.Services.Security;

public class SecurityManager : ISecurityManager
{

    #region Fields

    private readonly ISensorDetailService _sensorDetailService;
    private readonly IUserSocketManager _userSocketManager;
    private readonly IUserSmsManager _userSmsManager;
    private readonly IUserWebSocketManager _userWebSocketManager;
    private readonly IApplicationUserService _applicationUserService;
    private readonly IHomeUdpSocketManager _homeUdpSocketManager;
    private readonly IMapManager _mapManager;

    #endregion
    #region Methods

    public async Task MeasureDangerPossibility(string userId)
    {
        var user = await _applicationUserService.GetByIdAsync(userId);
        if (user == null)
        {
            return;
        }
        if (user.SecurityIsActive != true)
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


    public async Task SensReceiver(SensorDetail sensor, double sensValue)
    {
        CacheManager.SetSensorsLastValue(sensor.Id, sensor.ZoneId, sensor.UserId, sensValue);
        if (!Equals(sensValue, sensor.SensorGroup.NeutralValue))
        {
            var weight = sensor.Weight ?? sensor.SensorGroup.Weight;
            var index = weight;
            if (!sensor.SensorGroup.IsDigital)
            {
                if (sensor.SensorGroup.NeutralValue != 0)
                    index = Convert.ToInt32((sensor.SensorGroup.NeutralValue - sensValue) / sensor.SensorGroup.NeutralValue * 100);
            }
            else
            {
                //spray
                if (sensor.Id == 4 && (await _applicationUserService.GetSecurityStatus(sensor.UserId)))
                {
                    var lastSpray = CacheManager.GetLastSpray(sensor.UserId);
                    if (lastSpray == null || lastSpray.Value.AddSeconds(30) < DateTime.Now)
                    {
                        CacheManager.SetLastSpray(sensor.UserId);
                        CacheManager.SetSpecialMessage(5, 0);
                    }
                    //else if (lastSpray.Value.AddSeconds(2) >= DateTime.Now)
                    //{
                    //    CacheManager.SetLastSpray(sensor.UserId);
                    //    CacheManager.SetSpecialMessage(5, 1);
                    //}
                }

                if (sensor.Identifier == "202")
                {
                    var lastDoorLock = CacheManager.GetLastDoorLock(sensor.UserId);
                    if (lastDoorLock != null)
                    {
                        if (await _applicationUserService.GetSecurityStatus(sensor.UserId))
                        {
                            if (lastDoorLock.Value.AddSeconds(3) < DateTime.Now)
                            {
                                CacheManager.SetSpecialMessage("202", 1);
                            }
                            else
                            {
                                CacheManager.SetSpecialMessage("202", 2);
                            }
                        }
                        else
                        {
                            if (lastDoorLock.Value.AddSeconds(3) < DateTime.Now)
                            {
                                CacheManager.SetSpecialMessage("202", 0);
                            }
                            else
                            {
                                CacheManager.SetSpecialMessage("202", 2);
                            }
                        }
                    }
                }
            }
            IndexManager.SetIndex(sensor, index);
        }
        else
        {
            IndexManager.SetIndex(sensor, 0);
        }
        await MeasureDangerPossibility(sensor.UserId);
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
            Console.WriteLine("add task");
            while (CacheManager.UserSecurityCheck.TryGetValue(user, out var check) && check)
            {
                if (c < 6)
                {
                    Console.WriteLine("check wait");
                    await Task.Delay(10000);
                    c++;
                    continue;
                }

                c = 0;
                Console.WriteLine("check wait done");
                //find de active sensors
                var deActiveSensors = await _sensorDetailService.GetDeActiveSensors(userId: user);
                deActiveSensors.ForEach(x => IndexManager.SetIndex(x, x.Weight ?? x.SensorGroup.Weight));
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

    private async Task SendStatus(string userId)
    {
        if (CacheManager.HasUserStatusChanged(userId)&&(await _applicationUserService.GetSecurityStatus(userId)))
        {
            SoundAlertAsync(userId, CacheManager.GetAlertLevel(userId));
        }

        var changedZones = CacheManager.GetChangedZones(userId);
        foreach (var zone in changedZones)
        {
            await _userWebSocketManager.SendMessage($"{zone.Id},{(int)IndexManager.GetZoneStatus(zone.Id, userId)}", SocketMessageType.ZNo, userId);
        }
        var changedSensors = CacheManager.GetChangedSensors(userId);
        foreach (var changedSensor in changedSensors)
        {
            await _userWebSocketManager.SendMessage($"{changedSensor.Id},{(int)IndexManager.GetSensorStatus(changedSensor.Id, changedSensor.ZoneId, userId)},{_sensorDetailService.GetSensPercent(changedSensor, CacheManager.GetSensorsLastValue(changedSensor.Id))}", SocketMessageType.SNo, userId);
        }
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


    public SecurityManager(ISensorDetailService sensorDetailService, IUserSocketManager userSocketManager, IUserSmsManager userSmsManager, IUserWebSocketManager userWebSocketManager, IApplicationUserService applicationUserService, IHomeUdpSocketManager homeUdpSocketManager, IMapManager mapManager)
    {
        _sensorDetailService = sensorDetailService;
        _userSocketManager = userSocketManager;
        _userSmsManager = userSmsManager;
        _userWebSocketManager = userWebSocketManager;
        _applicationUserService = applicationUserService;
        _homeUdpSocketManager = homeUdpSocketManager;
        _mapManager = mapManager;
    }


    #endregion
}