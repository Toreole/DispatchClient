using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DispatchGUI.Models
{
    [Serializable]
    public class ProjectConfig
    {
        //TODO: the project config holds a bunch of information about the project itself.
        //1. the APP ID assigned by discord
        public string applicationID = "";
        //2. the IDs (names if possible) of the branches
        public ObservableCollection<DispatchBranch> Branches { get; set; }
        //3. the IDs of the builds (dates of them?)

        //4. the config.json location for dispatch. (absolute path)
        public string dispatchConfigLocation = "";

        public ProjectConfig()
        {
            Branches = new ObservableCollection<DispatchBranch>();
        }
    }

    [Serializable]
    public class DispatchBranch
    {
        public string branchID;
        public string name;
        public ObservableCollection<DispatchBuild> BuildsInBranch { get; set; }

        public string BranchNameID => $"{name}: {branchID}";

        public DispatchBranch()
        {
            BuildsInBranch = new ObservableCollection<DispatchBuild>();
        }
        public DispatchBranch(string Id, string name)
        {
            this.name = name;
            branchID = Id;
            BuildsInBranch = new ObservableCollection<DispatchBuild>();
        }
    }

    public class DispatchBuild
    {
        public string buildID;
        public string date;

        public string BuildDate => date;
        public string BuildID => buildID;

        public DispatchBuild()
        {

        }
        public DispatchBuild(string Id, string creationDate)
        {
            this.buildID = Id;
            this.date = creationDate;
        }
    }
}
