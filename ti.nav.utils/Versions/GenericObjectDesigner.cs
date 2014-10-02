using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TI.Nav.Utils
{
    public abstract class GenericObjectDesigner : IObjectDesigner
    {
        private IObjectDesignerRequest mRequest;
        private ICommandRunner mCommandRunner;

        internal GenericObjectDesigner(IObjectDesignerRequest request, ICommandRunner commandRunner)
        {
            mRequest = request;
            mCommandRunner = commandRunner;
        }

        public void Export(string filter, string fileName)
        {
            throw new NotImplementedException();
        }

        public void Import(List<string> files, object[] args)
        {
            // run the finsql with the command line options
            // command line options may be version specific            
        }

        internal virtual string ImportCommand(string command) { return null; }

        public ImportResponse Import(ImportRequest request)
        {
            var response = new ImportResponse() { Result = true };  
          
            // process the stuff in parallel 
            Parallel.ForEach<string>(request.Files, f => {                
                try
                {
                    ImportFile(f);
                }
                catch (ObjectDesignerException ex)
                {
                    response.Exceptions.Add(ex);
                    response.Result = false;
                }                
            });            
            return response;
        }

        private void ImportFile(string fileName)
        {
            var command = ImportCommand(string.Format("command=importobjects, file=\"{0}\",servername={1},database={2}", fileName, mRequest.Server, mRequest.Database));
            RunCommand(fileName, command);
        }

        public void Compile()
        {
            throw new NotImplementedException();
        }

        #region private methods
        private void RunCommand(string source, string arguments)
        {
            string message = mCommandRunner.RunCommand(mRequest, arguments);
            if (IsNavErrorMessage(message))
            {
                Log.Error("An error {@message} occured while executing the finsql command with arguments {@arguments}", message, arguments);
                throw new ObjectDesignerException(source, message);
            }
        }

        private bool IsNavErrorMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Log.Warning("The Microsoft Dynamics NAV development environment returned {@warning}", message);
                // License warning which should be ignored
                if (message.StartsWith("[18023763]"))
                    return false;

                return true;
            }
            return false;
        }

        

        #endregion
    }
}
