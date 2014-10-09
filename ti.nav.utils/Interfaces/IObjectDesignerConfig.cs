﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public interface IObjectDesignerConfig
    {
        string Server { get; set; }
        string Database { get; set; }
        string Path { get; set; }
        string ExecPath { get;  }
        int MajorVersion { get; }
        int MinorVersion { get; }
        
        string NavServerName { get; set; }
        
        string NavServerInstance { get; set; }
        
        string NavServerManagementPort { get; set; }
    }
}
