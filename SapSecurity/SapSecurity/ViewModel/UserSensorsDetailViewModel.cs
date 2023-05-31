using System.Text.Json.Serialization;
using SapSecurity.Model.Types;

namespace SapSecurity.ViewModel;

public class ZoneViewModel
{
    public int SensorCount { get; set; }
    public string ZoneName { get; set; }
    public SensorStatus ZoneStatus { get; set; }
    public int Id { get; set; }
    public string IconPath { get; set; }
}
