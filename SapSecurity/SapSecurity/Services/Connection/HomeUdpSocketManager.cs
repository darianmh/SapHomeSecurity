using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SapSecurity.Infrastructure;
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Model;
using SapSecurity.Model.Types;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Db;
using SapSecurity.Services.Notification;
using SapSecurity.Services.Security;
using SapSecurity.Services.SocketManager;

namespace SapSecurity.Services.Connection;

///<inheritdoc cref="IHomeUdpSocketManager"/>
public class HomeUdpSocketManager : ConnectionManager, IHomeUdpSocketManager
{

    #region Fields


    private readonly ISocketUdpManager _socketManager;
    private readonly int _port = SecurityConfig.HomeUdpSocketPort;
    private readonly ILogger<HomeUdpSocketManager> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    #endregion
    #region Methods

    public override bool SetupConnectionAsync()
    {
        return _socketManager.OpenSocket(_port, (socket, endPoint, message, socketId) => CallBack(socket, endPoint, message, socketId), null);
    }


    /// <summary>
    /// save socket info
    /// </summary>
    /// <param name="socket">connected client</param>
    /// <param name="message">received message</param>
    /// <param name="socketId">connected socket id</param>
    /// <param name="finalizeCallBack">call back after work finish</param>
    /// <returns></returns>
    public async Task CallBack(UdpClient socket, IPEndPoint endPoint, string message, Guid socketId)
    {
        try
        {
            if (message.Contains($"<{SocketMessageType.Sen}>"))
            {
                await LogAsync(socket, endPoint, message);
            }
            else if (message.Contains($"<{SocketMessageType.Act}>"))
            {

                var sensorId = _socketManager.ReadMessage(message, SocketMessageType.Act.ToString());
                var sensor = HomeSocketHandle.GetSensorId(sensorId);
                if (sensor != null)
                    IndexManager.SetAliveMessage(sensor.Value);
            }
            else
            {
                //ignore
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
    }
    #endregion
    #region Utilities

    /// <summary>
    /// log info to db
    /// sens info and sensor id given in one message
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="endPoint"></param>
    /// <param name="message"></param>
    /// <param name="finalizeCallBack"></param>
    /// <returns></returns>
    private async Task LogAsync(UdpClient socket, IPEndPoint endPoint, string message)
    {
        try
        {
            var scope = _serviceScopeFactory.CreateScope();
            var _sensorDetailService = scope.ServiceProvider.GetService<ISensorDetailService>();
            var _sensorLogService = scope.ServiceProvider.GetService<ISensorLogService>();
            var _securityManager = scope.ServiceProvider.GetService<ISecurityManager>();
            var listOfSens = message.Split($"</{SocketMessageType.Sen}>");
            int? lastSensor = null;
            double? lastValue = null;
            foreach (var sense in listOfSens)
            {
                var messageInfo = _socketManager.ReadMessage(sense, SocketMessageType.Sen.ToString());
                if (string.IsNullOrEmpty(messageInfo)) continue;
                var messageSplit = messageInfo.Split(',');
                if (messageSplit.Length != 2) continue;
                var sensorId = messageSplit[0];
                var log = messageSplit[1];
                var sensor = HomeSocketHandle.GetSensorId(sensorId);
                if (sensor == null || sensor == 0)
                {
                    Console.WriteLine($"invalid sensor: {sensorId}, value: {log}");
                    continue;
                }
                var check = double.TryParse(log, out var status);
                if (check)
                {
                    lastSensor = sensor;
                    lastValue = status;
                    _logger.LogInformation($"status: {status}, sensor: {sensor}");
                    IndexManager.SetAliveMessage(sensor.Value);
                    await _sensorLogService.LogAsync(status, sensor.Value);
                }
            }
            if (lastSensor != null)
            {
                var sensorInfo = await _sensorDetailService.GetByIdAsync(lastSensor.Value);
                if (sensorInfo != null && lastValue != null)
                {
                    await _securityManager.SensReceiver(sensorInfo, lastValue.Value);
                    {
                        var callBackMessage = CacheManager.GetUserSecurityStatus(sensorInfo).ToString();
                        socket.Send(
                            Encoding.ASCII.GetBytes(
                                callBackMessage
                                )
                            , endPoint);
                        ConsoleExtension.WriteAppInfo($"Sent message to udp: {callBackMessage}");
                    }
                }
            }
            scope.Dispose();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            throw;
        }
    }



    #endregion
    #region Ctor

    public HomeUdpSocketManager(ISocketUdpManager socketManager, ISensorLogService sensorLogService, ILogger<HomeUdpSocketManager> logger, HomeSocketNotificationManager notificationManager, ISensorDetailService sensorDetailService, IServiceScopeFactory serviceScopeFactory) : base(notificationManager)
    {
        _socketManager = socketManager;
        //_sensorLogService = sensorLogService;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        //_sensorDetailService = sensorDetailService;
    }


    #endregion
}