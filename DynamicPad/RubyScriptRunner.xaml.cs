using System;
using IronRuby;
using Massive;
using Microsoft.Scripting.Hosting;

namespace DynamicPad
{
    public class RubyScriptRunner
    {
        private const string OverrideRubyObject =
            @"
require 'dynamicpad.exe'
connString = ""{0}""
class Object
    def puts(str)
        log.Print(str)
    end

    def p(str)
        log.Print(str)
    end

    def clear()
        log.Clear
    end
    def Dump(obj)
        log.Dump(obj)
    end

    def DumpE(obj)
        log.DumpEnumerable(obj)
    end
end";
        public void RunRubyScript(ScriptArguments scriptArguments)
        {
            try
            {
                ScriptRuntime runtime = Ruby.CreateRuntime();
                ScriptEngine engine = runtime.GetEngine("IronRuby");

                ScriptScope scriptScope = engine.CreateScope();

                AddTbl(scriptScope, scriptArguments.ConnectionString);

                scriptScope.SetVariable("log", scriptArguments.Logger);
                var initialScript = string.Format(OverrideRubyObject, scriptArguments.ConnectionString.Replace("\\", "\\\\"));
                engine.Execute(initialScript, scriptScope);
                engine.Execute(scriptArguments.Script, scriptScope);
            }
            catch (Exception exception)
            {
                scriptArguments.Logger.Print("\n--- Exception -------------------------------------------\n");
                scriptArguments.Logger.Print(exception.ToString());
                scriptArguments.Logger.Print("\n--- Exception end----------------------------------------\n");
            }
        }

        private static void AddTbl(ScriptScope scriptScope, string connectionString)
        {
            var tbl = new DynamicModel(connectionString);
            scriptScope.SetVariable("tbl", tbl);
        }
    }
}