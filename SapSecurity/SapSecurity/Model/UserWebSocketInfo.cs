using System.Net.WebSockets;

namespace SapSecurity.Model;

public class UserWebSocketInfo : UserSocketInfo<WebSocket>
{
    public UserWebSocketInfo(WebSocket handler, string userId, Guid socketId) : base(handler, userId, socketId)
    {
    }
}