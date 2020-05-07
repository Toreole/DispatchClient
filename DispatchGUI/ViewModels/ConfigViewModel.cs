using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using DispatchGUI.Services;
using System.IO;

namespace DispatchGUI.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        string appID = "";
        bool canGenerateConfigJson = false;
        public bool CanGenerateConfigJson { get => canGenerateConfigJson; }
        public string AppID 
        { 
            get => appID;
            set
            {
                if(ulong.TryParse(value, out ulong result))
                {
                    this.RaiseAndSetIfChanged(ref appID, value);
                }
            }
        }
        public string ConfigJsonPath
        {
            get {
                if (ConfigHost.ActiveConfig != null)
                    return ConfigHost.ActiveConfig.dispatchConfigLocation;
                return ""; 
            }
            set
            {
                if (Path.IsPathFullyQualified(value))
                {
                    canGenerateConfigJson = true;
                    this.RaiseAndSetIfChanged(ref ConfigHost.ActiveConfig.dispatchConfigLocation, value);
                }
                else
                {
                    this.RaiseAndSetIfChanged(ref canGenerateConfigJson, false);
                }
            }
        }
    }
}
