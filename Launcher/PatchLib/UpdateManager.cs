using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace PatchLib
{
    public class UpdateManager
    {
        public Action<string, float, float> OnStatusChange;
        public Action<bool> OnUpdateFinished;
        Thread _thread;

        string _patchServer;
        string _remoteDataPath;
        string _gameDataFolder;

        public UpdateManager(string patchServer, string remoteDataPath, string gameDataFolder)
        {
            PatchLog.Print("Starting UpdateManager(patchServer: {0}, remoteDataPath: {1}, gameDataFolder: {2})", patchServer, remoteDataPath, gameDataFolder);
            _patchServer = patchServer;
            _remoteDataPath = remoteDataPath;
            _gameDataFolder = gameDataFolder;
        }

        public void Start()
        {
            _thread = new Thread(new ThreadStart(DoUpdate));
            _thread.Name = "Update Manager";
            _thread.Start();
        }

        void DoUpdate()
        {
            DateTime start = DateTime.Now;
            // Request remote manifest
            OnStatusChange("Requesting Remote Manifest", 0, 0);
            string serverDataPath = _patchServer + _remoteDataPath;
            PatchLog.Print("Requesting Remote Manifest: " + serverDataPath + "manifest");
            WebRequest wr = new WebRequest(serverDataPath + "manifest", WebRequest.RequestType.String);

            // Load/Generate local manifest
            OnStatusChange("Getting Local Manifest", 0, 0);
            string manifestFileName = _gameDataFolder + "manifest";
            Manifest localManifest = Manifest.Load(manifestFileName);

            // Wait for remote manifest
            OnStatusChange("Waiting for Remote Manifest", 0, 0);
            string result = wr.WaitForResponseString();
            PatchLog.Print("Remote Manifest Status: " + wr.Status);
            if (wr.Status != WebRequest.WebRequestStatus.Download_Complete)
            {
                OnStatusChange("Unable to get manifest from the server", 0, 0);
                OnUpdateFinished(false);
                return;
            }
            Manifest remoteManifest = new Manifest(result.Replace("\r", "").Split('\n'));

            // Compare manifests
            OnStatusChange("Comparing manifests", 0, 0);
            ManifestDifference[] differences = Manifest.Compare(localManifest, remoteManifest);
            PatchLog.Print("Comparing manifests. Differences: " + differences.Length);

            // Delete local files and kickoff downloads
            OnStatusChange("Starting Downloads", 0, 0);
            foreach (ManifestDifference md in differences)
            {
                if (md.Delete)
                {
                    PatchLog.Print("Removing file: " + _gameDataFolder + md.File);
                    File.Delete(_gameDataFolder + md.File);
                }
                else
                {
                    string webFile = md.File.Replace("\\", "/");
                    md.WebFile = webFile.TrimStart('/');
                    PatchLog.Print("Downloading file: {0} -> {1}", serverDataPath + md.WebFile, _gameDataFolder + md.File + ".compressed");
                    md.Request = new WebRequest(serverDataPath + md.WebFile, WebRequest.RequestType.File, _gameDataFolder + md.File + ".compressed");
                }
            }

            // Wait for requests to complete and decompress them
            int doneCount = 0;
            do
            {
                doneCount = 0;
                string currentStatus = null;
                float percent = 0;
                foreach (ManifestDifference md in differences)
                {
                    if (md.Request != null)
                    {
                        if (md.Request.Status >= WebRequest.WebRequestStatus.Download_Canceled)
                        {
                            PatchLog.Print("Download Finished for file {0}. Status: {1}", md.File, md.Request.Status);
                            if (md.Request.Status == WebRequest.WebRequestStatus.Download_Complete)
                            {
                                // This request is done but hasn't been decompressed yet
                                md.Decompress = new Zip();
                                string compressedFile = _gameDataFolder + md.File + ".compressed";
                                md.Decompress.DecompressAsync(compressedFile, _gameDataFolder + md.File);
                                PatchLog.Print("Decompressing {0} -> {1}", compressedFile, _gameDataFolder + md.File);

                                //while (!md.Decompress.Finished) { Thread.Sleep(50); }
                                //md.Decompress = null;

                                // Mark this request done
                                md.Request = null;
                            }
                            else
                            {
                                // This request failed, try it again
                                PatchLog.Print("Failed to download {0}.\n{1}", md.File, md.Request.ResultString);
                                md.Request = new WebRequest(serverDataPath + md.WebFile, WebRequest.RequestType.File, _gameDataFolder + md.File + ".compressed");
                            }
                        }
                        else if (currentStatus == null)
                        {
                            currentStatus = "Downloading: " + md.File;
                            percent = md.Request.PercentComplete / 100.0f;
                        }
                    }
                    else if (md.Decompress != null)
                    {
                        if (md.Decompress.Finished)
                        {                            
                            string compressedFile = _gameDataFolder + md.File + ".compressed";
                            PatchLog.Print("Decompress finished. Deleting compressed file: " + compressedFile);
                            File.Delete(compressedFile);

                            md.Decompress = null;
                        }
                        else if (currentStatus == null)
                        {
                            currentStatus = "Decompressing: " + md.File;
                            percent = md.Decompress.Progress;
                        }
                    }
                    else
                        doneCount++;
                }

                if (currentStatus != null)
                    OnStatusChange(currentStatus, percent, (float)doneCount / (float)differences.Length);
                Thread.Sleep(10);
            } while (doneCount < differences.Length);
            
            // Save the remote manifest as our own
            remoteManifest.Save(manifestFileName);


            // All done
            TimeSpan s = DateTime.Now - start;
            string updateTimeStatus = string.Format("Completed.  ({0} seconds)", s.TotalSeconds);
            OnStatusChange(updateTimeStatus, 1, 1);
            PatchLog.Print(updateTimeStatus);
            OnUpdateFinished(true);
        }
    }
}
