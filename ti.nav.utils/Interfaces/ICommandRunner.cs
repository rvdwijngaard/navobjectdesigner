using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public interface ICommandRunner
    {
        string RunCommand(IObjectDesignerConfig request, string command);
    }
}
