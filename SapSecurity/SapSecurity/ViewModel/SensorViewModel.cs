using SapSecurity.Model;
using SapSecurity.Model.Types;

namespace SapSecurity.ViewModel;

public class SensorViewModel
{
    public string GroupTitle { get; set; }
    public string ZoneTitle { get; set; }
    public SensorStatus Status { get; set; }
    public double? SensValue { get; set; }
    public bool IsDigital { get; set; }
    public int Id { get; set; }
    public string Identifier { get; set; }
    public string SensorName { get; set; }
    public string GroupImagePath { get; set; }
    public int SensPercent { get; set; }
    public string? IconDanger { get; set; }
    public string? IconSafe { get; set; }
}