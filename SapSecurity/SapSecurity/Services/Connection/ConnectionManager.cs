using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Notification;

namespace SapSecurity.Services.Connection;

/// <inheritdoc cref="IConnectionManager" />
public abstract class ConnectionManager : IConnectionManager
{
    #region Fields

    public INotificationManager NotificationManager { get; }


    #endregion
    #region Methods
    public abstract bool SetupConnectionAsync();
    public async Task<bool> SoundAlertAsync(string userId, AlertLevel level)
        => await NotificationManager.SoundAlert(userId, level);

    #endregion
    #region Utilities



    #endregion
    #region Ctor


    public ConnectionManager(INotificationManager notificationManager)
    {
        NotificationManager = notificationManager;
    }


    #endregion

}