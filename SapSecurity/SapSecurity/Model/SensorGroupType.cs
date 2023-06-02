namespace SapSecurity.Model;

public enum SensorGroupType
{
    Sensor = 1,
    ZoneReceiver = 2,
    HomeReceiver = 3,
    /// <summary>
    /// depends on home security is active or de active
    /// </summary>
    HomeSecurityDepend= 4,
    CriticalHomeReceiver = 5,
}