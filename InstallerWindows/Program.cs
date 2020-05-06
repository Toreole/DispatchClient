using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using System;
using System.ComponentModel;
using Microsoft.Win32;

namespace InstallerWindows
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Program program = new Program();
            program.Install();
            //TODO: option to uninstall.

            Console.WriteLine("Setup will exit now.");
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
            //Unzip the downloaded release into the installation path.
            ZipFile.ExtractToDirectory(this.tempFilePath, this.installationPath);
            this.exePath = Path.Combine(installationPath, "DispatchGUI.exe");
            
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

            progressBar.Dispose();
        }
    }
}
