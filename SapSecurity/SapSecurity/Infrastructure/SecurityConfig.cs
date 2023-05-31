using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapSecurity.Infrastructure
{
    public static class SecurityConfig
    {
        //public static int LastLogSeconds { get; set; } = 15;
        public static int UserHttpPort { get; set; } = 7080;
        public static int HomeSocketPort { get; set; } = 7081;
        public static int HomeHttpPort { get; set; } = 7082;
        public static int UserSocket { get; set; } = 7083;
        public static int UserWebSocket { get; set; } = 7084;
        public static int HomeUdpSocketPort { get; set; } = 7085;
        public static int IndexDangerValue { get; set; } = 50;
        public static int IndexWarningValue { get; set; } = 20;
    }


}
