using System.ComponentModel.DataAnnotations.Schema;

namespace SapSecurity.Model;

public class SensorLog : BaseEntity
{
    public DateTime DateTimeUtc { get; set; }
    /// <summary>
    /// active deActive of digital sensors
    /// value of analog sensors
    /// </summary>
    public double Status { get; set; }
    public int SensorDetailId { get; set; }

    //np
    [ForeignKey("SensorDetailId")]
    public SensorDetail SensorDetail { get; set; }
}