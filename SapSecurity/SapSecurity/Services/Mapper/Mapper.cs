using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Mapper;

public class Mapper : IMapper
{
    public SensorGroupViewModel Map(SensorGroup model, SensorStatus groupStatus)
    {
        return new SensorGroupViewModel()
        {
            Status = groupStatus,
            Icon = model.ImagePath,
            Title = model.Title,
            Id = model.Id,
            IconDanger = model.ImagePathDanger,
            IconSafe = model.SafeImagePath
        };
    }

    public SensorViewModel? Map(SensorDetail model, SensorStatus sensorStatus, double? sensValue, int sensPercent)
    {
        return new SensorViewModel()
        {
            Status = sensorStatus,
            GroupTitle = model.SensorGroup.Title,
            ZoneTitle = model.Zone.Title,
            Id = model.Id,
            IsDigital = model.SensorGroup.IsDigital,
            SensValue = sensValue,
            GroupImagePath = model.SensorGroup.ImagePath,
            SensorName = model.Title,
            SensPercent = sensPercent,
            IconDanger = model.SensorGroup.ImagePathDanger,
            IconSafe = model.SensorGroup.SafeImagePath
        };
    }

    public ZoneViewModel Map(Zone model, SensorStatus zoneStatus)
    {
        return new ZoneViewModel()
        {
            SensorCount = model.SensorDetails.Count,
            ZoneName = model.Title,
            ZoneStatus = zoneStatus,
            Id = model.Id,
            IconPath = model.IconPath,
        };
    }

    public CameraImageViewModel Map(CameraImage model)
    {
        return new CameraImageViewModel()
        {
            DateTime = model.DateTimeUtc.ToLocalTime(),
            Path = "http://109.122.199.199:8090/" + model.Path,
            Id = model.Id
        };
    }

    public SensorLog Map(double status, int sensorDetailId)
    {
        return new SensorLog()
        {
            Status = status,
            DateTimeUtc = DateTime.Now.ToUniversalTime(),
            SensorDetailId = sensorDetailId
        };
    }
}