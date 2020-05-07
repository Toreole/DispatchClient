using System;
using System.Collections.Generic;
using System.Text;

namespace DispatchGUI.Models
{
    [Serializable]
    public class ProjectConfig
    {
        //TODO: the project config holds a bunch of information about the project itself.
        //1. the APP ID assigned by discord
        public ulong applicationID;
        //2. the IDs (names if possible) of the branches
        public DispatchBranch[] branches;
        //3. the IDs of the builds (dates of them?)
        [NonSerialized]
        public Dictionary<DispatchBranch, DispatchBuild[]> buildsByBranch;

        //4. the config.json location for dispatch. (absolute path)
        public string dispatchConfigLocation;
    }

    [Serializable]
    public class DispatchBranch
    {
        public ulong branchID;
        public string name;
    }

    public class DispatchBuild
    {
        public ulong buildID;
        public string date;
    }
}
