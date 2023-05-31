using System.Net;
using System.Net.Sockets;

namespace SapSecurity.Services.SocketManager;

public interface ISocketUdpManager
{

    /// <summary>
    /// returns exact message without splitter keys
    /// </summary>
    /// <param name="message">received message</param>
    /// <param name="key">tag name key</param>
    /// <returns></returns>
    string ReadMessage(string message, string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="port">port</param>
    /// <param name="messageCallBack">function to call when message received</param>
    /// <param name="disconnectCallBack"></param>
    /// <returns></returns>
    bool OpenSocket(int port, Func<UdpClient, IPEndPoint, string, Guid, Task> messageCallBack,
        Func<Guid, bool>? disconnectCallBack);
}