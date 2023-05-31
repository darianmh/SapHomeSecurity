namespace SapSecurity.ViewModel;

/// <summary>
/// response of sensor status to user
/// </summary>
public class SensorStatusViewModel
{
    public SensorStatusViewModel(double status, int sensorId)
    {
        Status = status;
        SensorId = sensorId;
    }

    public double Status { get; set; }
    public int SensorId { get; set; }
}