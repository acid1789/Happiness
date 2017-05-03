using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PatchLib;
using System.IO;
using System.IO.Compression;

namespace HappinessLauncher
{
    static class Program
    {
        const string UPDATE_FOLDER = "UpdateFolder";
        const string PATCH_SERVER = "http://ec2-54-187-139-124.us-west-2.compute.amazonaws.com/";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            PatchLog.Initialize("Launcher.log");

            #region Update Housekeeping
            string exeFile = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            if (exeFile.Contains(UPDATE_FOLDER))
            {
                PatchLog.Print("Launching from update folder. Copying back to the main folder and restarting...");
                try
                {
                    string parentDir = Path.GetFullPath(Path.GetDirectoryName(exeFile) + "/..") + "\\";
                    string[] files = Directory.GetFiles(Path.GetDirectoryName(exeFile));
                    foreach (string file in files)
                    {
                        string destFile = parentDir + Path.GetFileName(file);
                        if (File.Exists(destFile))
                            File.Delete(destFile);
                        File.Copy(file, destFile);
                    }

                    string regularExe = parentDir + Path.GetFileName(exeFile);
                    PatchLog.Print("Launching: " + regularExe);
                    PatchLog.Shutdown();
                    System.Diagnostics.Process.Start(regularExe);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                PatchLog.Shutdown();
                return;
            }
            if (Directory.Exists(UPDATE_FOLDER))
            {
                PatchLog.Print("Removing Update Folder");
                Directory.Delete(UPDATE_FOLDER, true);
                if (Directory.Exists(UPDATE_FOLDER))
                {
                    System.Threading.Thread.Sleep(3000);
                    Directory.Delete(UPDATE_FOLDER, true);
                }
            }
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            // Load the config
            string appDataFolder = Directory.GetCurrentDirectory() + "/";                        
            string gameDataFolder = appDataFolder + "Game/";
            PatchLog.Print("gameDataFolder = " + gameDataFolder);

            // Do Launcher Update
            try
            {
                if (DoLauncherUpdate())
                {
                    GameUpdate gu = new GameUpdate();
                    gu.Show();
                    gu.Start(PATCH_SERVER);                    

                    while (!gu.Finished)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                PatchLog.Print(ex.ToString());
            }
            PatchLog.Shutdown();
        }

        static bool DoLauncherUpdate()
        {
            // Request launcher manfest from web server
            WebRequest wr = new WebRequest(PATCH_SERVER + "/launcher/launcher.manifest", WebRequest.RequestType.String, null, true);
            PatchLog.Print("Fetching: " + PATCH_SERVER + "/launcher/launcher.manifest");

            // Compare
            string response = wr.WaitForResponseString();
            Manifest remoteManifest = new Manifest(response.Replace("\r", "").Split('\n'));
            bool updateNeeded = false;
#if !DEBUG
            string[] files = remoteManifest.GetFileList();
            foreach (string file in files)
            {
                string fileHash = Hash.HashFile(file);
                string remoteHash = remoteManifest.GetHash(file);
                if (fileHash != remoteHash)
                {
                    PatchLog.Print("Update is requred because hashes dont match for file {0}\n   local: {1}\n  remote: {2}", file, fileHash, remoteHash);
                    updateNeeded = true;
                    break;
                }
            }
#endif

            if (updateNeeded)
            {
                // Local doesnt match the server, get the server version
                if (File.Exists("Launcher.zip"))
                    File.Delete("Launcher.zip");
                PatchLog.Print("Downloading: " + PATCH_SERVER + "launcher/Launcher.zip");
                wr = new WebRequest(PATCH_SERVER + "launcher/Launcher.zip", WebRequest.RequestType.File, "Launcher.zip");
                wr.WaitForResponseData();

                // Unzip the zip
                PatchLog.Print("Extracting Launcher.zip");
                if (Directory.Exists(UPDATE_FOLDER))
                    Directory.Delete(UPDATE_FOLDER, true);
                Directory.CreateDirectory(UPDATE_FOLDER);
                ZipFile.ExtractToDirectory("Launcher.zip", UPDATE_FOLDER);
                File.Delete("Launcher.zip");

                // Run another instance of the launcher
                string exeFile = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                string exePath = Path.GetDirectoryName(exeFile);
                string exeName = Path.GetFileName(exeFile);
                string updateExe = exePath + "/" + UPDATE_FOLDER + "/" + exeName;
                PatchLog.Print("Launching updated version: " + updateExe);
                PatchLog.Shutdown();
                System.Diagnostics.Process.Start(updateExe);

                // Return false here so that this instance exits
                return false;
            }

            // Up to date, continue on
            return true;
        }
    }
}
