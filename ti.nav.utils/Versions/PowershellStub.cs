using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TI.Nav.Utils.Exceptions;

namespace TI.Nav.Utils.Versions
{
    public class PowershellStub : IObjectDesigner
    {
        public PowershellStub(IObjectDesignerConfig request)
        {
        }              

        public ExportResponse Export(ExportRequest request)
        {
            Log.Information("Export {@request}",request);
            var response = new ExportResponse() { Successful = true };
            
            if (request.Filters != null && request.Filters.FirstOrDefault().Filter == "error")
            {
                response.Successful = false;
                var ex = new ObjectDesignerException("source", "error text");
                response.Exceptions.Add(ex);
                Log.Error(ex, "Export exception");
            }
            return response;     
        }

        public ImportResponse Import(ImportRequest request)
        {
            Log.Information("Import {@request}", request);
            var response = new ImportResponse() { Successful = true};

            if (request.Files.Contains("error"))
            {
                response.Successful = false;
                var ex = new ObjectDesignerException("source", "error text");
                response.Exceptions.Add(ex);
                Log.Error(ex,"Import exception");
            }
            return response;            
        }

        public CompileResponse Compile(CompileRequest request)
        {
            Log.Information("Compile {@request}", request);
            var response = new CompileResponse() { Successful = true };

            if (request.Filter == "error")               
            {
                response.Successful = false;
                var ex = new ObjectDesignerException("source", "error text");
                response.Exceptions.Add(ex);
                Log.Error(ex, "Compile exception");
            }
            return response;            
        }
    }
}
