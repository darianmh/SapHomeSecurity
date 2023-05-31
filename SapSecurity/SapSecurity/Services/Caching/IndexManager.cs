using SapSecurity.Infrastructure;
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Caching
{
    public static class IndexManager
    {
        public static readonly List<IndexModel> Index = new();
        private static readonly Dictionary<int, DateTime> LastActiveTime = new();

        public static void SetIndex(SensorDetail sensor, int indexValue)
        {
            var now = DateTime.Now;
            if (Index.Any(x => x.SensorId == sensor.Id))
            {
                foreach (var model in Index.Where(x => x.SensorId == sensor.Id))
                {
                    if (Math.Abs(indexValue - sensor.SensorGroup.NeutralValue!.Value) < 0.01)
                    {
                        var userStatus = CacheManager.GetAlertLevel(sensor.UserId);
                        if (userStatus == AlertLevel.High) return;
                    }

                    var statusChanged = false;
                    //if (indexValue == 0 && model.CreateDate.AddSeconds(SecurityConfig.LastLogSeconds) >= DateTime.Now) continue;
                    if (Math.Abs(indexValue - sensor.SensorGroup.NeutralValue!.Value) > 0.01)
                    {
                        model.IndexValue = indexValue;
                        CacheManager.SetChangedSensor(sensor.UserId, sensor.Id);
                        CacheManager.SetChangedZone(sensor.UserId, sensor.ZoneId);
                        statusChanged = true;
                    }
                    //todo wight sensor
                    //weight sensor , value not changed , sensor is not in neutral value
                    //do not change time
                    if (sensor.Id == 25 && !statusChanged && Math.Abs(indexValue - sensor.SensorGroup.NeutralValue!.Value) < 0.01)
                    {
                        if (model.CreateDate.AddSeconds(8) > now)
                        {
                            return;
                        }
                    }
                    model.CreateDate = DateTime.Now;
                }
            }
            else
            {
                Index.Add(new IndexModel(sensor.Id, sensor.ZoneId, sensor.SensorGroupId, sensor.UserId, indexValue, DateTime.Now));
                CacheManager.SetChangedSensor(sensor.UserId, sensor.Id);
                CacheManager.SetChangedZone(sensor.UserId, sensor.ZoneId);
            }
            // Console.WriteLine($"Sensor Value = {indexValue} All = {string.Join(" , ", Index.Select(x => $"{x.SensorId}: {x.IndexValue}"))}");
        }

        public static int GetSensorIndex(int sensorId)
        {
            //var deleteDate = DateTime.Now.AddSeconds(SecurityConfig.LastLogSeconds * -1);
            return Index.FirstOrDefault(x => x.SensorId == sensorId /*&& x.CreateDate >= deleteDate*/)?.IndexValue ?? 0;
        }

        public static int GetUserIndexValue(string userId)
        {
            //var deleteDate = DateTime.Now.AddSeconds(SecurityConfig.LastLogSeconds * -1);
            var userIndexes = Index.Where(x => x.UserId == userId /*&& x.CreateDate >= deleteDate*/).ToList();
            var dateNow = DateTime.Now;


            var sum = 0;
            foreach (var user in userIndexes)
            {
                //weight sensor
                //if > 5 seconds full index value
                //else half index value
                if (user.SensorId == 25)
                {
                    if (user.CreateDate.AddSeconds(8) <= dateNow)
                    {
                        sum += user.IndexValue;
                        Console.WriteLine($"weight {user.IndexValue}");
                    }
                    else
                    {
                        sum += user.IndexValue / 2;
                        Console.WriteLine($"weight {user.IndexValue / 2}");
                    }
                }
                else
                {
                    sum += user.IndexValue;
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
