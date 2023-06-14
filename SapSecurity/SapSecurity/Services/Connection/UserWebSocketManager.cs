using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Db;
using SapSecurity.Services.Notification;
using SapSecurity.Services.SocketManager;
using SapSecurity.Infrastructure.Extensions;

namespace SapSecurity.Services.Connection;

///<inheritdoc cref="IUserWebSocketManager"/>
public class UserWebSocketManager : ConnectionManager, IUserWebSocketManager
{

    #region Fields

    private readonly ISocketManager _socketManager;
    private readonly ILogger<UserSocketManager> _logger;
    private readonly IApplicationUserService _applicationUserService;


    #endregion
    #region Methods
    public override bool SetupConnectionAsync()
    {
        return false;
    }

    public async Task SendMessage(string message, SocketMessageType messageType, string userId, bool justAdmin = true)
    {
        try
        {
            var toRemove = new List<UserWebSocketInfo>();
            if (!justAdmin)
            {
                foreach (var info in UserSocketHandle.UserWebSocketInfos.Where(x => x.UserId == userId))
                {
                    try
                    {
                        await info.Handler.SendAsync(
                            new ArraySegment<byte>(Encoding.ASCII.GetBytes(messageType.GetMessageInFormat(message))), WebSocketMessageType.Text, true,
                            CancellationToken.None);
                    }
                    catch (Exception e)
                    {
                        toRemove.Add(info);
                        _logger.LogError(e.Message, e);
                    }
                }
                toRemove.ForEach(x => UserSocketHandle.UserWebSocketInfos.Remove(x));
            }
            toRemove = new List<UserWebSocketInfo>();
            foreach (var info in UserSocketHandle.AdminWebSocketInfos.Where(x => x.UserId == userId))
            {
                try
                {
                    await info.Handler.SendAsync(
                        new ArraySegment<byte>(Encoding.ASCII.GetBytes(messageType.GetMessageInFormat(message))), WebSocketMessageType.Text, true,
                        CancellationToken.None);
                }
                catch (Exception e)
                {
                    toRemove.Add(info);
                    _logger.LogError(e.Message, e);
                }
            }
            toRemove.ForEach(x => UserSocketHandle.AdminWebSocketInfos.Remove(x));

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
    }

    public async Task SetupConnectionAsync(ControllerContext controllerContext, CancellationToken token)
    {
        var context = controllerContext.HttpContext;
        if (context.WebSockets.IsWebSocketRequest)
        {
            try
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var buffer = new byte[1024 * 4];
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                while (!result.CloseStatus.HasValue)
                {
                    var text = Encoding.ASCII.GetString(buffer);
                    await CallBack(webSocket, text, Guid.NewGuid());
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }

    public async Task CallBack(WebSocket socket, string message, Guid socketId)
    {
        try
        {
            message = message.Replace("\"", String.Empty);
            if (message.StartsWith($"<{SocketMessageType.UId}>"))
            {
                var token = _socketManager.ReadMessage(message, SocketMessageType.UId.ToString());
                if (token == null) return;
                var id = _applicationUserService.GetUserId(token);
                if (id == null) return;
                SetUserInfo(id, socket, socketId);
                ConsoleExtension.WriteAppInfo($"userId: {id} connected");
            }
            else if (message.StartsWith($"<{SocketMessageType.AId}>"))
            {
                var id = _socketManager.ReadMessage(message, SocketMessageType.AId.ToString());
                if (id == null) return;
                SetAdminInfo(id, socket, socketId);
                ConsoleExtension.WriteAppInfo($"userId: {id} connected");
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

    private void SetUserInfo(string id, WebSocket socket, Guid socketId)
    {
        var info = new UserWebSocketInfo(socket, id, socketId);
        UserSocketHandle.UserWebSocketInfos.Add(info);
    }
    private void SetAdminInfo(string id, WebSocket socket, Guid socketId)
    {
        var info = new UserWebSocketInfo(socket, id, socketId);
        UserSocketHandle.AdminWebSocketInfos.Add(info);
    }

    #endregion
    #region Ctor


    public UserWebSocketManager(UserSocketNotificationManager notificationManager, ISocketManager socketManager, ILogger<UserSocketManager> logger, IApplicationUserService applicationUserService) : base(notificationManager)
    {
        _socketManager = socketManager;
        _logger = logger;
        _applicationUserService = applicationUserService;
    }

    #endregion


}