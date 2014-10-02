using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public class Nav2013R2 : GenericObjectDesigner
    {
        public Nav2013R2(IObjectDesignerRequest request, ICommandRunner commandRunner) : base(request, commandRunner) { }

        internal override string ImportCommand(string command)
        {
            return command += ",validatetablechanges=0";
        }
        
    }
}
