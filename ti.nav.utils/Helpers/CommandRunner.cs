using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils.Helpers
{
    public class CommandRunner : ICommandRunner
    {
        public string RunCommand(IObjectDesignerConfig config, string command)        
        {
            if (!File.Exists(config.ExecPath))
                throw new FileNotFoundException(string.Format("The path for the Micorosoft Dynamics NAV Object Designer does not exist [{0}]", config.ExecPath));

            string logFile = Utilities.GetTempFileName(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());

            string arguments = String.Format("{0},logfile=\"{1}\"",command, logFile);

            System.Diagnostics.ProcessStartInfo pInfo = new ProcessStartInfo(config.ExecPath, arguments);

            pInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pInfo.CreateNoWindow = true;

            Process process = Process.Start(pInfo);
            string message = string.Empty;
            process.WaitForExit();

            if (process.ExitCode == 1)
            {
                if (File.Exists(logFile))
                {
                    message = File.ReadAllText(logFile);
                }
                else
                {
                    Log.Error("Unknown error occured in finsql while processing {arguments}", arguments);
                }
            }
            File.Delete(logFile);
            return message;
        }
    }
}
