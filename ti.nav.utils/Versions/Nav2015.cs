using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils.Versions
{
    public class Nav2015 : GenericObjectDesigner
    {
        private IObjectDesignerConfig mConfig;

        public Nav2015(IObjectDesignerConfig config, ICommandRunner commandRunner) : base(config, commandRunner) 
        {
            mConfig = config;
        }
        
        internal override string ImportCommand(string command)
        {
            return Nav2015Command(command);
        }

        internal override string CompileCommand(string command)
        {
            return Nav2015Command(command);            
        }

        private string Nav2015Command(string command)
        {
            if (!string.IsNullOrEmpty(mConfig.NavServerName))
            {
                return command += string.Format(
                    ",synchronizeschemachanges=force" +
                    ",navservername={0}" +
                    ",navserverinstance={1}" +
                    ",navservermanagementport={2}", mConfig.NavServerName, mConfig.NavServerInstance, mConfig.NavServerManagementPort);
            }
            return command + ",synchronizeschemachanges=no";
        }
    }
}
