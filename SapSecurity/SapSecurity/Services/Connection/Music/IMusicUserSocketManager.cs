namespace SapSecurity.Services.Connection.Music;

/// <summary>
/// socket connection for user read data
/// </summary>
public interface IMusicUserSocketManager : IConnectionManager
{
    Task SendMessage(int message);
}