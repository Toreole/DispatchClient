using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using System.Drawing;
using DispatchGUI.Services;

namespace DispatchGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;
        string workingDirectory;
        string workingProjectFile;

        public ConfigViewModel ConfigView { get; private set; }

        public MainWindowViewModel()
        {
            Run = ReactiveCommand.Create(() => RunTestClient());
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

            return this;
        }

        ReactiveCommand<Unit, Unit> Test { get; }
        ReactiveCommand<Unit, Unit> Run { get; }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public string ConsoleOutputText { get => outputText; set => this.RaiseAndSetIfChanged(ref outputText, value); } 
        private string outputText = "Hello";

        void RunTestClient()
        {
            //TODO: this will run the .bat file eventually.
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
        }
    }
}
