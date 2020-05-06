using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DispatchGUI.ViewModels;
using DispatchGUI.Views;

namespace DispatchGUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //TODO: this todo database stems from the avalonia tutorial and all connected resources can be removed.
                var db = new Services.TodoDatabase();
                //filter the command line arguments
                var commandArguments = System.Environment.GetCommandLineArgs();
                if(commandArguments.Length > 0)
                {
                    //there is an argument, currently that should only be the case if it gets passed a path!
                    string path = commandArguments[0];
                    if(path.EndsWith(".disgui"))
                    {
                        //identified as file
                    }
                    else
                    {
                        //is a directory.
                    }
                }
                //make the main window.
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(db),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
