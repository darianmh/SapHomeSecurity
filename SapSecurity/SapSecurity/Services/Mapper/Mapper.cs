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

    public SensorViewModel? Map(SensorDetail model, SensorStatus sensorStatus, int? sensValue, int sensPercent)
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

    public SensorInfoModel MapInfo(SensorDetail sensor)
    {
        var neutralValue = sensor.NeutralValue;
        if (neutralValue == null) neutralValue = sensor.SensorGroup.NeutralValue;
        if (neutralValue == null) neutralValue = 0;
        return new SensorInfoModel()
        {
            SensorIdentifier = sensor.Identifier,
            GroupId = sensor.SensorGroupId,
            NeutralValue = (int)neutralValue,
            SensorId = sensor.Id,
            UserId = sensor.UserId,
            Weight = sensor.Weight ?? sensor.SensorGroup.Weight,
            ZoneId = sensor.ZoneId,
            IsDigital = sensor.SensorGroup.IsDigital,
            WeightPercent = sensor.SensorGroup.WeightPercent ?? 100,
            GroupType = sensor.SensorGroup.SensorGroupType,
        };
    }

    public SensorLog Map(int status, int sensorDetailId)
    {
        return new SensorLog()
        {
            Status = status,
            DateTimeUtc = DateTime.Now.ToUniversalTime(),
            SensorDetailId = sensorDetailId
        };
    }
}