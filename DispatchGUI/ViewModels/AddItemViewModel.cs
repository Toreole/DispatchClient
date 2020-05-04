using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Reactive;
using DispatchGUI.Models;

namespace DispatchGUI.ViewModels
{
    public class AddItemViewModel : ViewModelBase
    {
        string description;
        public string Description 
        { 
            get => description; 
            set => this.RaiseAndSetIfChanged(ref description, value); 
        }

        public ReactiveCommand<Unit, TodoItem> Ok { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        public AddItemViewModel()
        {
            var okEnabled = this.WhenAnyValue(
                x => x.Description,
                x => !string.IsNullOrWhiteSpace(x));

            Ok = ReactiveCommand.Create(
                () => new TodoItem { Description = this.Description },
                okEnabled);
            Cancel = ReactiveCommand.Create(() => { });
        }
    }
}
