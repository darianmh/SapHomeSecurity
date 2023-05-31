using System.Net.Sockets;

namespace SapSecurity.Model;

/// <summary>
/// determine gate way url and call back method
/// </summary>
public class HttpGatewayInfo
{
    public HttpGatewayInfo(string route, Func<HttpMessageInfo, NetworkStream, Task> urlCallBack)
    {
        Route = route;
        UrlCallBack = urlCallBack;
    }

    /// <summary>
    /// route for receiving message
    /// </summary>
    public string Route { get; }
    /// <summary>
    /// call back url for each rout
    /// </summary>
    public Func<HttpMessageInfo, NetworkStream, Task> UrlCallBack { get; }
}