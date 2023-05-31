using SapSecurity.Model;
using SapSecurity.Model.Types;

namespace SapSecurity.ViewModel;

public class SensorGroupViewModel
{
    public string Icon { get; set; }
    public string Title { get; set; }
    public SensorStatus Status { get; set; }
    public List<SensorViewModel> Sensors { get; set; }
    public int Id { get; set; }
    public string? IconDanger { get; set; }
    public string IconSafe { get; set; }
}