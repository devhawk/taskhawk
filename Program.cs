using System;
using System.Collections.Generic;
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

            var psi = new ProcessStartInfo()
            {
                FileName = filename,
                Arguments = args,
                CreateNoWindow = false,
                WorkingDirectory = cwd
            };
            
            var process = System.Diagnostics.Process.Start(psi);
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
                        console.WriteLine("task failed");
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                console.WriteLine(ex.Message);
                app.ShowHelp();
                return 1;
            }            

            console.WriteLine("Hello, WOrld!");
            return 0;
        }

        // static void Main(string[] args)
        // {
        //     public static int Main(string[] args)
        //         => CommandLineApplication.Execute<Program>(args);

        //     var rootCommand = new RootCommand
        //     {
        //         new Option<FileInfo>(
        //             "--task-file",
        //             "An option whose argument is parsed as a FileInfo")
        //     };

        //     rootCommand.Handler = CommandHandler.Create<FileInfo>((fileOption) =>
        //     {
        //         if (fileOption != null)
        //         Console.WriteLine($"The value for --int-option is: {intOption}");
        //         Console.WriteLine($"The value for --bool-option is: {boolOption}");
        //         Console.WriteLine($"The value for --file-option is: {fileOption?.FullName ?? "null"}");
        //     });

        //     foreach (var command in commands)
        //     {
        //         if (!ExecuteCommand(command))
        //         {
        //             break;
        //         }
        //     }
        // }
    }
}
