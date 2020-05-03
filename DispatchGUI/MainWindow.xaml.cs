using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace DispatchGUI
{
    public class MainWindow : Window
    {
        private Button testButton;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            testButton = this.FindControl<Button>("TestButton");
            testButton.IsEnabled = true;
            testButton.Click += OnClickTest;
        }

        public void OnClickTest(object sender, RoutedEventArgs args)
        {
            testButton.Content = "yikes";
            testButton.IsEnabled = false;
        }
    }
}
