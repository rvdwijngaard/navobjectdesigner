using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    static public class Logger
    {
        public static ILogger ConfigureLogger(string applicationName)
        {
            Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console()
                 .Enrich.FromLogContext()
                 .Enrich.WithMachineName()
                 .Enrich.WithProperty("UserName", Environment.UserName)
                 .Enrich.WithProperty("ApplicationName", applicationName)
                 .CreateLogger();

            return Log.Logger;
        }
    }
}
