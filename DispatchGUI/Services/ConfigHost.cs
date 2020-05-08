using DispatchGUI.Models;
using System.IO;
using Newtonsoft.Json;

namespace DispatchGUI.Services
{
    static class ConfigHost
    {
        public static ProjectConfig ActiveConfig { get; private set; }

        public static string workingDirectory, workingProjectFile;

        /// <summary>
        /// Creates a new project file and returns the path.
        /// </summary>
        public static string CreateNewIn(string directory)
        {
            ActiveConfig = new ProjectConfig
            {
                dispatchConfigLocation = Path.Combine(directory, "config.json")
            };
            string filePath = Path.Combine(directory, "newDispatchGUIConfig.disgui");

            //TODO: Create the file in the filepath.
            return filePath;
        }

        /// <summary>
        /// JSON Serialize the ActiveConfig to the specified path.
        /// </summary>
        /// <param name="path">the path to the .disgui file</param>
        public static void Save(string path)
        {
            JsonSerializer serializer = JsonSerializer.Create();
            FileStream stream = File.Open(path, FileMode.Create);
            System.IO.StreamWriter writer = new StreamWriter(stream);
            //serialize the config.
            serializer.Serialize(writer, ActiveConfig);
            writer.Flush();
            writer.Dispose();
            stream.Flush();
            stream.Dispose();
        }

        /// <summary>
        /// JSON Deserialize the ActiveConfig from the specified path.
        /// </summary>
        /// <param name="path">the path to the .disgui file</param>
        public static void Reload(string path)
        {
            JsonSerializer serializer = JsonSerializer.Create();
            FileStream stream = File.Open(path, FileMode.Open);
            System.IO.StreamReader reader = new StreamReader(stream);
            //Deserialize the config from the file.
            ActiveConfig = serializer.Deserialize(reader, typeof(ProjectConfig)) as ProjectConfig;
            reader.Dispose();
            stream.Dispose();
        }

        /// <summary>
        /// Needed for debugging.
        /// </summary>
        public static void Empty()
        {
            ActiveConfig = new ProjectConfig();
        }
    }
}
