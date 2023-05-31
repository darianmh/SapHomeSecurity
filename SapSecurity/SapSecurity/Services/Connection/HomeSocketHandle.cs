using SapSecurity.Model;

namespace SapSecurity.Services.Connection;

public static class HomeSocketHandle
{
    private static List<SensorSocketInfo>? _sensorSocketInfos;

    public static List<SensorSocketInfo> SensorSocketInfos => _sensorSocketInfos ??= new List<SensorSocketInfo>();
    /// <summary>
    /// list of sensors and their identifiers
    /// </summary>
    private static Dictionary<string, int>? _sensorInfos;

    /// <summary>
    /// list of sensors and their identifiers
    /// </summary>
    public static Dictionary<string, int> SensorInfos => _sensorInfos ??= new Dictionary<string, int>();

    public static int? GetSensorId(string sensorIdentifier)
    {
        var check = SensorInfos.TryGetValue(sensorIdentifier, out int sensorId);
        if (check) return sensorId;
        return null;
    }
}