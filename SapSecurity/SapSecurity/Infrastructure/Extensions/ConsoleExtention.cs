using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapSecurity.Infrastructure.Extensions
{
    public static class ConsoleExtension
    {

        #region Fields

        private static readonly List<string> BaseInfo = new List<string>();
        private static readonly List<string> Messages = new List<string>();


        #endregion
        #region Methods


        public static void WriteAppInfo(string? message = null)
        {
            //Console.WriteLine(message);
            //Console.Clear();
            // if (message != null) LogMessages(message);
            //BaseInfo.ForEach(Console.WriteLine);
            // Messages.ForEach(Console.WriteLine);
        }

        public static void SetBaseInfo(string message)
        {
            BaseInfo.Add(message);
            WriteAppInfo(message);
        }

        #endregion
        #region Utilities


        private static void LogMessages(string message)
        {
            Messages.Add(message);
            if (Messages.Count > 10)
            {
                Messages.RemoveAt(0);
            }
        }

        #endregion
        #region Ctor



        #endregion

    }
}
