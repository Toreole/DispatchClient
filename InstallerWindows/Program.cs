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
            //program.Run();
            program.TestRegistry();

            Console.WriteLine("Setup will exit now.");
            Console.ReadKey();
        }
        //"PLACEHOLDER_URL/RELEASE_FILE"
        readonly Uri downloadUri = new Uri("https://google.com/");
        readonly string tempFile = "TEMP_FILE";

        ProgressBar progressBar;
        WebClient client;
        string installationPath;
        string tempFilePath;

        public void TestRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("Classes").OpenSubKey("Directory").OpenSubKey("shell");
            //Set the icon: \shell\DispatchGUI "Icon" path_to_exe
            //Text:         \shell\DispatchGUI "" Create DispatchGUI here
            //Command:      \shell\DispatchGUI\command "" <path.exe> %1         //the %1 is the context path.

            //Add the Entry for the context menu on background
            //Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\DispatchGUI", "", "Open DispatchGUI here");
            //Set the icon
            //Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\DispatchGUI", "Icon", "\"<pathToExe>\", 1");
            //Define the command to run
            //Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\Directory\Background\shell\DispatchGUI\command", "", "\"<pathToExe>\" \"%1\"");

            //Define the filetype:
            //SetValue HKEY_CLASSES_ROOT\.disgui disguiproj
            //SetValue HKEY_CLASSES_ROOT\disguiproj Name:"" Value:"DispatchGUI Project"

            //Default Icon:
            //SetValue HKEY_CLASSES_ROOT\<FileTypeName>\DefaultIcon Name:"" Value:"\"<pathToExe>\", 1"

            //Set the application to open the file
            //SetValue HKEY_CLASSES_ROOT\<FileTypeName>\shell\open\command Name:"" Value:"\"<pathToExe>\" \"%1\""
        }

        public void Run()
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
            //Unzip the downloaded release into the installation path.
            ZipFile.ExtractToDirectory(this.tempFilePath, this.installationPath);

            //Setup the registry stuff: context menu.
            
            //Setup the .disgui file type and the connection with the installed program.
        }
    }
}
