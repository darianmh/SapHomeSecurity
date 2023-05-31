using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Services.Db;

namespace SapSecurity.Services.SocketManager;

public class SocketUdpManager : ISocketUdpManager
{

    #region Fields
    private readonly ILogger<SocketUdpManager> _logger;

    #endregion
    #region Methods
    public string ReadMessage(string message, string key)
    {
        var temp = message.Replace($"<{key}>", String.Empty);
        var split = temp.Split($"</{key}>");
        return split[0];
    }

    public bool OpenSocket(int port, Func<UdpClient, IPEndPoint, string, Guid, Task> messageCallBack,
        Func<Guid, bool>? disconnectCallBack)
    {
        try
        {
            var thread = new Thread(() =>
            {
                UdpClient udpServer = new UdpClient(port);
                ConsoleExtension.SetBaseInfo($"Listening On {udpServer.Client.LocalEndPoint} : {port}");
                while (true)
                {
                    try
                    {
                        var remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        var data = udpServer.Receive(ref remoteEP);
                        var text = Encoding.ASCII.GetString(data);
                        ConsoleExtension.WriteAppInfo($"Text received : {text} from : {remoteEP.Address}");
                        messageCallBack(udpServer, remoteEP, text, Guid.NewGuid());

                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message, e);
                    }
                }

            });
            thread.Start();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
        return false;
    }


    #endregion
    #region Utilities



    #endregion
    #region Ctor

    public SocketUdpManager(ILogger<SocketUdpManager> logger, IApplicationUserService applicationUserService)
    {
        _logger = logger;
    }


    #endregion

}