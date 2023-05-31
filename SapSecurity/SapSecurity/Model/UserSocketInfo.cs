using System.Net.Sockets;

namespace SapSecurity.Model;

public class UserSocketInfo : UserSocketInfo<Socket>
{
    public UserSocketInfo(Socket handler, string userId, Guid socketId) : base(handler, userId, socketId)
    {
    }
}

public class UserSocketInfo<T>
{

    public UserSocketInfo(T handler, string userId, Guid socketId)
    {
        Handler = handler;
        SocketId = socketId;
        UserId = userId;
    }

    /// <summary>
    /// connection info
    /// </summary>
    public T Handler { get; set; }

    public string UserId { get; set; }
    public Guid SocketId { get; set; }
}