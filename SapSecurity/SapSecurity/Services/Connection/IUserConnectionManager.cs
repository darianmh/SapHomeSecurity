using SapSecurity.Model.Types;

namespace SapSecurity.Services.Connection;

/// <summary>
/// send sensor info to user connection
/// </summary>
public interface IUserConnectionManager : IConnectionManager
{
    /// <summary>
    /// send message to user every channel that user is registered
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageType"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task SendMessage(string message, SocketMessageType messageType, string userId);

}