using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using DispatchGUI.Services;
using System.IO;
using System.Text.RegularExpressions;
using Avalonia.Media;
using System.Collections.ObjectModel;
using DispatchGUI.Models;

namespace DispatchGUI.ViewModels
{
    public class ConfigViewModel : ViewModelBase
    {
        bool canGenerateConfigJson = false;
        public bool CanGenerateConfigJson { get => canGenerateConfigJson; }

        static readonly SolidColorBrush greenBrush = new SolidColorBrush(Color.FromRgb(40, 150, 40));
        static readonly SolidColorBrush redBrush = new SolidColorBrush(Color.FromRgb(170, 30, 30));

        private SolidColorBrush appIdBorderBrush;
        public SolidColorBrush AppIdBorderBrush 
        { 
            get => appIdBorderBrush; 
            private set => this.RaiseAndSetIfChanged(ref appIdBorderBrush, value); 
        }

        public ObservableCollection<DispatchBranch> BranchList => ConfigHost.ActiveConfig.Branches;

        public ConfigViewModel()
        {
            this.WhenAnyValue(x => x.AppID).Subscribe( (input) => { FullValidateId(input); }, () => {});
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

        string appID = "";
        public string AppID
        {
            get => appID;
            set
            {
                this.RaiseAndSetIfChanged(ref appID, value);
            }
        }

        void FullValidateId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                AppIdBorderBrush = redBrush;
                return;
            }
            if (Regex.IsMatch(id, "^[0-9]*$", RegexOptions.None))
            {
                if (id.Length == 18)
                {
                    //this is a valid appID!
                    ConfigHost.ActiveConfig.applicationID = id;
                    AppIdBorderBrush = greenBrush;
                    return;
                }
            }
            AppIdBorderBrush = redBrush;
            //AppID = ConfigHost.ActiveConfig.applicationID;
            return;
        }

        //Note: only use the property to reset things!
        public void ForceRefresh()
        {
            //ConfigJsonPath = ConfigHost.ActiveConfig.dispatchConfigLocation;
            AppID = ConfigHost.ActiveConfig.applicationID;
            if (!string.IsNullOrEmpty(appID))
                AppIdBorderBrush = greenBrush;
            //AppID = ConfigHost.ActiveConfig.applicationID;
        }

        /// <summary>
        /// Generate a bunch of random branches with some builds.
        /// </summary>
        public void GenerateBranches()
        {
            var config = ConfigHost.ActiveConfig;
            for(int i = 0; i < 4; i++)
            {
                var branch = new DispatchBranch("1234567", "Not_Real");
                config.Branches.Add(branch);
                branch.BuildsInBranch.Add(new DispatchBuild("987765","01-01-2020"));
                branch.BuildsInBranch.Add(new DispatchBuild("63636", "02-01-2020"));
            }
        }

        public void Save()
        {
            ConfigHost.Save(ConfigHost.workingProjectFile);
        }
    }
}
