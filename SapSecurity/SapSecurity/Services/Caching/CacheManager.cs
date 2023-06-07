using System.Collections.Concurrent;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

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

    public static ConcurrentDictionary<int, int> ZoneSensorsSendCount = new ConcurrentDictionary<int, int>();
    public static ConcurrentDictionary<string, List<int>> ChangedZones
    {
        get
        {
            return _changedZones ??= new ConcurrentDictionary<string, List<int>>();
        }
    }

    private static ConcurrentDictionary<string, List<int>>? _changedZones;


    private static ConcurrentDictionary<string, bool> _userLockSendStatus = new();




    public static ConcurrentDictionary<string, List<int>> ChangedSensors { get; } = new();


    private static ConcurrentDictionary<string, bool> _userSecurityIsActive = new ConcurrentDictionary<string, bool>();

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




    private static BlockingCollection<SensorInfoModel>? _sensorInfos;
    public static BlockingCollection<SensorInfoModel> SensorInfos
    {
        get => _sensorInfos ??= new BlockingCollection<SensorInfoModel>();
        set => _sensorInfos = value;
    }


    private static readonly ConcurrentDictionary<int, SpecialMessageModel> SensorSpecialMessages = new();

    #endregion
    #region Methods

    public static void ResetZoneSensorValue(string userId)
    {
        var zones = SensorInfos.Where(x => x.UserId == userId).Select(x => x.ZoneId).ToList();
        zones.ForEach(x => ResetZoneSensorValue(x));
    }
    public static void ResetZoneSensorValue(int zoneId)
    {
        if (ZoneSensorsSendCount.ContainsKey(zoneId))
        {
            ZoneSensorsSendCount[zoneId] = 0;
        }
        else
        {
            ZoneSensorsSendCount.TryAdd(zoneId, 0);
        }
    }
    public static int GetZoneSensorValue(int zoneId)
    {
        if (ZoneSensorsSendCount.ContainsKey(zoneId))
        {
            return ZoneSensorsSendCount[zoneId];
        }
        else
        {
            ZoneSensorsSendCount.TryAdd(zoneId, 1);
            return 1;
        }
    }
    public static void SetZoneSensorValue(int zoneId)
    {
        if (ZoneSensorsSendCount.ContainsKey(zoneId))
        {
            ZoneSensorsSendCount[zoneId]++;
        }
        else
        {
            ZoneSensorsSendCount.TryAdd(zoneId, 1);
        }
    }
    public static void SetUserLockSendStatus(string userId, bool isLock)
    {
        if (_userLockSendStatus.ContainsKey(userId))
        {
            _userLockSendStatus[userId] = isLock;
        }
        else
        {
            _userLockSendStatus.TryAdd(userId, isLock);
        }
    }
    public static bool GetUserLockSendStatus(string userId)
    {
        if (_userLockSendStatus.TryGetValue(userId, out var isLock))
            return isLock;
        return false;
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




    public static void ResetSpecialMessages(string userId)
    {
        var sensors = SensorInfos.Where(x => x.UserId == userId).ToList();
        if (sensors != null) sensors.ForEach(x => DeleteSpecialMessage(x.SensorId));
    }

    public static void DeleteSpecialMessage(int sensorId)
    {
        SensorSpecialMessages.TryRemove(sensorId, out _);
    }
    public static int? ReadSpecialMessage(int sensorId)
    {
        if (SensorSpecialMessages.TryGetValue(sensorId, out var value))
        {
            if (value.IsOneTime) DeleteSpecialMessage(sensorId);
            return value.Message;
        }
        return null;
    }

    public static void SetSpecialMessage(string sensorIdentifier, int message, bool isOneTime)
    {
        var sensorId = SensorInfos.FirstOrDefault(x => x.SensorIdentifier == sensorIdentifier)?.SensorId;
        if (sensorId == null) return;
        SetSpecialMessage(sensorId.Value, message, isOneTime);
    }
    public static void SetSpecialMessage(int sensorId, int message, bool isOneTime)
    {
        if (SensorSpecialMessages.ContainsKey(sensorId))
            SensorSpecialMessages[sensorId] = new SpecialMessageModel()
            {
                IsOneTime = isOneTime,
                Message = message
            };
        else SensorSpecialMessages.TryAdd(sensorId, new SpecialMessageModel()
        {
            IsOneTime = isOneTime,
            Message = message
        });
    }


    private static readonly ConcurrentDictionary<int, int> SensorsLastMessage = new();

    public static int? GetSensorLastMessage(string sensorIdentifier)
    {
        var sensorId = SensorInfos.FirstOrDefault(x => x.SensorIdentifier == sensorIdentifier)?.SensorId;
        if (sensorId == null) return null;
        return GetSensorLastMessage((int)sensorId);
    }
    public static int? GetSensorLastMessage(int sensorId)
    {
        if (SensorsLastMessage.TryGetValue(sensorId, out var message))
        {
            return message;
        }

        return null;
    }
    public static void SetSensorLastMessage(int sensorId, int message)
    {
        if (SensorsLastMessage.ContainsKey(sensorId))
        {
            SensorsLastMessage[sensorId] = message;
        }
        else
        {
            SensorsLastMessage.TryAdd(sensorId, message);
        }
    }


    public static bool GetUserSecurityActivate(string userId)
    {
        _userSecurityIsActive.TryGetValue(userId, out var isActive);
        return isActive;
    }
    public static void SetUserSecurityActivate(string userId, bool isActive)
    {
        if (_userSecurityIsActive.ContainsKey(userId))
        {
            _userSecurityIsActive[userId] = isActive;
        }
        else
        {
            _userSecurityIsActive.TryAdd(userId, isActive);
        }
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

    public static List<SensorInfoModel> GetChangedSensors(string userId)
    {
        if (ChangedSensors.TryGetValue(userId, out var changes))
        {
            ChangedSensors.Remove(userId, out _);
            return SensorInfos.Where(z => changes.Any(c => c == z.SensorId)).ToList();
        }

        return new List<SensorInfoModel>();
    }
    public static List<SensorInfoModel> GetChangedZones(string userId)
    {
        if (ChangedZones.TryGetValue(userId, out var changes))
        {
            ChangedZones.Remove(userId, out _);
            return SensorInfos.Where(z => changes.Any(c => c == z.ZoneId)).ToList();
        }
        return new List<SensorInfoModel>();
    }


    /// <summary>
    /// clear logs after user security status changed
    /// </summary>
    /// <param name="userId"></param>
    public static void ClearAllLogs(string userId)
    {
        IndexManager.ResetUserHomeSecurityIndex(userId);
    }
    public static int? GetSensorsLastValue(int sensorDetailId)
    {
        return IndexManager.GetSensorValue(sensorDetailId);
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor



    #endregion


}