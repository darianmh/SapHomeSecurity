using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Mapper;

public interface IMapper
{
    SensorGroupViewModel Map(SensorGroup model, SensorStatus groupStatus);
    SensorLog Map(double status, int sensorDetailId);
    SensorViewModel? Map(SensorDetail model, SensorStatus sensorStatus, double? sensValue, int sensPercent);
    ZoneViewModel Map(Zone model, SensorStatus zoneStatus);
    CameraImageViewModel Map(CameraImage model);
}