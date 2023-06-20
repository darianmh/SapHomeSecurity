using System.Collections.Concurrent;
using SapSecurity.Infrastructure;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Caching
{
    public static class IndexManager
    {
        public static readonly BlockingCollection<SensorIndexModel> Index = new();
        private static readonly ConcurrentDictionary<int, DateTime> LastActiveTime = new();

        public static void SetIndex(SensorInfoModel sensor, int indexValue, int? sensValue)
        {
            var userStatus = CacheManager.GetAlertLevel(sensor.UserId);
            if (Index.Any(x => x.SensorId == sensor.SensorId))
            {
                foreach (var model in Index.Where(x => x.SensorId == sensor.SensorId))
                {
                    if (sensValue == sensor.NeutralValue)
                    {
                        if (userStatus == AlertLevel.High) return;

                        //اگر در حالت خطر نباشد می تواند خنثی شود و زمان ست می شود
                        model.CreateDate = DateTime.Now;
                    }

                    if (model.IndexValue != indexValue)
                    {
                        model.CreateDate = DateTime.Now;
                    }

                    if (model.SensorValue != sensValue)
                    {
                        CacheManager.SetChangedSensor(sensor.UserId, sensor.SensorId);
                        CacheManager.SetChangedZone(sensor.UserId, sensor.ZoneId);
                    }
                    model.SensorValue = sensValue;
                    model.IndexValue = indexValue;

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
            var sensor = Index.FirstOrDefault(x => x.SensorId == sensorId);
            return sensor != null ? GetSensorIndex(sensor) : 0;
        }
        /// <summary>
        /// دریافت مقدار شاخص هر سنسور به صورت تکی
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public static int GetSensorIndex(SensorIndexModel sensor)
        {
            var index = sensor.IndexValue;
            //return sensor.IndexValue;
            if (sensor.WeightPercent == null)
                return index;
            else if (sensor.WeightPercent == 100)
            {
                return index;
            }
            else
            {
                var now = DateTime.Now;
                var defSeconds = (now - sensor.CreateDate).Seconds;
                var tempIndex = (index * (int)sensor.WeightPercent / 100) * defSeconds;
                if (tempIndex > 100) tempIndex = 100;
                return tempIndex;
            }
        }
        public static int? GetSensorValue(int sensorId)
        {
            return Index.FirstOrDefault(x => x.SensorId == sensorId)?.SensorValue ?? 0;
        }

        public static int GetUserIndexValue(string userId)
        {
            var userIndexes = Index.Where(x => x.UserId == userId).ToList();

            var sum = 0;
            foreach (var user in userIndexes)
            {
                sum += GetSensorIndex(user);
            }

            return sum;
        }

        public static int GetZoneIndexValue(int zoneId)
        {
            var zoneIndexes = Index.Where(x => x.ZoneId == zoneId).ToList();
            return zoneIndexes.Any() ? zoneIndexes.Sum(GetSensorIndex) : 0;
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
                LastActiveTime.TryAdd(sensor, DateTime.Now);
            }
        }

        public static DateTime? GetAliveDate(int sensorId)
        {
            LastActiveTime.TryGetValue(sensorId, out var date);
            return date;
        }
    }
}
