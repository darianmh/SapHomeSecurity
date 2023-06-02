using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SapSecurity.Infrastructure;
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Model.Types;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Db;
using SapSecurity.Services.Notification;
using SapSecurity.Services.Security;
using SapSecurity.Services.SocketManager;
using SapSecurity.ViewModel;

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
    /// <param name="endPoint"></param>
    /// <param name="message">received message</param>
    /// <param name="socketId">connected socket id</param>
    /// <returns></returns>
    public async Task CallBack(UdpClient socket, IPEndPoint endPoint, string message, Guid socketId)
    {
        try
        {

            var scope = _serviceScopeFactory.CreateScope();
            var sensorDetailService = scope.ServiceProvider.GetService<ISensorDetailService>();
            var sensorLogService = scope.ServiceProvider.GetService<ISensorLogService>();
            var securityManager = scope.ServiceProvider.GetService<ISecurityManager>();
            if (securityManager == null || sensorDetailService == null || sensorLogService == null) return;
            if (message.Contains($"<{SocketMessageType.Sen}>"))
            {
                await LogAsync(socket, endPoint, message, sensorDetailService, sensorLogService, securityManager);
            }
            else if (message.Contains($"<{SocketMessageType.Act}>"))
            {

                var sensorId = _socketManager.ReadMessage(message, SocketMessageType.Act.ToString());
                var sensor = await sensorDetailService.GetSensorInfoByIdentifier(sensorId);
                if (sensor != null)
                    IndexManager.SetAliveMessage(sensor.SensorId);
            }
            else
            {
                //ignore
            }
            scope.Dispose();
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
    /// <param name="sensorDetailService"></param>
    /// <param name="sensorLogService"></param>
    /// <param name="securityManager"></param>
    /// <returns></returns>
    private async Task LogAsync(UdpClient socket, IPEndPoint endPoint, string message, ISensorDetailService sensorDetailService, ISensorLogService sensorLogService, ISecurityManager securityManager)
    {
        try
        {
            var listOfSens = message.Split($"</{SocketMessageType.Sen}>");
            SensorInfoModel? lastSensor = null;
            int? lastValue = null;
            foreach (var sense in listOfSens)
            {
                var messageInfo = _socketManager.ReadMessage(sense, SocketMessageType.Sen.ToString());
                if (string.IsNullOrEmpty(messageInfo)) continue;
                var messageSplit = messageInfo.Split(',');
                if (messageSplit.Length != 2) continue;
                var sensorId = messageSplit[0];
                var log = messageSplit[1];
                var sensor = await sensorDetailService.GetSensorInfoByIdentifier(sensorId);
                if (sensor == null)
                {
                    Console.WriteLine($"invalid sensor: {sensorId}, value: {log}");
                    continue;
                }
                var check = int.TryParse(log, out var status);
                if (check)
                {
                    lastSensor = sensor;
                    lastValue = status;
                    IndexManager.SetAliveMessage(sensor.SensorId);
                    await sensorLogService.LogAsync(status, sensor.SensorId);
                }
            }
            if (lastSensor != null && lastValue != null)
            {
                var callBackMessage = await securityManager.SensReceiver(lastSensor, lastValue.Value);
                if (callBackMessage != null)
                {
                    socket.Send(
                        BitConverter.GetBytes((int)callBackMessage)
                        , endPoint);
                    CacheManager.SetSensorLastMessage(lastSensor.SensorId, (int)callBackMessage);
                    ConsoleExtension.WriteAppInfo($"Sent message to udp: {callBackMessage}");
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            throw;
        }
    }



    #endregion
    #region Ctor

    public HomeUdpSocketManager(ISocketUdpManager socketManager, ILogger<HomeUdpSocketManager> logger, HomeSocketNotificationManager notificationManager, IServiceScopeFactory serviceScopeFactory) : base(notificationManager)
    {
        _socketManager = socketManager;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }


    #endregion
}