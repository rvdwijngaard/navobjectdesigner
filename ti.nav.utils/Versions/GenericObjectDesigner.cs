using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using TI.Nav.Utils.Exceptions;

namespace TI.Nav.Utils.Versions
{
    public abstract class GenericObjectDesigner : IObjectDesigner
    {
        private IObjectDesignerConfig mConfig;
        private ICommandRunner mCommandRunner;

        internal GenericObjectDesigner(IObjectDesignerConfig config, ICommandRunner commandRunner)
        {
            mConfig = config;
            mCommandRunner = commandRunner;
        }

        public ExportResponse Export(ExportRequest config)
        {
            Log.Verbose("Exporting for config {@config}", config);
            string cmd = string.Format("command=exportobjects, file=\"{0}\",servername={1},database={2},filter={3}", config.FileName, mConfig.Server, mConfig.Database, config.Filter);
            string result = string.Empty;

            var response = new ExportResponse() { Successful = true };
            try
            {
                RunCommand(config.Filter, cmd);
            }
            catch (ObjectDesignerException ex)
            {
                Log.Verbose(ex, "Compilation Errors");
                response.Exceptions.Add(ex);
                response.Successful = false;
            }
            return response;
        }

        internal virtual string ImportCommand(string command) { return null; }

        public ImportResponse Import(ImportRequest config)
        {
            var response = new ImportResponse() { Successful= true };

            // process the stuff in parallel 
            Parallel.ForEach<string>(config.Files, f =>
            {
                try
                {
                    ImportFile(f);
                }
                catch (ObjectDesignerException ex)
                {
                    response.Exceptions.Add(ex);
                    response.Successful = false;
                }
            });
            return response;
        }

        public CompileResponse Compile(CompileRequest config)
        {
            string cmd = string.Format("command=compileobjects, servername={0},database={1},filter={2}", mConfig.Server, mConfig.Database, config.Filter);
            string result = null;
            var response = new CompileResponse() { Successful = true };
            try
            {
                RunCommand(null, cmd);
            }
            catch (ObjectDesignerException ex)
            {
                Log.Verbose(ex, "Compilation Errors");
                response.Successful = false;
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
            var command = ImportCommand(string.Format("command=importobjects, file=\"{0}\",servername={1},database={2}", fileName, mConfig.Server, mConfig.Database));
            RunCommand(fileName, command);
        }

        private string RunCommand(string source, string arguments)
        {
            string result = mCommandRunner.RunCommand(mConfig, arguments);
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
