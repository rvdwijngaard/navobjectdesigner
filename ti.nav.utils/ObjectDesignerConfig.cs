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
        /// <summary>
        /// The name of the database server on which you want to run the command. The format of this parameter is <computername>\<database server instance>.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The database in which the objects that you want to compile are stored.
        /// </summary>
        public string Database { get; set; }
        
        /// <summary>
        /// The path of the Microsoft Dynamics NAV Development Environment executable. 
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Enable the test mode; 
        /// </summary>
        public bool TestMode { get; set; }

        /// <summary>
        /// Gets the major version of the Microsoft Dynamics NAV Development Environment executable
        /// </summary>
        public int MajorVersion
        {
            get { return TestMode ? 0 : Utilities.GetVersion(this).FileMajorPart; }
        }

        /// <summary>
        /// Gets the minor version of the Microsoft Dynamics NAV Development Environment executable
        /// </summary
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

        /// <summary>
        /// Specifies the name of the server that hosts the Microsoft Dynamics NAV Server instance, such as MyServer.
        /// </summary>
        public string NavServerName { get; set; }

        /// <summary>
        /// Specifies the Microsoft Dynamics NAV Server instance that is being used, such as DynamicsNAV80.
        /// </summary>
        public string NavServerInstance { get; set; }

        /// <summary>
        /// Specifies the port on the Microsoft Dynamics NAV Server server that the Microsoft Dynamics NAV Windows PowerShell cmdlets access, such as 7045.
        /// </summary>
        public string NavServerManagementPort { get; set; }

    }
}
