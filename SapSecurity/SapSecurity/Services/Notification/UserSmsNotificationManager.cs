using SapSecurity.Model.Types;
using SapSecurity.Services.Db;
using SapSecurity.Services.Sms;

namespace SapSecurity.Services.Notification;

/// <summary>
/// send sms to user
/// </summary>
public class UserSmsNotificationManager : INotificationManager
{

    #region Fields
    private readonly ISmsManager _smsManager;
    private readonly IApplicationUserService _applicationUserService;

    #endregion
    #region Methods


    public async Task<bool> SoundAlert(string userId, AlertLevel alertLevel)
    {
        var user = await _applicationUserService.GetByIdAsync(userId);
        if (user == null) return false;
        if (!user.LoginInfos.Any(x => x.PhoneNumber != null)) return false;
        var message = GetMessage(alertLevel);
        try
        {
            foreach (var phoneNumber in user.LoginInfos.Select(x => x.PhoneNumber))
            {
                await _smsManager.SendSms(phoneNumber, message);
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }

    private string GetMessage(AlertLevel alertLevel)
    {
        return $"دزدگیر به صدا در آمده. درجه اهمیت :{alertLevel.ToString()}";
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor


    public UserSmsNotificationManager(ISmsManager smsManager, IApplicationUserService applicationUserService)
    {
        _smsManager = smsManager;
        _applicationUserService = applicationUserService;
    }

    #endregion

}