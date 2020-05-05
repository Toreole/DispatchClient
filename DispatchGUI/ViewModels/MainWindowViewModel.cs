using DispatchGUI.Services;
using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Reactive.Linq;
using DispatchGUI.Models;
using System.Runtime.Serialization;
using System.Reactive;
using ReactiveUI;

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
            ConsoleOutputText += "Yo";
        }

    }
}
