using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public class ExportRequest
    {
        public string Filter { get; set; }
        public string FileName { get; set; }
    }

    public class ExportResponse
    {
        public Exception Exception { get; set; }
        public bool Succesful { get; set; }
    }
}
