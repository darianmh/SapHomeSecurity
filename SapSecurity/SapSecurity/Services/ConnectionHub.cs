using SapSecurity.Model;
using SapSecurity.Services.Connection;
using SapSecurity.Services.Security;
using SapSecurity.Services.Tools;
using SapSecurity.Data;

namespace SapSecurity.Services;

public class ConnectionHub : IConnectionHub
{
    #region Fields

    private readonly IHomeUdpSocketManager _homeUdpSocketManager;
    private readonly IUserSocketManager _userSocketManager;
    private readonly IUserWebSocketManager _userWebSocketManager;
    private readonly ApplicationContext _context;

    #endregion
    #region Methods


    public void Setup()
    {
   
    }


    public void RunRegisterSensorLogSocketUdpAsync()
        => _homeUdpSocketManager.SetupConnectionAsync();

    public void RunRegisterUserSocketAsync()
    =>
        //setup client socket
        _userSocketManager.SetupConnectionAsync();

    public void RunRegisterUserWebSocketAsync()
        => _userWebSocketManager.SetupConnectionAsync();




    #endregion
    #region Utilities






    #endregion
    #region Ctor

    public ConnectionHub(
        IUserSocketManager userSocketManager,
        ISecurityManager securityManager,
        ApplicationContext context,
        IUserWebSocketManager userWebSocketManager,
        IHomeUdpSocketManager homeUdpSocketManager)
    {
        _userSocketManager = userSocketManager;
        _context = context;
        _userWebSocketManager = userWebSocketManager;
        _homeUdpSocketManager = homeUdpSocketManager;
    }


    #endregion


}