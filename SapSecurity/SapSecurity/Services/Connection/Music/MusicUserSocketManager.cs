using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using SapSecurity.Infrastructure;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Notification;
using SapSecurity.Services.SocketManager;

namespace SapSecurity.Services.Connection.Music;
///<inheritdoc cref="IMusicUserSocketManager"/>
public class MusicUserSocketManager : ConnectionManager, IMusicUserSocketManager
{

    #region Fields

    private readonly int _port = 7086;
    private readonly ISocketManager _socketManager;
    private readonly ILogger<UserSocketManager> _logger;


    #endregion
    #region Methods
    public override bool SetupConnectionAsync()
    {

        return _socketManager.OpenSocket(_port, CallBack, DisconnectCallBack);
    }

    public async Task SendMessage(int message)
    {
        try
        {
            if (message == UserSocketHandle.LastMusic && UserSocketHandle.LastMusicDate.AddSeconds(30) < DateTime.Now) return;
            UserSocketHandle.LastMusic = message;
            var all = UserSocketHandle.UserMusicSocketInfos;
            var toRemove = new List<UserSocketInfo>();
            foreach (var info in all)
            {
                try
                {

                    await info.Handler.SendAsync(Encoding.UTF8.GetBytes(message.ToString()), SocketFlags.None);
                }
                catch (Exception e)
                {
                    toRemove.Add(info);
                }
            }
            toRemove.ForEach(x => UserSocketHandle.UserMusicSocketInfos.Remove(x));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
    }

    public bool DisconnectCallBack(Guid socketId)
    {
        try
        {
            if (UserSocketHandle.UserSocketInfos.Any(x => x.SocketId == socketId))
            {
                var info = UserSocketHandle.UserMusicSocketInfos.First(x => x.SocketId == socketId);
                UserSocketHandle.UserMusicSocketInfos.Remove(info);
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
            Console.WriteLine("connected for music");
            if (message.StartsWith($"<{SocketMessageType.UId}>"))
            {
                SetUserInfo(socket, socketId);
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

    private void SetUserInfo(Socket socket, Guid socketId)
    {
        var info = new UserSocketInfo(socket, "1", socketId);
        UserSocketHandle.UserMusicSocketInfos.Add(info);
    }

    #endregion
    #region Ctor


    public MusicUserSocketManager(UserSocketNotificationManager notificationManager, ISocketManager socketManager, ILogger<UserSocketManager> logger) : base(notificationManager)
    {
        _socketManager = socketManager;
        _logger = logger;
    }

    #endregion


}