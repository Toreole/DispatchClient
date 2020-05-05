using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using DispatchGUI.Models;
using DispatchGUI.Services;

namespace DispatchGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public MainWindowViewModel(TodoDatabase db)
        {
            Content = List = new TodoListViewModel(db.GetItems());
            Test = ReactiveCommand.Create(() => List.Test());
            Run = ReactiveCommand.Create(() => RunTestClient());
        }

        ReactiveCommand<Unit, Unit> Test { get; }
        ReactiveCommand<Unit, Unit> Run { get; }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        [DataMember]
        public TodoListViewModel List { get; }

        public string ConsoleOutputText { get => outputText; set => this.RaiseAndSetIfChanged(ref outputText, value); } 
        private string outputText = "Hello";

        public void AddItem()
        {
            var vm = new AddItemViewModel();

            Observable.Merge(vm.Ok, vm.Cancel.Select(_ => (TodoItem)null))
                .Take(1)
                .Subscribe(todoItem =>
                {
                    if (todoItem != null)
                    {
                        List.Items.Add(todoItem);
                    }
                    Content = List;
                });

            Content = vm;
        }

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
