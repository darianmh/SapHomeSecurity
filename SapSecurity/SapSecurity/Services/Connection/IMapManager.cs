using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SapSecurity.Model.Types;
using SapSecurity.Services.Db;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Connection;

public interface IMapManager
{
    Task SoundAlert(string userId, AlertLevel alertLevel);
}

public class MapManager : IMapManager
{

    #region Fields

    private readonly IApplicationUserService _userService;
    private readonly ILogger<MapManager> _logger;

    #endregion
    #region Methods
    public async Task SoundAlert(string userId, AlertLevel alertLevel)
    {
        try
        {
            if (alertLevel != AlertLevel.High) return;
            var user = await _userService.GetByIdAsync(userId, true);
            if (user == null) return;
            var model = new MapNotificationModel
            {
                Address = user.Address,
                Lan = user.Lan,
                Lat = user.Lat,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                UserId = user.Id,
            };
            var client = new HttpClient();
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));//A
            var result = await client.PostAsync("http://109.122.199.199:7070/Notification",
                new StringContent(JsonConvert.SerializeObject(model),Encoding.UTF8, "application/json"));
            
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
    }


    #endregion
    #region Utilities



    #endregion
    #region Ctor

    public MapManager(IServiceScopeFactory scopeFactory, ILogger<MapManager> logger)
    {
        _logger = logger;
        var scope = scopeFactory.CreateScope();
        _userService = scope.ServiceProvider.GetService<IApplicationUserService>();
    }

    #endregion


}