using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public static class ObjectDesignerFactory
    {
        public static IObjectDesigner GetObjectDesigner(IObjectDesignerRequest request)
        {                        
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
   
    public class ObjectDesignerRequest : IObjectDesignerRequest
    {       
        public string Server{ get; set; }
        public string Database { get; set; }
        public string Path { get; set; }        

        public int MajorVersion
        {
            get { return Utilities.GetVersion(this).FileMajorPart; }
        }

        public int MinorVersion
        {
            get { return Utilities.GetVersion(this).FileMinorPart; }
        }

        public string ExecPath
        {
            get
            {
                return System.IO.Path.Combine(this.Path, "finsql.exe");
            }
         
        }
    }
}

