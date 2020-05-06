using DispatchGUI.Models;
using System.IO;

namespace DispatchGUI.Services
{
    static class ConfigHost
    {
        static ProjectConfig activeConfig;

        public static void CreateNewIn(string directory)
        {
            activeConfig = new ProjectConfig();

        }

        static void Save(string path)
        {

        }

        static void Reload(string path)
        {

        }
    }
}
