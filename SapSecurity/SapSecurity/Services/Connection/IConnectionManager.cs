using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Notification;

namespace SapSecurity.Services.Connection;

/// <summary>
/// manage all types of connection
/// </summary>
public interface IConnectionManager
{
    /// <summary>
    /// notification sender
    /// </summary>
    INotificationManager NotificationManager { get; }
    /// <summary>
    /// send alert notification
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    Task<bool> SoundAlertAsync(string userId, AlertLevel level);

    /// <summary>
    /// setup connections
    /// </summary>
    /// <returns></returns>
    bool SetupConnectionAsync();
}