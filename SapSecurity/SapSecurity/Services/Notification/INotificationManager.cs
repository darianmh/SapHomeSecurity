using SapSecurity.Model;
using SapSecurity.Model.Types;

namespace SapSecurity.Services.Notification;

public interface INotificationManager
{
    /// <summary>
    /// send alert to user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="alertLevel"></param>
    /// <returns></returns>
    Task<bool> SoundAlert(string userId,AlertLevel alertLevel);
}