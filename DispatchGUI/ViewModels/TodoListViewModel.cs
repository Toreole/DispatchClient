using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DispatchGUI.Models;

namespace DispatchGUI.ViewModels
{
    public class TodoListViewModel : ViewModelBase
    {
        public TodoListViewModel(IEnumerable<TodoItem> items)
        {
            Items = new ObservableCollection<TodoItem>(items);
        }

        public ObservableCollection<TodoItem> Items { get; }
    }
}
