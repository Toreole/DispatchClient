using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DispatchGUI.Services
{
    static class CmdService
    {
        public static string ExecuteDispatchCommand(string command)
        {
            return ExecuteCommand($"dispatch {command}");
        }
        public static string ExecuteCommand(string command)
        {
            string output = "";

            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            //set the command
            startInfo.Arguments = $"/C {command}";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            //setup output
            process.OutputDataReceived += (sender, data) => { output += $"{data.Data}\n"; };
            process.StartInfo = startInfo;
            //read output
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            return output;
        }

    }
}
