namespace SapSecurity.Model;

/// <summary>
/// Zone of sensors
/// </summary>
public class Zone : BaseEntity
{
    public string Title { get; set; }
    /// <summary>
    /// owner
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// icon
    /// </summary>
    public string IconPath { get; set; }

    //np
    public IList<SensorDetail> SensorDetails { get; set; }
}