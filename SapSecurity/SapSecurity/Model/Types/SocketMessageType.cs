namespace SapSecurity.Model.Types;

public enum SocketMessageType
{
    /// <summary>
    /// sensor log
    /// </summary>
    Sen = 1,
    /// <summary>
    /// app notification
    /// </summary>
    Not = 2,
    /// <summary>
    /// sensor identifier
    /// </summary>
    Ide = 3,
    /// <summary>
    /// user identifier
    /// </summary>
    UId = 4,
    /// <summary>
    /// hand shake request from sensors
    /// </summary>
    Hns = 5,
    /// <summary>
    /// sensor status notification
    /// </summary>
    SNo = 6,
    /// <summary>
    /// user active status notification
    /// </summary>
    ANo = 7,
    /// <summary>
    /// zone status notification
    /// </summary>
    ZNo = 8,
    /// <summary>
    /// sensor active message
    /// </summary>
    Act=9
}