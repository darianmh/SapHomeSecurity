using Microsoft.Extensions.DependencyInjection;
using SapSecurity.Model.Types;
using SapSecurity.ViewModel;

namespace SapSecurity.Services;

public interface IConnectionHub
{
    /// <summary>
    /// setup program
    /// </summary>
    /// <returns></returns>
    void Setup();
    /// <summary>
    /// run a udp socket for sensor logs
    /// </summary>
    /// <returns></returns>
    void RunRegisterSensorLogSocketUdpAsync();
    /// <summary>
    /// user music socket
    /// </summary>
    void RunMusicSocketAsync();
    /// <summary>
    /// run a socket for user connection
    /// </summary>
    /// <returns></returns>
    void RunRegisterUserSocketAsync();
    /// <summary>
    /// run a web socket for user connection
    /// </summary>
    /// <returns></returns>
    void RunRegisterUserWebSocketAsync();



}