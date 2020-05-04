using DispatchGUI.Services;
using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Reactive.Linq;
using DispatchGUI.Models;
using System.Runtime.Serialization;
using System.Reactive;

namespace DispatchGUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public MainWindowViewModel(TodoDatabase db)
        {
            Content = List = new TodoListViewModel(db.GetItems());
            Test = ReactiveCommand.Create(() => List.Test());
        }

        ReactiveCommand<Unit, Unit> Test { get; }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        [DataMember]
        public TodoListViewModel List { get; }

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

    }
}
