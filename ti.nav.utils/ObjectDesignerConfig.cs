using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI.Nav.Utils.Helpers;

namespace TI.Nav.Utils
{
    public class ObjectDesignerConfig : IObjectDesignerConfig
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Path { get; set; }
        public bool TestMode { get; set; }

        public int MajorVersion
        {
            get { return TestMode ? 0 :Utilities.GetVersion(this).FileMajorPart; }
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
