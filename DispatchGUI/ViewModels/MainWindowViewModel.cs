﻿using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Drawing;
using DispatchGUI.Services;
using System.Threading.Tasks;

namespace DispatchGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        string workingDirectory;
        string workingProjectFile;

        public ConfigViewModel ConfigView { get; private set; }

        #region Constructors_Initialization
        public MainWindowViewModel()
        {
            Run = ReactiveCommand.Create(() => { Task.Run(RunTestClient); });
            ConfigView = new ConfigViewModel();
        }

        //existing file, initialize.
        public MainWindowViewModel FromFile(string filePath)
        {
            this.workingProjectFile = filePath;
            this.workingDirectory = System.IO.Path.GetDirectoryName(filePath);
            ConfigHost.Reload(filePath);
            return this;
        }
        //only got the path: 1. search for file, if none found create new.
        public MainWindowViewModel FromDirectory(string path)
        {
            this.workingDirectory = path;
            this.workingProjectFile = ""; //mark empty to begin with.

            foreach(string file in Directory.GetFiles(path + "/"))
            {
                if(file.EndsWith(".disgui")) //this is a disgui file.
                {
                    this.workingProjectFile = file;
                    break;
                }
            }

            //no file could be found, create a new one
            if(string.IsNullOrEmpty(this.workingProjectFile))
            {
                this.workingProjectFile = ConfigHost.CreateNewIn(path);
            } 
            else
            {
                ConfigHost.Reload(this.workingProjectFile);
            }

            return this;
        }
        #endregion

        ReactiveCommand<Unit, Unit> Test { get; }
        ReactiveCommand<Unit, Unit> Run { get; }

        public string ConsoleOutputText { get => outputText; set => this.RaiseAndSetIfChanged(ref outputText, value); } 
        private string outputText = "Hello";

        //TODO: these commands should be run as >async< (Task.Run).
        void RunTestClient()
        {
            //1. Check for / Setup file.
            string path = Path.GetFullPath("command.bat");
            if(!File.Exists(path))
            {
                FileStream stream = File.Create(path);
                byte[] buffer = Encoding.UTF8.GetBytes("dispatch help");
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
            }
            //create the process.
            Process process = new Process();
            
            ProcessStartInfo startInfo = new ProcessStartInfo(path, "");
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, args) => ConsoleOutputText += $"{args.Data}\n";
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            //CLEANER: this does not produce a temporary file!
            //System.Diagnostics.Process process = new System.Diagnostics.Process();
            //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo.FileName = "cmd.exe";
            //                       /C indicates that the argument is a command which cmd will run and then close.
            //startInfo.Arguments = "/C copy /b Image1.jpg + Archive.rar Image2.jpg";
            //process.StartInfo = startInfo;
            //process.Start();
        }
    }
}
