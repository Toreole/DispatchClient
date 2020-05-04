using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using DispatchGUI.Models;

namespace DispatchGUI.ViewModels
{
    public class TodoListViewModel : ViewModelBase
    {
        public TodoListViewModel(IEnumerable<TodoItem> items)
        {
            Items = new ObservableCollection<TodoItem>(items);
        }

        public void Test()
        {
            Items.Add(new TodoItem() { Description = "This has been added later with a Button." });
        }

        public ObservableCollection<TodoItem> Items { get; }
    }
}
