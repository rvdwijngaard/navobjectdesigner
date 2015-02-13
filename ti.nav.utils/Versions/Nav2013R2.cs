﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils.Versions
{
    public class Nav2013R2 : GenericObjectDesigner
    {
        public Nav2013R2(IObjectDesignerConfig request, ICommandRunner commandRunner) : base(request, commandRunner) { }

        internal override string ImportCommand(string command)
        {
            return command += ",validatetablechanges=0";
        }

        internal override string CompileCommand(string command)
        {
            return command += ",validatetablechanges=0";
        }
    }
}
