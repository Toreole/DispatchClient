using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Net;
using System;
using System.ComponentModel;

namespace InstallerWindows
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Program program = new Program();
            program.Run();

            Console.WriteLine("Setup will exit now.");
            Console.ReadKey();
        }

        readonly Uri downloadUri = new Uri("PLACEHOLDER_URL/RELEASE_FILE");
        readonly string tempFile = "TEMP_FILE";

        ProgressBar progressBar;
        WebClient client;
        string installationPath;
        string tempFilePath;

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
