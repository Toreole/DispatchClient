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
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Avalonia.Controls;

namespace DispatchGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ConfigViewModel ConfigView { get; private set; }
        public ReactiveCommand<Unit, Unit> GenerateConfigJson { get; }

        public void TestSave()
        {
            Console.Write("Test");
        }

        #region Constructors_Initialization
        public MainWindowViewModel()
        {
            ConfigHost.Empty();
            ConfigView = new ConfigViewModel();
        }

        //existing file, initialize.
        public MainWindowViewModel FromFile(string filePath)
        {
            ConfigHost.workingProjectFile = filePath;
            ConfigHost.workingDirectory = System.IO.Path.GetDirectoryName(filePath);
            ConfigHost.Reload(filePath);
            return this;
        }
        //only got the path: 1. search for file, if none found create new.
        public MainWindowViewModel FromDirectory(string path)
        {
            ConfigHost.workingDirectory = path;
            ConfigHost.workingProjectFile = ""; //mark empty to begin with.

            foreach(string file in Directory.GetFiles(path + "/"))
            {
                if(file.EndsWith(".disgui")) //this is a disgui file.
                {
                    ConfigHost.workingProjectFile = file;
                    break;
                }
            }

            //no file could be found, create a new one
            if(string.IsNullOrEmpty(ConfigHost.workingProjectFile))
            {
                ConfigHost.workingProjectFile = ConfigHost.CreateNewIn(path);
            } 
            else
            {
                ConfigHost.Reload(ConfigHost.workingProjectFile);
            }

            return this;
        }

        public void Initialize()
        {
            if (ConfigHost.ActiveConfig == null)
                return;
            ConfigView.AppID = ConfigHost.ActiveConfig.applicationID;
        }
        #endregion

        ReactiveCommand<Unit, Unit> Test { get; }
        ReactiveCommand<Unit, Unit> Run { get; }

        public string ConsoleOutputText { get => outputText; set => this.RaiseAndSetIfChanged(ref outputText, value); } 
        private string outputText = "Hello";
    }
}
