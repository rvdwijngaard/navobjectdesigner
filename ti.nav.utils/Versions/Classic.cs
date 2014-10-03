using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TI.Nav.Utils
{
    public class Classic : IObjectDesigner
    {
        public Classic(IObjectDesignerRequest request)
        {

        }       

        public ImportResponse Import(ImportRequest request)
        {
            throw new NotImplementedException();
        }

        public CompileResponse Compile(CompileRequest request)
        {
            throw new NotImplementedException();
        }

        public ExportResponse Export(ExportRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
