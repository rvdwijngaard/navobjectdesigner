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
            var queue = new ConcurrentQueue<string>();
            var response = new ImportResponse() { Successful = true };
            var options = new ParallelOptions() { };

            Action<string> import = (string f) =>
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
                catch (ObjectDesignerDeadlockException dle)
                {
                    Log.Warning(dle, "Deadlock exception occurred; {@item} will be retried later", dle.Source);
                    queue.Enqueue(dle.Source);
                }
            };

            Log.Verbose("Import all items in parallel");
            Parallel.ForEach<string>(config.Files, options, import);

            Log.Verbose("Retry {@count} items with a deadlock exception", queue.Count());
            queue.ToList().ForEach(import);

            return response;
        }

        internal virtual string CompileCommand(string command) { return command; }

        public CompileResponse Compile(CompileRequest request)
        {
            Log.Verbose("Compile all objects for request {@request}", request);
            
            var response = new CompileResponse() { Successful = true };
            var types = new List<string> { "Table", "Codeunit", "Page", "Report", "XMLport", "Query" };
            string result = null;
            
            Action<string> compile = (string t) =>
                {
                    try
                    {
                        CompileObjectsForType(request, string.Format("Type={0}", t));
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

                            var ex = new CompilationException(string.Format("{0} {1}", source.Item1, source.Item2), GetValueFromMatch(m.Groups, "message"));
                            response.Exceptions.Add(ex);
                            Log.Verbose(ex, "Compilation errors for object {@type} {@id}", source.Item1, source.Item2);
                        }
                    }
                };

            Parallel.ForEach<string>(types, compile);
            Log.Verbose("Compile all objects for {@request} completed", request);

            return response;
        }

        private void CompileObjectsForType(CompileRequest config, string Typefilter)
        {
            string filter = !string.IsNullOrEmpty(config.Filter) ? string.Format("{0};{1}", config.Filter, Typefilter) : Typefilter;
            string cmd = CompileCommand(
                    string.Format("command=compileobjects, servername={0},database={1},filter={2}", mConfig.Server, mConfig.Database, filter));
            Log.Verbose("Compile the objects with filter {@filter}", filter);
            RunCommand(null, cmd);
        }

        private string GetValueFromMatch(GroupCollection groups, string key)
        {
            return groups[key] == null ? string.Empty : groups[key].Value;
        }

        #region private methods
        private void ImportFile(string fileName)
        {
            var command = ImportCommand(string.Format("command=importobjects, file=\"{0}\",servername={1},database={2},importaction=overwrite", fileName, mConfig.Server, mConfig.Database));
            RunCommand(fileName, command);
        }

        private void RunCommand(string source, string arguments)
        {
            Log.Verbose("Run finsql command with arguments {@arguments} for source {@source}", arguments, source);
            ProcessCommandResult(source, mCommandRunner.RunCommand(mConfig, arguments));
        }

        private void ProcessCommandResult(string source, string message)
        {
            // License warning which should be ignored
            if (!string.IsNullOrEmpty(message) && !message.StartsWith("[18023763]"))
            {
                if (message.StartsWith("[22926089]") || message.StartsWith("[22926090]"))
                    throw new ObjectDesignerDeadlockException(source, message);
                throw new ObjectDesignerException(source, message);
            }

        }

        #endregion
    }
}
