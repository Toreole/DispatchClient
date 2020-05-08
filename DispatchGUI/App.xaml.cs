using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DispatchGUI.ViewModels;
using DispatchGUI.Views;
using System.IO;

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
                MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();
                //filter the command line arguments
                var commandArguments = System.Environment.GetCommandLineArgs();
                if (commandArguments.Length > 0)
                {
                    foreach (string arg in commandArguments)
                    {
                        //TODO: this doesnt correctly parse all the arguments.
                        if (!Path.IsPathRooted(arg)) //skip arguments that arent paths.
                            continue;
#nullable enable
                        string? extension = Path.GetExtension(arg);
                        if (extension != ".disgui")
                            if (extension != null)
                                continue;
#nullable disable
                            //there is an argument, currently that should only be the case if it gets passed a path!
                            string path = commandArguments[0];
                        if (path.EndsWith(".disgui"))  //identified as file
                            path = Path.GetDirectoryName(path);

                        //always load from the directory, even when its a bit slower than going directly from the file.
                        mainWindowViewModel.FromDirectory(path);
                    }
                }

                //make the main window.
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
