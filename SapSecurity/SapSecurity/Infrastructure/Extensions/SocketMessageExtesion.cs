using SapSecurity.Model.Types;

namespace SapSecurity.Infrastructure.Extensions
{
    public static class SocketMessageExtension
    {
        public static string GetMessageInFormat(this SocketMessageType socketMessageType, string message)
        {
            return $"<{socketMessageType}>{message}</{socketMessageType}>";
        }
    }
}
