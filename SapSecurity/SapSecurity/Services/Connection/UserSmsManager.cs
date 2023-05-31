using Microsoft.Extensions.Configuration;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Notification;

namespace SapSecurity.Services.Connection;

/// <inheritdoc cref="IUserSmsManager" />
public class UserSmsManager : ConnectionManager, IUserSmsManager
{

    #region Fields


    #endregion
    #region Methods


    public override bool SetupConnectionAsync()
    {
        return true;
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor


    public UserSmsManager(UserSmsNotificationManager notificationManager) : base(notificationManager)
    {

    }

    #endregion

}