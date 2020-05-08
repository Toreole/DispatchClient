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

        //This enables / disables the "fetch branch data" button. very important.
        bool canGetBranches = false;
        public bool CanGetBranches
        {
            get => canGetBranches;
            set => this.RaiseAndSetIfChanged(ref canGetBranches, value);
        }

        void FullValidateId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                AppIdBorderBrush = redBrush;
                CanGetBranches = false;
                return;
            }
            if (Regex.IsMatch(id, "^[0-9]*$", RegexOptions.None))
            {
                if (id.Length == 18)
                {
                    //this is a valid appID!
                    ConfigHost.ActiveConfig.applicationID = id;
                    AppIdBorderBrush = greenBrush;
                    CanGetBranches = true;
                    return;
                }
            }
            AppIdBorderBrush = redBrush;
            //AppID = ConfigHost.ActiveConfig.applicationID;
            CanGetBranches = false;
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
            ConfigHost.Save(Path.GetFullPath("test.disgui"));
            AppIdBorderBrush = new SolidColorBrush(Color.FromRgb(100, 100, 250));
        }

        public void GetBranchesAndBuilds()
        {
            GetBranches();
            GetBuilds();
        }

        //TODO: scanning for the ApplicationID as the first string in a row might be better.
        /// <summary>
        /// Gets all the branches by name and ID.
        /// </summary>
        public void GetBranches()
        {
            string rawData = CmdService.ExecuteDispatchCommand($"branch list {ConfigHost.ActiveConfig.applicationID}");
            string[] chunks = rawData.Split('|');

            //wipe all previous branch data.
            ProjectConfig config = ConfigHost.ActiveConfig;
            config.Branches.Clear();

            for(int i = 1; i < chunks.Length; ) //scan all the data.
            {
                string entry = chunks[^i].Trim();
                //skip empty entries
                if(string.IsNullOrWhiteSpace(entry) || string.IsNullOrEmpty(entry) || !entry.StartsWith("20"))
                {
                    i++;
                    continue;
                }
                var currentBranch = new DispatchBranch();

                //yuuup its hardcoded kekw
                currentBranch.branchID = chunks[^(i + 3)].Trim();
                currentBranch.name = chunks[^(i + 2)].Trim();
                currentBranch.liveBuild = chunks[^(i + 1)].Trim();
                currentBranch.creationDate = entry;
                config.Branches.Add(currentBranch);
                //skip the next few because its not needed.
                i += 5;
            }
        }

        //TODO: scanning for the ApplicationID as the first string in a row might be better.
        /// <summary>
        /// Retrieve the builds of every branch.
        /// </summary>
        void GetBuilds()
        {
            var config = ConfigHost.ActiveConfig;
            foreach(var branch in config.Branches)
            {
                //clear the list of builds.
                branch.BuildsInBranch.Clear();
                string rawData = CmdService.ExecuteDispatchCommand($"build list {config.applicationID} {branch.branchID}");
                string[] chunks = rawData.Split('|');
                //search the data.
                for (int i = 1; i < chunks.Length;)
                {
                    string entry = chunks[^i].Trim();

                    //skip empty entries
                    if (string.IsNullOrWhiteSpace(entry) || string.IsNullOrEmpty(entry) || !entry.StartsWith("20"))
                    {
                        i++;
                        continue;
                    }
                    //construct build from data.
                    var build = new DispatchBuild();
                    build.date = entry;
                    build.creationBranch = chunks[^(i + 1)].Trim();
                    build.buildStatus = chunks[^(i + 2)].Trim();
                    build.buildID = chunks[^(i + 3)].Trim();
                    //add build to branch data.
                    branch.BuildsInBranch.Add(build);
                    //skip ahead.
                    i += 5;
                }
            }
        }
    }
}
