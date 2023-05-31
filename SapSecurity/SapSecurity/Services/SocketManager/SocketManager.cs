using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using SapSecurity.Infrastructure.Extensions;
using SapSecurity.Model.Types;
using SapSecurity.Infrastructure.Extensions;

namespace SapSecurity.Services.SocketManager;

public class SocketManager : ISocketManager
{

    #region Fields

    private readonly ILogger<SocketManager> _logger;


    #endregion
    #region Methods

    public string ReadMessage(string message, string key)
    {
        var temp = message.Replace($"<{key}>", String.Empty);
        var split = temp.Split($"</{key}>");
        return split[0];
    }
    public virtual bool OpenSocket(int port, Func<Socket, string, Guid, Task> messageCallBack,
        Func<Guid, bool>? disconnectCallBack)
    {
        try
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, port));
            listener.Listen(10);
            ConsoleExtension.SetBaseInfo($"Listening On {listener.LocalEndPoint} : {port}");
            var thread = new Thread(async () =>
            {
                while (true)
                {
                    try
                    {
                        Socket handler = await listener.AcceptAsync();
                        Accept(handler, messageCallBack, disconnectCallBack);
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

    public static void SendMessage(Socket handler, string message, SocketMessageType socketMessageType)
    {
        try
        {
            if (handler.Connected)
            {
                var sendMessage = socketMessageType.GetMessageInFormat(message);
                handler.Send(Encoding.ASCII.GetBytes(sendMessage));
            }
        }
        catch (Exception)
        {
            //ignore
        }
    }

    #endregion
    #region Utilities

    protected void Accept(Socket handler, Func<Socket, string, Guid, Task> messageCallBack, Func<Guid, bool>? disconnectCallBack)
    {
        var thread = new Thread(async () =>
         {
            // Incoming data from the client.
             ConsoleExtension.WriteAppInfo($"Client Connected {handler.RemoteEndPoint}");
             var guId = Guid.NewGuid();
             string? endpoint = "";
             try
             {
                 endpoint = handler.RemoteEndPoint?.ToString();
                 var ts = new CancellationTokenSource();
                 CancellationToken ct = ts.Token;
                 Thread? task = null;
                 while (handler.IsConnected())
                 {
                     try
                     {
                         try
                         {
                             var bytes = new byte[1024];
                             int bytesRec = await handler.ReceiveAsync(bytes, SocketFlags.None, ct);
                             var data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                             if (data.Length > 0)
                             {
                                 messageCallBack(handler, data, guId);
                                 ConsoleExtension.WriteAppInfo(
                                     $"Text received : {data} from : {handler.RemoteEndPoint}");
                             }
                         }
                         catch (Exception e)
                         {
                             _logger.LogError(e.Message, e);
                         }

                     }
                     catch (Exception e)
                     {
                         _logger.LogError(e.Message, e);
                     }

                 }

                 if (task != null) ts.Cancel();

                 handler.Shutdown(SocketShutdown.Both);
                 handler.Close();
             }
             catch (Exception e)
             {
                 _logger.LogError(e.Message, e);
             }

             ConsoleExtension.WriteAppInfo($"{endpoint} Disconnected");

             disconnectCallBack?.Invoke(guId);
         });
        thread.Start();
    }


    #endregion
    #region Ctor


    public SocketManager(ILogger<SocketManager> logger)
    {
        _logger = logger;
    }


    #endregion

}