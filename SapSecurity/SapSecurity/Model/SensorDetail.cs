using System.ComponentModel.DataAnnotations.Schema;

namespace SapSecurity.Model;

/// <summary>
/// each sensor info
/// </summary>
public class SensorDetail : BaseEntity
{
    /// <summary>
    /// unique key of each sensor hardware
    /// </summary>
    public string Identifier { get; set; }
    public int SensorGroupId { get; set; }
    public int ZoneId { get; set; }
    /// <summary>
    /// owner
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// is active pr de active by user
    /// </summary>
    public bool IsActive { get; set; }
    public string Title { get; set; }
    public int? Weight { get; set; }
    /// <summary>
    /// value when sensor is working Properly
    /// </summary>
    public int? NeutralValue { get; set; }

    //np
    [ForeignKey("SensorGroupId")]
    public SensorGroup SensorGroup { get; set; }
    [ForeignKey("ZoneId")]
    public Zone Zone { get; set; }
    public IList<SensorLog> SensorLogs { get; set; }
}