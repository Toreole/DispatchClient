using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DispatchGUI.Services;
using System;
using System.Globalization;
using System.IO;
using System.Reactive;
using System.Text.RegularExpressions;

namespace DispatchGUI.Views
{
    public class ConfigView : UserControl
    {

        public ConfigView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
