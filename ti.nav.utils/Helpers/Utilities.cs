using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils.Helpers
{
    public static class Utilities 
    {
        public static FileVersionInfo GetVersion(IObjectDesignerConfig request)
        {
            return FileVersionInfo.GetVersionInfo(request.ExecPath);            
        }

        // The temp file is important: each finsql executable must have the log files in a separate directory!
        public static string GetTempFileName(string label)
        {
            string tempPath = Path.Combine(System.IO.Path.GetTempPath(), DateTime.Today.ToShortDateString(), label);
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return Path.Combine(tempPath, Guid.NewGuid().ToString() + ".txt");
        }
    }
}
