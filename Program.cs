using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace DevHawk.TaskHawk
{
    class Program
    {
        static bool ExecuteCommand(string command, string cwd = "")
        {
            var spacePos = command.IndexOf(' ');
            var (filename, args) = spacePos == -1
                ? (command, string.Empty)
                : (command.Substring(0, spacePos), command.Substring(spacePos).Trim());

            var startInfo = new ProcessStartInfo(filename)
            {
                Arguments = args,
                CreateNoWindow = false,
                UseShellExecute = false,
                WorkingDirectory = cwd,
            };

            var process = System.Diagnostics.Process.Start(startInfo);
            process.WaitForExit();
            return process.ExitCode == 0;
        }

        static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        [Argument(0)]
        [Required]
        private string TaskFile { get; } = string.Empty;

        private async Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
        {
            try
            {
                if (!File.Exists(TaskFile))
                {
                    throw new Exception($"{TaskFile} does not exist");
                }

                var cwd = Path.GetDirectoryName(TaskFile);
                var tasks = await File.ReadAllLinesAsync(TaskFile);

                foreach (var task in tasks)
                {
                    console.WriteLine(task);
                    if (!ExecuteCommand(task, cwd ?? string.Empty))
                    {
                        return 1;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
