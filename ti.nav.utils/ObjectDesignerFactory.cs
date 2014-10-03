using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI.Nav.Utils.Helpers;
using TI.Nav.Utils.Versions;

namespace TI.Nav.Utils
{
    public static class ObjectDesignerFactory
    {
        public static IObjectDesigner GetObjectDesigner(IObjectDesignerConfig request)
        {
            if (request.MajorVersion == 0)
            {
                return new PowershellStub(request);
            }
            if (request.MajorVersion < 7)
            {
                return new Classic(request);
            }
            else if ((request.MajorVersion == 7) && (request.MinorVersion == 0))
            {
                return new Nav2013(request, new CommandRunner());
            }
            else if ((request.MajorVersion == 7) && (request.MinorVersion > 0))
            {
                return new Nav2013R2(request, new CommandRunner());
            }
            else
            {
                return new Nav2015(request, new CommandRunner());
            }                        
        }       
    }      
}

