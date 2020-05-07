using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DispatchGUI.Services;
using System;
using System.IO;
using System.Reactive;
using System.Text.RegularExpressions;

namespace DispatchGUI.Views
{
    public class ConfigView : UserControl
    {
        TextBox appID_Input;
        TextBox configPath_Input;
        Button configButton;

        public ConfigView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);


            appID_Input = this.FindControl<TextBox>("AppID");
            appID_Input.LostFocus += UpdateAppId;

            configPath_Input = this.FindControl<TextBox>("ConfigPath");
            configPath_Input.LostFocus += UpdateConfigPath;

            configButton = this.FindControl<Button>("GenerateConfigButton");
            //!TextInput event is never called? appID_Input.TextInput += OnTextInput;
            //appID_Input.KeyDown can be used to detect Return press, but doesnt help...
            //appID_Input.LostFocus may be an alternative, but is only called when selecting something else.
        }

        private void UpdateAppId(object sender, RoutedEventArgs e)
        {
            string temp = this.appID_Input.Text;
            if (string.IsNullOrEmpty(temp))
                return;
            if (Regex.IsMatch(temp, "^[0-9]*$", RegexOptions.None))
            {
                if (temp.Length == 18)
                {
                    //this is a valid appID!
                    ConfigHost.ActiveConfig.applicationID = temp;
                }
            } 
            else
            {
                appID_Input.Text = ConfigHost.ActiveConfig.applicationID;
            }
        }

        void UpdateConfigPath(object sender, RoutedEventArgs args)
        {
            string path = this.configPath_Input.Text;
            if (string.IsNullOrEmpty(path))
            {
                configButton.IsEnabled = false;
                return;
            }
            if (System.IO.Path.IsPathFullyQualified(path) && !File.Exists(path))
            {
                ConfigHost.ActiveConfig.dispatchConfigLocation = path;
                configButton.IsEnabled = true;
            }
            else
                configButton.IsEnabled = false;
        }
    }
}
