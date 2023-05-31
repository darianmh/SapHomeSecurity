using SapSecurity.Model;
using SapSecurity.Model.Types;

namespace SapSecurity.Services.Notification;

/// <summary>
/// base model for sending notification to user
/// </summary>
public class NotificationManager : INotificationManager
{
    #region Fields



    #endregion
    #region Methods

    public async Task<bool> SoundAlert(string userId, AlertLevel alertLevel)
    {
        return true;
    }


    #endregion
    #region Utilities



    #endregion
    #region Ctor



    #endregion

}