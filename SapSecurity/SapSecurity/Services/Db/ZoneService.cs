using Microsoft.Extensions.DependencyInjection;
using SapSecurity.Infrastructure.Repositories;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Mapper;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public class ZoneService : IZoneService
{

    #region Fields

    private readonly IZoneRepository _zoneRepository;
    private readonly IMapper _mapper;
    private readonly ISensorDetailService _sensorDetailService;

    #endregion
    #region Methods
    public async Task<List<ZoneViewModel>> GetUserZonesAsync(string userId)
    {
        var list = await _zoneRepository.GetUserZonesAsync(userId);
        return list.Select(model => _mapper.Map(model, IndexManager.GetZoneStatus(model.Id, model.UserId))).ToList();
    }


    #endregion
    #region Utilities

    #endregion
    #region Ctor

    public ZoneService( IServiceScopeFactory serviceScopeFactory)
    {
        var scope = serviceScopeFactory.CreateScope();
        _zoneRepository = scope.ServiceProvider.GetService<IZoneRepository>();
        _mapper = scope.ServiceProvider.GetService<IMapper>();
        _sensorDetailService = scope.ServiceProvider.GetService<ISensorDetailService>();
    }

    #endregion


}