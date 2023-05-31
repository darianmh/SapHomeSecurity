using Microsoft.Extensions.DependencyInjection;
using SapSecurity.Infrastructure.Repositories;
using SapSecurity.Model;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Mapper;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public class SensorDetailService : ISensorDetailService
{

    #region Fields

    private readonly ISensorDetailRepository _sensorDetailRepository;
    private readonly ISensorLogService _sensorLogService;
    private readonly IMapper _mapper;
    #endregion
    #region Methods


    public async Task<string?> GetUserBySensor(int sensorId)
    {
        var detail = await _sensorDetailRepository.GetByIdAsync(sensorId);
        return detail?.UserId;
    }

    public async Task<SensorDetail?> GetByIdentifierAsync(string identifier)
        => await _sensorDetailRepository.GetByIdentifier(identifier);

    public async Task<SensorDetail?> GetByIdAsync(int id)
    {
        var sensorDetail = CacheManager.SensorDetails.FirstOrDefault(x => x.Id == id);
        if (sensorDetail == null)
        {
            sensorDetail = await _sensorDetailRepository.GetByIdAsync(id);
            if (sensorDetail != null) CacheManager.AddSensorDetail(sensorDetail);
        }
        return sensorDetail;
    }

    public async Task<bool> SetSensorState(int sensorId, int sensorState)
    {
        var sensor = await GetByIdAsync(sensorId);
        if (sensor == null) return false;
        if (sensorState == 0)
            sensor.IsActive = false;
        else if (sensorState == 1)
            sensor.IsActive = true;
        else
            return false;
        _sensorDetailRepository.Update(sensor);
        await _sensorDetailRepository.SaveChangeAsync();
        return true;
    }

    public async Task<List<SensorViewModel>> GetAllSensors(int zoneId)
    {
        var sensors = await _sensorDetailRepository.GetAllSensors(zoneId);
        return sensors.Select(async sensorDetail => await GetSensorViewModel(sensorDetail)).Select(x => x.Result).ToList();
    }

    public async Task<SensorViewModel?> GetSensorViewModel(SensorDetail sensorDetail)
    {
        var sensPercent = await GetSensPercent(sensorDetail);
        return _mapper.Map(sensorDetail, IndexManager.GetSensorStatus(sensorDetail.Id, sensorDetail.ZoneId, sensorDetail.UserId),
            CacheManager.GetSensorsLastValue(sensorDetail.Id), sensPercent);
    }



    public async Task<int> GetSensPercent(SensorDetail sensor)
    {
        if (!sensor.IsActive) return 0;
        var lastLog = await _sensorLogService.GetLastLogAsync(sensor.Id);
        return GetSensPercent(sensor, lastLog?.Status);
    }

    public int GetSensPercent(SensorDetail sensor, double? lastValue)
    {
        if (lastValue == null) return 100;
        var neutralValue = sensor.SensorGroup.NeutralValue ?? 1;
        if (!sensor.SensorGroup.IsDigital)
        {
            var percent = (neutralValue - lastValue) / neutralValue * 100;
            return Convert.ToInt32(percent);
        }
        else
        {
            if (Math.Abs(lastValue.Value - neutralValue) > 0.1) return 0;
            return 100;
        }
    }

    public async Task<List<SensorDetail>> GetDeActiveSensors(string userId)
    {
        var allUserSensors = await _sensorDetailRepository.GetAllSensors(userId);
        var deActive = new List<SensorDetail>();
        foreach (var sensor in allUserSensors)
        {
            var date = IndexManager.GetAliveDate(sensor.Id);
            if (date == null)
            {
                deActive.Add(sensor);
                continue;
            }

            if (date.Value.AddSeconds(60) < DateTime.Now)
            {
                deActive.Add(sensor);
                continue;
            }
        }
        return deActive;
    }

    public async Task<SensorViewModel?> GetByIdViewModelAsync(int id)
    {
        var sensor = await GetByIdAsync(id);
        return sensor != null ? await GetSensorViewModel(sensor) : null;
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor


    public SensorDetailService(IServiceScopeFactory serviceScopeFactory)
    {
        var scope = serviceScopeFactory.CreateScope();
        _sensorDetailRepository = scope.ServiceProvider.GetService<ISensorDetailRepository>();
        _mapper = scope.ServiceProvider.GetService<IMapper>();
        _sensorLogService = scope.ServiceProvider.GetService<ISensorLogService>();
    }

    #endregion


}