using DispatchGUI.Models;
using System.IO;

namespace DispatchGUI.Services
{
    static class ConfigHost
    {
        static ProjectConfig activeConfig;
        public static ProjectConfig ActiveConfig { get => activeConfig; }

        /// <summary>
        /// Creates a new project file and returns the path.
        /// </summary>
        public static string CreateNewIn(string directory)
        {
            activeConfig = new ProjectConfig();
            string filePath = Path.Combine(directory, "newDispatchGUIConfig.disgui");

            //TODO: Create the file in the filepath.
            return filePath;
        }

        public static void Save(string path)
        {

        }

        public static void Reload(string path)
        {

        }
    }
}
