using Microsoft.AspNetCore.Mvc;
using SapSecurity.Model.Types;

namespace SapSecurity.Services.Connection;

/// <summary>
/// web socket connection for user read data
/// </summary>
public interface IUserWebSocketManager : IConnectionManager
{
    /// <summary>
    /// send message to user over web socket
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageType"></param>
    /// <param name="userId"></param>
    /// <param name="justAdmin"></param>
    /// <returns></returns>
    Task SendMessage(string message, SocketMessageType messageType, string userId, bool justAdmin = true);

    Task SetupConnectionAsync(ControllerContext finalizeCallBack, CancellationToken token);
}