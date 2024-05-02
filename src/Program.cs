using EQTool.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Documents;

namespace EQTool
{
    public class Program
    {
#if QUARM
        private const string programName = "QuarmTool.exe";
#else
        private const string programName = "EQTool.exe";
#endif
        private static string configFile = $"{programName}.config";

        [STAThread]
        public static void Main(string[] args)
        {
            var debug = false;
#if DEBUG
            debug = true;
#endif
            try
            {
                if (!debug)
                {
                    _ = OnResolveAssembly(null, new ResolveEventArgs("System.Threading.Tasks.Extensions"));
                    AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
                    if (!File.Exists(configFile))
                    {
                        UpdateConfig(args);
                        Thread.Sleep(1000);
                        return;
                    }
                    else
                    {
                        var fileondisk = File.ReadAllText(configFile);
                        if (fileondisk != Resources.App)
                        {
                            UpdateConfig(args);
                            Thread.Sleep(1000);
                            return;
                        }
                    }
					EnsureResourcesExist();

					DapperExtensions.DapperExtensions.SetMappingAssemblies(new[] { Assembly.GetExecutingAssembly() });
					DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.SqliteDialect();

				}

                App.Main();
            }
            catch (Exception ex)
            {
                File.AppendAllText("Errors.txt", ex.ToString());
                throw;
            }
        }

        private static void UpdateConfig(string[] args)
        {
            File.WriteAllText(configFile, Resources.App);
            var path = System.IO.Directory.GetCurrentDirectory() + $"/{programName}";
            _ = System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = path,
                Arguments = args.FirstOrDefault(),
                UseShellExecute = true
            });
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyName = new AssemblyName(args.Name);
            var path = assemblyName.Name + ".dll";
            var asses = executingAssembly.GetManifestResourceNames();
            Debug.WriteLine($"Try Resolve {args.Name} {asses.Count()}");
            using (var stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                {
                    return null;
                }

                var assemblyRawBytes = new byte[stream.Length];
                _ = stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                if (args.Name.StartsWith("System.Threading.Tasks.Extensions"))
                {
                    try
                    {
                        File.WriteAllBytes("System.Threading.Tasks.Extensions.dll", assemblyRawBytes);
                    }
                    catch { }
                }
                return Assembly.Load(assemblyRawBytes);
            }
        }

		private static bool EnsureResourcesExist()
		{
			string baseFolder = AppContext.BaseDirectory;
			List<string> files = new List<string>()
			{
				"x64\\SQLite.Interop.dll",
				"x86\\SQLite.Interop.dll"
			};

			if (!File.Exists(Path.Combine(baseFolder, "System.Data.SQLite.dll")))
			{
				using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("EQTool.Embeds.System.Data.SQLite.dll"))
				{
					if (resource == null)
					{
						return false;
					}
					using (var fileStream = File.Create($"{baseFolder}\\System.Data.SQLite.dll"))
					{
						resource.CopyTo(fileStream);
					}
				}
			}
			foreach(string file in files)
			{
				if (!File.Exists(Path.Combine(baseFolder, file)))
				{
					using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"EQTool.Embeds.{file.Replace("\\", ".")}"))
					{
						if (resource == null)
						{
							return false;
						}
						FileInfo fi = new FileInfo(Path.Combine(baseFolder, file));
						if (!fi.Directory.Exists)
						{
							Directory.CreateDirectory(fi.DirectoryName);
						}

						using (var fileStream = File.Create(Path.Combine(baseFolder, file)))
						{
							resource.CopyTo(fileStream);
						}
					}
				}
			}

			return true;
		}
    }
}
