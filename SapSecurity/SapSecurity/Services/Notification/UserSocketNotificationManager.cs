using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Connection;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Notification;
/// <summary>
/// send notification for user through the socket
/// </summary>
public class UserSocketNotificationManager : INotificationManager
{

    #region Fields
    private readonly ILogger<UserSocketNotificationManager> _logger;


    #endregion
    #region Methods


    public async Task<bool> SoundAlert(string userId, AlertLevel alertLevel)
    {
        try
        {
            var sensors = UserSocketHandle.UserSocketInfos.Where(x => x.UserId == userId).ToList();
            var model = new AlertViewModel() { Level = alertLevel };
            var json = JsonConvert.SerializeObject(model);
            var toRemove = new List<UserSocketInfo>();
            foreach (var x in sensors)
            {
                try
                {

                    SocketManager.SocketManager.SendMessage(x.Handler, json, SocketMessageType.Not);
                }
                catch (Exception e)
                {
                    toRemove.Add(x);
                }
            }
            toRemove.ForEach(x => UserSocketHandle.UserSocketInfos.Remove(x));
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
        return false;
    }


    #endregion
    #region Utilities



    #endregion
    #region Ctor

    public UserSocketNotificationManager(ILogger<UserSocketNotificationManager> logger)
    {
        _logger = logger;
    }


    #endregion

}