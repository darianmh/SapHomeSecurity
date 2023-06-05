using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using SapSecurity.Infrastructure.Repositories;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Mapper;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public class SensorGroupService : ISensorGroupService
{

    #region Fields

    private readonly ISensorGroupRepository _sensorGroupRepository;
    private readonly ISensorDetailService _sensorDetailService;
    private readonly IMapper _mapper;

    #endregion
    #region Methods


    public async Task<PaginatedViewModel<SensorGroupViewModel>> GetAllAsync(int pageSize, int pageIndex, string userId)
    {
        var list = await _sensorGroupRepository.GetAllAsync(pageSize, pageIndex);
        return new PaginatedViewModel<SensorGroupViewModel>()
        {
            Count = list.Count,
            Data = Map(list.Data, userId)
        };
    }


    public async Task<List<SensorGroupViewModel>> GetAllAsync(string userId)
    {
        var list = await _sensorGroupRepository.GetAllAsync();
        return Map(list, userId);

    }

    public async Task<List<SelectListItem>> GetAllSelectList()
    {
        var all = await _sensorGroupRepository.GetAllAsync();
        return all.Select(x => new SelectListItem() { Text = x.Title, Value = x.Id.ToString() }).ToList();
    }

    #endregion
    #region Utilities

    private List<SensorGroupViewModel> Map(List<SensorGroup> list, string userId)
    {
        return list.Select(async model =>
        {
            var sensors = await GetSensors(model.SensorDetails, userId);
            var status = GetGroupStatus(sensors);
            var result = _mapper.Map(model, status);
            result.Sensors = sensors;
            return result;
        }).Select(x => x.Result).ToList();
    }
    private SensorStatus GetGroupStatus(List<SensorViewModel> resultSensors)
    {
        var allSensorStatus = resultSensors.Select(x => x.Status).ToList();
        SensorStatus groupStatus;
        //اگر تعداد خطر ها بیش از 50 درصد بود به معنای خطر
        if (allSensorStatus.Count(x => x == SensorStatus.Danger) > allSensorStatus.Count / 2) groupStatus = SensorStatus.Danger;
        //اگر یک خطر بود یا تعداد تذکر ها بیش از 50 درصد یعنی تذکر
        else if ((allSensorStatus.Count(x => x == SensorStatus.Warning) > allSensorStatus.Count / 2)
                 || allSensorStatus.Any(x => x == SensorStatus.Danger)
                ) groupStatus = SensorStatus.Warning;
        else if (allSensorStatus.All(x => x == SensorStatus.DeActive))
            groupStatus = SensorStatus.DeActive;
        else
            groupStatus = SensorStatus.Active;
        return groupStatus;
    }

    private async Task<List<SensorViewModel>> GetSensors(IList<SensorDetail> sensorDetails, string userId)
    {
        return sensorDetails.Where(x => x.UserId == userId).Select(async sensor =>
        {
            var sensPercent = await _sensorDetailService.GetSensPercent(sensor);
            var sensorStatus = IndexManager.GetSensorStatus(sensor.Id, sensor.ZoneId, userId);
            return _mapper.Map(sensor, sensorStatus, sensor.SensorLogs.MaxBy(x => x.Id)?.Status, sensPercent);
        }).Select(x => x.Result).ToList();
    }


    #endregion
    #region Ctor


    public SensorGroupService(IServiceScopeFactory serviceScopeFactory)
    {
        var scope = serviceScopeFactory.CreateScope();
        _sensorGroupRepository = scope.ServiceProvider.GetService<ISensorGroupRepository>();
        _mapper = scope.ServiceProvider.GetService<IMapper>();
        _sensorDetailService = scope.ServiceProvider.GetService<ISensorDetailService>();
    }

    #endregion

}