using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LibrettoUI_2;

internal class ProcessLauncher
{

    StringBuilder stdoutBuffer = new StringBuilder();

    public List<String> StatusMessage = new List<string>();

        //public string? StandardError;
    public int ExitCode = 0;

    static public string GetLibrettoXCommandLineArgs(string templateFile, string schemaFile, string outputPath)
    {
        return $"librettox.py -t \"{templateFile}\" -s \"{schemaFile}\" -o \"{outputPath}\"";
    }

    public void LaunchProcess(string command, string arguments, bool wait = false)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();

        stdoutBuffer.Clear();

        Stopwatch sw = new Stopwatch();

        sw.Start();

        Console.WriteLine("Running commands {0} {1}...", command, arguments);

        startInfo.FileName = command;
        startInfo.Arguments = arguments;

        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;

        process.EnableRaisingEvents = true;

        process.OutputDataReceived += (sender, args) => {
            if (! String.IsNullOrEmpty(args.Data))
            {
                this.StatusMessage.Add(args.Data);
            }
        };

        process.StartInfo = startInfo;


        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();

            this.ExitCode = process.ExitCode;
             
            sw.Stop();
        }
        catch (Exception ex)
        {
            this.StatusMessage.Add(ex.Message);
        }
        finally
        {
            process.Close();
            process.Dispose();
        }

        string minutes = sw.ElapsedMilliseconds < 60000 ? " < 1 min" : (sw.ElapsedMilliseconds / 60000).ToString("#,### mins");

        return;
    }
}

