using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TI.Nav.Utils
{
    public interface IObjectDesigner
    {
        ExportResponse Export(ExportRequest request);                
        ImportResponse Import(ImportRequest request);
        CompileResponse Compile(CompileRequest request);        
    }
}
