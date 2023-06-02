using SapSecurity.Model;

namespace SapSecurity.Services.Connection;

public static class HomeSocketHandle
{
    private static List<SensorSocketInfo>? _sensorSocketInfos;

    public static List<SensorSocketInfo> SensorSocketInfos => _sensorSocketInfos ??= new List<SensorSocketInfo>();
}