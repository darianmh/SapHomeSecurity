using SapSecurity.Model.Types;

namespace SapSecurity.Services.Notification;

/// <summary>
/// send push notification for user
/// </summary>
public class HttpPushNotificationManager : INotificationManager
{


    #region Fields



    #endregion
    #region Methods

    public async Task<bool> SoundAlert(string userId, AlertLevel alertLevel)
    {
        //todo fix here
        return true;
    }


    #endregion
    #region Utilities



    #endregion
    #region Ctor



    #endregion
}