using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using System;
using System.ComponentModel;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace InstallerWindows
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Program program = new Program();
            //TODO: option to uninstall?
            program.SetupInstall();

            Console.WriteLine("Press any key to finalize setup.");
            Console.ReadKey();
        }

        //"PLACEHOLDER_URL/RELEASE_FILE"
        readonly Uri downloadUri = new Uri("https://github.com/Toreole/DispatchClient/releases/latest/app.zip");
        readonly string tempFile = "TEMP_DOWNLOAD.zip";

        ProgressBar progressBar;
        WebClient client;
        string installationPath;
        string tempFilePath;
        string exePath;

        string dispatchExe;

        public void SetupInstall()
        {
            Console.WriteLine("Also install Dispatch from dl-dispatch.discordapp.com? (y/n)");
            for (; ; )
            {
                var result = Console.ReadKey();
                if (result.Key == ConsoleKey.Y)
                {
                    GetDispatchAndInstall();
                    break;
                }
                else if (result.Key == ConsoleKey.N)
                {
                    Install();
                    break;
                }
            }
        }

        /// <summary>
        /// Download the win64 Dispatch.exe from discord and add it to the environment PATH
        /// </summary>
        void GetDispatchAndInstall()
        {
            //Select the folder to install dispatch in.
            FolderBrowserDialog dispFolder = new FolderBrowserDialog();
            dispFolder.Description = "Select the folder to install Dispatch by Discord";
            dispFolder.ShowNewFolderButton = true;
            dispFolder.RootFolder = Environment.SpecialFolder.MyComputer;

            var result = dispFolder.ShowDialog();
            //when the result is OK, start the download async.
            if(result == DialogResult.OK)
            {
                DownloadDispatch(dispFolder.SelectedPath);
                AddDispatchToPATH();
                dispFolder.Dispose();
                Install();
            }
            else
            {
                dispFolder.Dispose();
                Console.WriteLine("Failed getting a valid folder.");
            }
        }

        void DownloadDispatch(string installPath)
        {
            Console.WriteLine("Downloading Dispatch...");
            //save the path.
            this.dispatchExe = Path.Combine(installPath, "dispatch.exe");
            //download the thing.
            WebClient dispDownload = new WebClient();
            //downloads the latest win64 dispatch executable from discord.
            dispDownload.DownloadFile("https://dl-dispatch.discordapp.net/download/win64", this.dispatchExe);
            dispDownload.Dispose();
        }

        void AddDispatchToPATH()
        {
            string pathVariable = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            pathVariable = $"{pathVariable};{dispatchExe};";
            Environment.SetEnvironmentVariable("Path", pathVariable, EnvironmentVariableTarget.User);
        }

        public void Install()
        {
            FolderBrowserDialog fileDialog = new FolderBrowserDialog();

            fileDialog.Description = "Select a folder to install DispatchGUI.";
            fileDialog.ShowNewFolderButton = true;
            fileDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            var result = fileDialog.ShowDialog();
            //fileDialog has succesfully found a path.
            if (result == DialogResult.OK)
            {
                this.installationPath = fileDialog.SelectedPath;
                Console.WriteLine($"OK: {fileDialog.SelectedPath}");
                tempFilePath = Path.Combine(this.installationPath, this.tempFile);

                //Initialize the WebClient and the ProgressBar.
                this.client = new WebClient();
                this.progressBar = new ProgressBar();

                //Update the progress of the download.
                client.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => { progressBar.Value = e.ProgressPercentage; };
                progressBar.Name = "Downloading Latest Release from GitHub...";
                
                //Setup the next steps.
                client.DownloadFileCompleted += OnDownloadComplete;

                //Start downloading the zip released from GitHub
                client.DownloadFileAsync(this.downloadUri, this.tempFilePath);
            }
            else
                Console.WriteLine("Failed getting path.");
        }

        //The download is complete, go ahead with installing it.
        void OnDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            this.client.Dispose();
            progressBar.Dispose();
            //Unzip the downloaded release into the installation path.
            ZipFile.ExtractToDirectory(this.tempFilePath, this.installationPath);
            this.exePath = Path.Combine(installationPath, "DispatchGUI.exe");

            AddDisGuiToRegistry();
        }

        void AddDisGuiToRegistry()
        {
            //Setup the registry stuff: context menu.
            //Add the Entry for the context menu on background
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\DispatchGUI", "", "Open DispatchGUI here");
            //Set the icon
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\DispatchGUI", "Icon", $"\"{this.exePath}\", 1");
            //Define the command to run
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\DispatchGUI\command", "", $"\"{this.exePath}\" \"%1\"");

            //Setup the .disgui file type and the connection with the installed program.
            //Define the filetype:
            //SetValue HKEY_CLASSES_ROOT\.disgui disguiproj
            Registry.SetValue(@"HKEY_CLASSES_ROOT\.disgui", "", "disguiproj");
            //SetValue HKEY_CLASSES_ROOT\disguiproj Name:"" Value:"DispatchGUI Project"
            Registry.SetValue(@"HKEY_CLASSES_ROOT\disguiproj", "", "DispatchGUI Project");

            //Default Icon:
            //SetValue HKEY_CLASSES_ROOT\<FileTypeName>\DefaultIcon Name:"" Value:"\"<pathToExe>\", 1"
            Registry.SetValue(@"HKEY_CLASSES_ROOT\disguiproj\DefaultIcon", valueName: "", value: $"\"{this.exePath}\", 1");

            //Set the application to open the file
            //SetValue HKEY_CLASSES_ROOT\<FileTypeName>\shell\open\command Name:"" Value:"\"<pathToExe>\" \"%1\""
            Registry.SetValue(@"HKEY_CLASSES_ROOT\disguiproj\shell\open\command", valueName: "", value: $"\"{this.exePath}\", \"%1\"");

        }
    }
}
