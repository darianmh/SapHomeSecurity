using SapSecurity.Model;

namespace SapSecurity.Services.Connection;

public static class UserSocketHandle
{
    private static List<UserSocketInfo>? _userSocketInfos;

    public static List<UserSocketInfo> UserSocketInfos => _userSocketInfos ??= new List<UserSocketInfo>();
    private static List<UserWebSocketInfo>? _userWebSocketInfos;

    public static List<UserWebSocketInfo> UserWebSocketInfos => _userWebSocketInfos ??= new List<UserWebSocketInfo>();
}