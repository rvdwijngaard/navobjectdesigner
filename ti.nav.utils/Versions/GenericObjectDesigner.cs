﻿using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

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

        public ExportResponse Export(ExportRequest request)
        {
            Log.Verbose("Exporting for request {@request}", request);
            string cmd = string.Format("command=exportobjects, file=\"{0}\",servername={1},database={2},filter={3}", request.FileName, mRequest.Server, mRequest.Database, request.Filter);
            string result = string.Empty;

            var response = new ExportResponse() { Succesful = true };
            try
            {
                RunCommand(request.Filter, cmd);
            }
            catch (ObjectDesignerException ex)
            {
                Log.Verbose(ex, "Compilation Errors");
                response.Exception = ex;
                response.Succesful = false;
            }
            return response;
        }

        internal virtual string ImportCommand(string command) { return null; }

        public ImportResponse Import(ImportRequest request)
        {
            var response = new ImportResponse() { Result = true };

            // process the stuff in parallel 
            Parallel.ForEach<string>(request.Files, f =>
            {
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

        public CompileResponse Compile(CompileRequest request)
        {
            string cmd = string.Format("command=compileobjects, servername={0},database={1},filter={2}", mRequest.Server, mRequest.Database, request.Filter);
            string result = null;
            var response = new CompileResponse() { Succesful = true };
            try
            {
                RunCommand(null, cmd);
            }
            catch (ObjectDesignerException ex)
            {
                Log.Verbose(ex, "Compilation Errors");
                response.Succesful = false;
                result = ex.Message;
            }

            if (!String.IsNullOrEmpty(result))
            {
                const string expr = @"(\[\d*\](?<message>.*?)--) Object:\s(?<type>\w*)\s(?<id>\d*)\s(?<name>(\s|\w)*)";
                foreach (Match m in Regex.Matches(result, expr, RegexOptions.Singleline))
                {
                    Tuple<string, string> source = new Tuple<string, string>(GetValueFromMatch(m.Groups, "type"), GetValueFromMatch(m.Groups, "id"));

                    var ex = new CompilationException(string.Format("{0} {1}", source.Item1, source.Item2), GetValueFromMatch(m.Groups, "errortext"));
                    response.Exceptions.Add(ex);
                    Log.Verbose(ex, "Compilation errors for object {@type} {@id}", source.Item1, source.Item2);
                }
            }
            return response;
        }

        private string GetValueFromMatch(GroupCollection groups, string key)
        {
            return groups[key] == null ? string.Empty : groups[key].Value;
        }

        #region private methods
        private void ImportFile(string fileName)
        {
            var command = ImportCommand(string.Format("command=importobjects, file=\"{0}\",servername={1},database={2}", fileName, mRequest.Server, mRequest.Database));
            RunCommand(fileName, command);
        }

        private string RunCommand(string source, string arguments)
        {
            string result = mCommandRunner.RunCommand(mRequest, arguments);
            if (IsNavErrorMessage(result))
            {
                Log.Error("An error {@message} occured while executing the finsql command with arguments {@arguments}", result, arguments);
                throw new ObjectDesignerException(source, result);
            }
            return result;
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
