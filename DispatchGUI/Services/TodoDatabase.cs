using System;
using System.Collections.Generic;
using System.Text;
using DispatchGUI.Models;

namespace DispatchGUI.Services
{
    public class TodoDatabase
    {
        public IEnumerable<TodoItem> GetItems() => new[]
        {
            new TodoItem{Description = "Run"},
            new TodoItem{Description = "reeee"},
            new TodoItem{Description="lmao"}
        };
    }
}
