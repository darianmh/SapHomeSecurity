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
    Act = 9,
    /// <summary>
    /// define admin as user 
    /// </summary>
    AId = 10,
    /// <summary>
    /// send received message to admin
    /// </summary>
    Adm = 11,
    /// <summary>
    /// send alive time to admin
    /// </summary>
    Adl = 12,
    /// <summary>
    /// send message
    /// </summary>
    Sed = 13,
    /// <summary>
    /// send indexes
    /// </summary>
    Ind = 14
}