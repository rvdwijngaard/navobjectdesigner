using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    interface IUtilities
    {
        FileVersionInfo GetVersion(IObjectDesignerRequest request);
    }
}
