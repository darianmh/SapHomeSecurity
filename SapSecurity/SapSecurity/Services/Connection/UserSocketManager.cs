using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using SapSecurity.Infrastructure;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Notification;
using SapSecurity.Services.SocketManager;

namespace SapSecurity.Services.Connection;
///<inheritdoc cref="IUserSocketManager"/>
public class UserSocketManager : ConnectionManager, IUserSocketManager
{

    #region Fields

    private readonly int _port = SecurityConfig.UserSocket;
    private readonly ISocketManager _socketManager;
    private readonly ILogger<UserSocketManager> _logger;


    #endregion
    #region Methods
    public override bool SetupConnectionAsync()
    {

        return _socketManager.OpenSocket(_port, CallBack, DisconnectCallBack);
    }

    public bool DisconnectCallBack(Guid socketId)
    {
        try
        {
            if (UserSocketHandle.UserSocketInfos.Any(x => x.SocketId == socketId))
            {
                var info = UserSocketHandle.UserSocketInfos.First(x => x.SocketId == socketId);
                UserSocketHandle.UserSocketInfos.Remove(info);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
        return true;
    }
    public async Task CallBack(Socket socket, string message, Guid socketId)
    {
        try
        {
            if (message.StartsWith($"<{SocketMessageType.UId}>"))
            {
                var id = _socketManager.ReadMessage(message, SocketMessageType.UId.ToString());
                Console.WriteLine(id);
                SetUserInfo(id, socket, socketId);
            }
            else
            {
                //ignore
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
    }

    #endregion
    #region Utilities

    private void SetUserInfo(string id, Socket socket, Guid socketId)
    {
        var info = new UserSocketInfo(socket, id, socketId);
        UserSocketHandle.UserSocketInfos.Add(info);
    }

    #endregion
    #region Ctor


    public UserSocketManager(UserSocketNotificationManager notificationManager, ISocketManager socketManager, ILogger<UserSocketManager> logger) : base(notificationManager)
    {
        _socketManager = socketManager;
        _logger = logger;
    }

    #endregion


}