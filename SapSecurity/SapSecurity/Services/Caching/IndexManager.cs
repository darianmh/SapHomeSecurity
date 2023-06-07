using SapSecurity.Infrastructure;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Caching
{
    public static class IndexManager
    {
        public static readonly List<SensorIndexModel> Index = new();
        private static readonly Dictionary<int, DateTime> LastActiveTime = new();

        public static void SetIndex(SensorInfoModel sensor, int indexValue, int? sensValue)
        {
            if (Index.Any(x => x.SensorId == sensor.SensorId))
            {
                foreach (var model in Index.Where(x => x.SensorId == sensor.SensorId))
                {
                    if (indexValue == sensor.NeutralValue)
                    {
                        var userStatus = CacheManager.GetAlertLevel(sensor.UserId);
                        if (userStatus == AlertLevel.High) return;
                    }

                    if (model.SensorValue != sensValue)
                    {
                        CacheManager.SetChangedSensor(sensor.UserId, sensor.SensorId);
                        CacheManager.SetChangedZone(sensor.UserId, sensor.ZoneId);
                    }
                    model.SensorValue = sensValue;
                    model.IndexValue = indexValue;
                    model.CreateDate = DateTime.Now;
                }
            }
            else
            {
                Index.Add(new SensorIndexModel(sensor.SensorId, sensor.ZoneId, sensor.GroupId, sensor.UserId, indexValue, DateTime.Now, sensValue, sensor.WeightPercent));
                CacheManager.SetChangedSensor(sensor.UserId, sensor.SensorId);
                CacheManager.SetChangedZone(sensor.UserId, sensor.ZoneId);
            }
        }

        public static int GetSensorIndex(int sensorId)
        {
            return Index.FirstOrDefault(x => x.SensorId == sensorId)?.IndexValue ?? 0;
        }
        public static int? GetSensorValue(int sensorId)
        {
            return Index.FirstOrDefault(x => x.SensorId == sensorId)?.SensorValue;
        }

        public static int GetUserIndexValue(string userId)
        {
            var userIndexes = Index.Where(x => x.UserId == userId).ToList();

            var sum = 0;
            foreach (var user in userIndexes)
            {
                if (user.WeightPercent == null)
                    sum += user.IndexValue;
                else if (user.WeightPercent == 100)
                {
                    sum += user.IndexValue;
                }
                else
                {
                    var now = DateTime.Now;
                    var defSeconds = (now - user.CreateDate).Seconds;
                    var tempIndex = (user.IndexValue * (int)user.WeightPercent / 100) * defSeconds;
                    if (tempIndex > 100) tempIndex = 100;
                    sum += tempIndex;
                }
            }

            return sum;
        }

        public static int GetZoneIndexValue(int zoneId)
        {
            //var deleteDate = DateTime.Now.AddSeconds(SecurityConfig.LastLogSeconds * -1);
            var zoneIndexes = Index.Where(x => x.ZoneId == zoneId /*&& x.CreateDate >= deleteDate*/).ToList();
            return zoneIndexes.Any() ? zoneIndexes.Sum(x => x.IndexValue) : 0;
        }

        public static SensorStatus GetUserHomeStatus(string userId)
        {
            var indexValue = GetUserIndexValue(userId);
            if (indexValue >= SecurityConfig.IndexDangerValue) return SensorStatus.Danger;
            if (indexValue >= SecurityConfig.IndexWarningValue) return SensorStatus.Warning;
            return SensorStatus.Active;
        }

        public static SensorStatus GetZoneStatus(int zoneId, string userId)
        {
            var homeStatus = GetUserHomeStatus(userId);
            var zoneIndex = GetZoneIndexValue(zoneId);
            if (zoneIndex == 0) return SensorStatus.Active;
            if (homeStatus == SensorStatus.Danger) return SensorStatus.Danger;
            return SensorStatus.Warning;
        }

        public static SensorStatus GetSensorStatus(int sensorId, int zoneId, string userId)
        {
            var zoneStatus = GetZoneStatus(zoneId, userId);
            var sensorIndex = GetSensorIndex(sensorId);
            if (sensorIndex == 0) return SensorStatus.Active;
            if (zoneStatus == SensorStatus.Danger) return SensorStatus.Danger;
            return SensorStatus.Warning;
        }
        public static void ResetUserHomeSecurityIndex(string userId)
        {
            foreach (var model in Index.Where(x => x.UserId == userId))
            {
                model.IndexValue = 0;
                model.CreateDate = DateTime.Now;
            }
        }

        public static void SetAliveMessage(int sensor)
        {
            if (LastActiveTime.ContainsKey(sensor))
            {
                LastActiveTime[sensor] = DateTime.Now;
            }
            else
            {
                LastActiveTime.Add(sensor, DateTime.Now);
            }
        }

        public static DateTime? GetAliveDate(int sensorId)
        {
            LastActiveTime.TryGetValue(sensorId, out var date);
            return date;
        }
    }
}
