using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SapSecurity.Model.Types;
using SapSecurity.Services.Connection;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Notification;
/// <summary>
/// send socket notification to sensors
/// </summary>
public class HomeSocketNotificationManager : INotificationManager
{

    #region Fields

    private readonly ILogger<HomeSocketNotificationManager> _logger;

    #endregion
    #region Methods


    public async Task<bool> SoundAlert(string userId, AlertLevel alertLevel)
    {
        try
        {
            var sensors = HomeSocketHandle.SensorSocketInfos.Where(x => x.UserId == userId).ToList();
            var model = new AlertViewModel() { Level = alertLevel };
            var json = JsonConvert.SerializeObject(model);
            sensors.ForEach(x => SocketManager.SocketManager.SendMessage(x.Handler, json, SocketMessageType.Not));
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

    public HomeSocketNotificationManager(ILogger<HomeSocketNotificationManager> logger)
    {
        _logger = logger;
    }


    #endregion

}