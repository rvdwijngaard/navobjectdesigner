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
            var response = new ExportResponse() { Successful = true };

            if (request.Filter == "error")
            {
                response.Successful = false;
                response.Exceptions.Add(new ObjectDesignerException("source", "error text"));
            }
            return response;     
        }

        public ImportResponse Import(ImportRequest request)
        {
            var response = new ImportResponse() { Successful = true};

            if (request.Files.Contains("error"))
            {
                response.Successful = false;
                response.Exceptions.Add(new ObjectDesignerException("source", "error text"));
            }
            return response;            
        }

        public CompileResponse Compile(CompileRequest request)
        {
            var response = new CompileResponse() { Successful = true };

            if (request.Filter == "error")               
            {
                response.Successful = false;
                response.Exceptions.Add(new ObjectDesignerException("source", "error text"));
            }
            return response;            
        }
    }
}
