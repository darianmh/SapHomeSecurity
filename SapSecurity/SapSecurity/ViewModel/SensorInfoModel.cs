using SapSecurity.Model;

namespace SapSecurity.ViewModel;

public class SensorInfoModel
{
    public int SensorId { get; set; }
    public string SensorIdentifier { get; set; }
    public int ZoneId { get; set; }
    public int GroupId { get; set; }
    public string UserId { get; set; }
    public int Weight { get; set; }
    public int WeightPercent { get; set; }
    public int NeutralValue { get; set; }
    public bool IsDigital { get; set; }
    public SensorGroupType  GroupType { get; set; }
}