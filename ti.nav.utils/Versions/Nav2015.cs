﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils.Versions
{
    public class Nav2015 : GenericObjectDesigner
    {        
        public Nav2015(IObjectDesignerConfig request, ICommandRunner commandRunner) : base(request, commandRunner) { }
        
        internal override string ImportCommand(string command)
        {
            return command += ",synchronizeschemachanges=force"; 
        }

        internal override string CompileCommand(string command)
        {
            return command += ",synchronizeschemachanges=no"; // todo: make this configurable
        }
    }
}
