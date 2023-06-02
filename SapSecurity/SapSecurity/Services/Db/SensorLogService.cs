using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SapSecurity.Infrastructure;
using SapSecurity.Infrastructure.Repositories;
using SapSecurity.Model;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Mapper;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public class SensorLogService : ISensorLogService
{


    #region Fields
    private readonly ISensorLogRepository _sensorLogRepository;
    private readonly IMapper _mapper;


    #endregion
    #region Methods

    public async Task LogAsync(int status, int sensorDetailId)
    {
        var log = _mapper.Map(status, sensorDetailId);
        await _sensorLogRepository.InsertAsync(log);
    }


    public async Task SaveChangesAsync()
    {
        await _sensorLogRepository.SaveChangeAsync();
    }

    public Task<SensorLog?> GetLastLogAsync(int sensorId)
    {
        return _sensorLogRepository.GetLastLogAsync(sensorId);
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor

    public SensorLogService(IServiceScopeFactory serviceScopeFactory)
    {
        var scope = serviceScopeFactory.CreateScope();
        _sensorLogRepository = scope.ServiceProvider.GetService<ISensorLogRepository>();
        _mapper = scope.ServiceProvider.GetService<IMapper>();
    }


    #endregion

}