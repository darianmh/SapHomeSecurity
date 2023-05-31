using System.Net.Sockets;
using SapSecurity.Infrastructure;

namespace SapSecurity.Model;

public class SensorSocketInfo
{
    public SensorSocketInfo(Socket handler, int sensorId, Guid socketId, string userId)
    {
        Handler = handler;
        SocketId = socketId;
        UserId = userId;
        SensorId = sensorId;
    }

    /// <summary>
    /// connection info
    /// </summary>
    public Socket Handler { get; set; }

    public int SensorId { get; set; }
    public Guid SocketId { get; set; }
    public string UserId { get; set; }
}