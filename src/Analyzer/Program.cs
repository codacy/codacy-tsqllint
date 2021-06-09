using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using System;
using System.IO;

namespace Codacy.TSQLLint
{
    internal static class Program
    {
        [ExcludeFromCodeCoverage]
        public static int Main()
        {
            Program.HandleMissingAssemblies();

            using (var analyzer = new CodeAnalyzer())
            {
                analyzer.Run()
                    .GetAwaiter().GetResult();
            }

            return 0;
        }

        public static void HandleMissingAssemblies()
        {
            // https://developer.samsung.com/tizen/blog/en-us/2020/02/17/assembly-loading-problem-in-tizen-net-applications
            AppDomain.CurrentDomain.AssemblyResolve += (object s, ResolveEventArgs eventArgs) =>
            {
                var appDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var assemblyName = eventArgs.Name.Split(',')[0];
                var assemblyPath = Path.Combine(appDir, assemblyName + ".dll");
                return File.Exists(assemblyPath) ? Assembly.LoadFile(assemblyPath) : null;
            };

        }
    }

}
