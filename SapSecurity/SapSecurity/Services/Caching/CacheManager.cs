using System.Collections.Concurrent;
using SapSecurity.Infrastructure;
using SapSecurity.Model;
using SapSecurity.Model.Types;

namespace SapSecurity.Services.Caching;

public static class CacheManager
{

    #region Fields

    private static ConcurrentDictionary<string, bool>? _userSecurityCheck;
    public static ConcurrentDictionary<string, bool> UserSecurityCheck
    {
        get
        {
            return _userSecurityCheck ??= new ConcurrentDictionary<string, bool>();
        }
    }

    public static ConcurrentDictionary<string, List<int>> ChangedZones
    {
        get
        {
            return _changedZones ??= new ConcurrentDictionary<string, List<int>>();
        }
    }

    private static ConcurrentDictionary<string, List<int>>? _changedZones;







    public static ConcurrentDictionary<string, List<int>> ChangedSensors { get; } = new();

    public static ConcurrentDictionary<string, SensorStatus> UserSecurityStatus
    {
        get
        {
      return _userSecurityStatus ??= new ConcurrentDictionary<string, SensorStatus>();
        }
    }

    private static ConcurrentDictionary<string, SensorStatus>? _userSecurityStatus;

    public static ConcurrentDictionary<string, bool> UserSecurityStatusChanged
    {
        get
        {
      return _userSecurityStatusChanged ??= new ConcurrentDictionary<string, bool>();
        }
    }

    private static ConcurrentDictionary<string, bool>? _userSecurityStatusChanged;


    public static ConcurrentDictionary<int, double> SensorsLastValue
    {
        get
        {
      return _sensorsLastValue ??= new ConcurrentDictionary<int, double>();
        }
    }

    private static ConcurrentDictionary<int, double>? _sensorsLastValue;


    private static BlockingCollection<ApplicationUser>? _users;
    public static BlockingCollection<ApplicationUser> Users
    {
        get
        {
       return _users ??= new BlockingCollection<ApplicationUser>();
        }
        set
        {
            _users = value;
        }
    }


    private static List<SensorDetail>? _sensorDetails;
    public static List<SensorDetail> SensorDetails
    {
        get
        {
         return _sensorDetails ??= new List<SensorDetail>();
        }
    }




    private static readonly ConcurrentDictionary<int, int> SensorSpecialMessages = new();
    private static readonly Dictionary<string, DateTime> LastSpray = new();
    private static readonly Dictionary<string, DateTime> LastDoorMessage = new();

    #endregion
    #region Methods

    public static void SetDoorLock(string userId)
    {
        if (LastDoorMessage.ContainsKey(userId))
        {
            LastDoorMessage[userId] = DateTime.Now;
        }
        else
        {
            LastDoorMessage.Add(userId, DateTime.Now);
        }
    }
    public static DateTime? GetLastDoorLock(string userId)
    {
        LastDoorMessage.TryGetValue(userId, out var dateTime);
        return dateTime;
    }
    public static void SetLastSpray(string userId)
    {
        if (LastSpray.ContainsKey(userId))
        {
            LastSpray[userId] = DateTime.Now;
        }
        else
        {
            LastSpray.Add(userId, DateTime.Now);
        }
    }
    public static DateTime? GetLastSpray(string userId)
    {
        LastSpray.TryGetValue(userId, out var dateTime);
        return dateTime;
    }

    public static void SetSensorsLastValue(int sensorId, int zoneId, string userId, double sensValue)
    {
        if (SensorsLastValue.ContainsKey(sensorId))
        {
            SensorsLastValue[sensorId] = sensValue;
        }
        else
        {
            SetChangedZone(userId, zoneId);
            SetChangedSensor(userId, sensorId);
            SensorsLastValue.TryAdd(sensorId, sensValue);
        }
    }

    public static void SetChangedSensor(string userId, int sensorId)
    {
        if (ChangedSensors.ContainsKey(userId))
        {
            ChangedSensors[userId].Add(sensorId);
        }
        else
        {
            ChangedSensors.TryAdd(userId, new List<int>() { sensorId });
        }
    }
    public static void SetChangedZone(string userId, int zoneId)
    {
        if (ChangedZones.ContainsKey(userId))
        {
            ChangedZones[userId].Add(zoneId);
        }
        else
        {
            ChangedZones.TryAdd(userId, new List<int>() { zoneId });
        }
    }
    public static double? GetSensorsLastValue(int sensorId)
    {
        if (SensorsLastValue.TryGetValue(sensorId, out var lastValue)) return lastValue;
        return null;
    }
    public static void AddSensorDetail(SensorDetail sensorDetail)
    {
        if (SensorDetails.All(x => x.Id != sensorDetail.Id))
            SensorDetails.Add(sensorDetail);
    }

    public static bool SecurityStatus = false;
    public static void RemoveUser(ApplicationUser user)
    {
        var toRemove = Users.FirstOrDefault(x => x.Id == user.Id);
        if (toRemove != null) Users = new BlockingCollection<ApplicationUser>();
    }

    public static bool Alarm = false;
    /// <summary>
    /// status of user security
    /// </summary>
    /// <param name="sensor"></param>
    /// <returns></returns>
    public static int GetUserSecurityStatus(SensorDetail sensor)
    {

        var specialMessage = ReadSpecialMessage(sensor.Id);
        if (specialMessage != null) return specialMessage.Value;
        //door lock
        if (sensor.Identifier == "202") return 2;


        //spray
        if (sensor.Id == 5) return 1;



        var homeIndex = IndexManager.GetUserIndexValue(sensor.UserId);
        if (!SecurityStatus) return 0;

        var checkUser = true;
        if (sensor.SensorGroup.IsCritical)
        {
            var indexValue = IndexManager.GetSensorIndex(sensor.Id);

            if (indexValue != 0) checkUser = false;
            else homeIndex = indexValue;
        }
        if (checkUser)
        {
            homeIndex = sensor.SensorGroup.IsZoneRestricted
                ? IndexManager.GetZoneIndexValue(sensor.ZoneId)
                : IndexManager.GetUserIndexValue(sensor.UserId);
        }

        if (sensor.Id == 3)
        {
            return homeIndex >= SecurityConfig.IndexDangerValue ? 0 : 1;
        }

        Console.WriteLine(SecurityConfig.IndexDangerValue);
        return homeIndex >= SecurityConfig.IndexDangerValue ? 1 : 0;
    }

    private static int? ReadSpecialMessage(int sensorId)
    {
        if (SensorSpecialMessages.TryGetValue(sensorId, out var value))
        {
            SensorSpecialMessages.TryRemove(sensorId, out var item);
            return value;
        }
        return null;
    }

    public static void SetSpecialMessage(string sensorIdentifier, int message)
    {
        var sensorId = SensorDetails.FirstOrDefault(x => x.Identifier == sensorIdentifier)?.Id;
        if (sensorId == null) return;
        SetSpecialMessage(sensorId.Value, message);
    }
    public static void SetSpecialMessage(int sensorId, int message)
    {
        if (SensorSpecialMessages.ContainsKey(sensorId))
            SensorSpecialMessages[sensorId] = message;
        else SensorSpecialMessages.TryAdd(sensorId, message);
    }

    public static AlertLevel GetAlertLevel(string userId)
    {
        if (UserSecurityStatus.TryGetValue(userId, out var value))
        {
            switch (value)
            {
                case SensorStatus.Danger: return AlertLevel.High;
                case SensorStatus.Warning: return AlertLevel.Medium;
                default: return AlertLevel.None;
            }
        }

        return AlertLevel.None;
    }
    public static bool HasUserStatusChanged(string userId)
    {
        if (UserSecurityStatusChanged.TryGetValue(userId, out var changed))
        {
            UserSecurityStatusChanged[userId] = false;
            return changed;
        }
        return false;
    }
    /// <summary>
    /// set user security status
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="sensorStatus"></param>
    /// <param name="force">force to change value</param>
    public static void SetSecurityStatus(string userId, SensorStatus sensorStatus, bool force = false)
    {
        if (UserSecurityStatus.TryGetValue(userId, out var oldValue))
        {
            if (!force && (oldValue == SensorStatus.Danger || oldValue == SensorStatus.Warning)) return;
            if (oldValue != sensorStatus)
            {
                UserSecurityStatus[userId] = sensorStatus;
                UserSecurityStatusChanged[userId] = true;
            }
        }
        else
        {
            UserSecurityStatus.TryAdd(userId, sensorStatus);
            UserSecurityStatusChanged.TryAdd(userId, true);
        }
    }

    public static List<SensorDetail> GetChangedSensors(string userId)
    {
        if (ChangedSensors.TryGetValue(userId, out var changes))
        {
            ChangedSensors.Remove(userId, out var item);
            return SensorDetails.Where(z => changes.Any(c => c == z.Id)).ToList();
        }

        return new List<SensorDetail>();
    }
    public static List<Zone> GetChangedZones(string userId)
    {
        if (ChangedZones.TryGetValue(userId, out var changes))
        {
            ChangedZones.Remove(userId, out var item);
            return SensorDetails.Where(z => changes.Any(c => c == z.ZoneId)).Select(x => x.Zone).ToList();
        }
        return new List<Zone>();
    }

    public static bool GetStatusDoorLock()
    {
        return _statusDoorLock;
    }
    private static bool _statusDoorLock = false;
    public static void SetStatusDoorLock(bool statusDoorLock)
    {
        _statusDoorLock = statusDoorLock;
    }

    /// <summary>
    /// clear logs after user security status changed
    /// </summary>
    /// <param name="userId"></param>
    public static void ClearAllLogs(string userId)
    {
        IndexManager.ResetUserHomeSecurityIndex(userId);
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor



    #endregion


}