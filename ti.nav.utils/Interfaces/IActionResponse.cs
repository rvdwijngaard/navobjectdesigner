using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils.Interfaces
{
    public interface IActionResponse
    {
        List<Exception> Exceptions { get; set; }
        bool Successful { get; set; }
    }
}
